using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour
{
    public float speed = 2f;
    public List<Color> colors = new List<Color>();

    [SerializeField] private GameObject spawnedPrefab;
    private GameObject instantiatedPrefab;

    public GameObject cannon;
    public GameObject bullet;

    [SerializeField] private AudioListener audioListener;
    [SerializeField] private Camera playerCamera;

    public float rotationSpeed = 90;
    public float force = 700f;
    
    Rigidbody rb;
    Transform t;

    // Lives and game state tracking
    private NetworkVariable<int> lives = new NetworkVariable<int>(3);
    public bool isFirewall = true; // Will be set in OnNetworkSpawn
    private NetworkVariable<int> thievesFound = new NetworkVariable<int>(0);
    private const int TOTAL_THIEVES = 5;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        t = GetComponent<Transform>();
    }

    void Update()
    {
        if (!IsOwner) return;

        Vector3 moveDirection = new Vector3(0, 0, 0);

        // Forward/backward movement
        if (Input.GetKey(KeyCode.W))
        {
            rb.linearVelocity += this.transform.forward * speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            rb.linearVelocity -= this.transform.forward * speed * Time.deltaTime;
        }

        // Rotation
        if (Input.GetKey(KeyCode.A))
        {
            t.rotation *= Quaternion.Euler(0, -rotationSpeed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            t.rotation *= Quaternion.Euler(0, rotationSpeed * Time.deltaTime, 0);
        }

        transform.position += moveDirection * speed * Time.deltaTime;

        // ========== Jump with SPACE (works for BOTH players) ==========
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * 15f, ForceMode.Impulse);
            Debug.Log("Jump!");
        }
        // ===============================================================

        // ========== Only FIREWALL (CLIENT) can guess with G ==========
        if (isFirewall && Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log($"G pressed! Firewall at {transform.position}");
            GuessBoxServerRpc();
        }
        // ==============================================================

        // Keep original spawning functionality (for testing)
        if (Input.GetKeyDown(KeyCode.I))
        {
            instantiatedPrefab = Instantiate(spawnedPrefab);
            instantiatedPrefab.GetComponent<NetworkObject>().Spawn(true);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            instantiatedPrefab.GetComponent<NetworkObject>().Despawn(true);
            Destroy(instantiatedPrefab);
        }

        // Debug: print position
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log($"Player position: {transform.position}");
        }
    }

    [ServerRpc]
    void GuessBoxServerRpc()
    {
        Debug.Log("GuessBoxServerRpc called on server!");
        
        // Get ALL raycast hits, then find first non-player hit
        Vector3 rayStart = transform.position + Vector3.up * 5f;
        RaycastHit[] hits = Physics.RaycastAll(rayStart, Vector3.down, 20f);
        
        Debug.Log($"RaycastAll found {hits.Length} hits");
        
        // Find the first hit that's NOT the player
        foreach (RaycastHit hit in hits)
        {
            Debug.Log($"Checking hit: {hit.collider.gameObject.name}");
            
            // Skip if it's the player
            if (hit.collider.gameObject == this.gameObject)
            {
                Debug.Log("Skipping player");
                continue;
            }
            
            // Found a non-player object - check if it's a box
            BoxController box = hit.collider.GetComponent<BoxController>();
            if (box != null)
            {
                Debug.Log($"Found box: {hit.collider.gameObject.name}");
                
                if (!box.IsRevealed())
                {
                    bool foundThief = box.RevealBox();
                    
                    if (foundThief)
                    {
                        thievesFound.Value++;
                        Debug.Log($"THIEF FOUND! Total: {thievesFound.Value}/{TOTAL_THIEVES}");
                        
                        if (thievesFound.Value >= TOTAL_THIEVES)
                        {
                            GameOverClientRpc(true);
                        }
                    }
                    else
                    {
                        lives.Value--;
                        Debug.Log($"WRONG! Lives remaining: {lives.Value}");
                        UpdateLivesClientRpc(lives.Value);
                        
                        if (lives.Value <= 0)
                        {
                            GameOverClientRpc(false);
                        }
                    }
                    
                    return;
                }
                else
                {
                    Debug.Log("Box already revealed");
                    return;
                }
            }
        }
        
        Debug.Log("No box found under player!");
    }

    [ClientRpc]
    void UpdateLivesClientRpc(int newLives)
    {
        Debug.Log($"LIVES UPDATE: {newLives} remaining");
    }

    [ClientRpc]
    void GameOverClientRpc(bool firewallWon)
    {
        if (firewallWon)
        {
            Debug.Log("FIREWALL WINS! All thieves caught!");
        }
        else
        {
            Debug.Log("GAME OVER! Firewall ran out of lives.");
        }
    }

    public override void OnNetworkSpawn()
    {
        GetComponent<MeshRenderer>().material.color = colors[(int)OwnerClientId];

        // Role Assignment
        if (IsHost)
        {
            isFirewall = false; // Host is Hacker
            Debug.Log("This player is the HACKER (Host) - gives hints");
        }
        else
        {
            isFirewall = true; // Client is Firewall
            Debug.Log("This player is the FIREWALL (Client) - finds thieves");
        }

        if (!IsOwner) return;
        
        audioListener.enabled = true;
        playerCamera.enabled = true;
    }
}