using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

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
    public float sensitivityX = 15f;

    Rigidbody rb;
    Transform t;
    float mouseX;

    bool isGrounded;

    // Lives and game state tracking
    private NetworkVariable<int> lives = new NetworkVariable<int>(3);
    public bool isFirewall = true; // Will be set in OnNetworkSpawn
    private NetworkVariable<int> thievesFound = new NetworkVariable<int>(0);
    private const int TOTAL_THIEVES = 5;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        t = GetComponent<Transform>();
        isGrounded = false;
    }

    void Update()
    {
        if (!IsOwner) return;

        // Forward/backward movement
        if (Input.GetKey(KeyCode.W))
        {
            rb.linearVelocity += this.transform.forward * speed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            rb.linearVelocity -= this.transform.forward * speed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            // t.rotation *= Quaternion.Euler(0, -rotationSpeed * Time.deltaTime, 0);
            rb.linearVelocity -= this.transform.right * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            // t.rotation *= Quaternion.Euler(0, rotationSpeed * Time.deltaTime, 0);
            rb.linearVelocity += this.transform.right * speed * Time.deltaTime;
        }

        // Camera rotation
        float h = 500f * Input.GetAxis("Mouse X") * Time.deltaTime;
        t.transform.Rotate(0, h, 0);

        // ========== Jump with SPACE (works for BOTH players) ==========
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
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

    void OnCollisionEnter(Collision coll)
    {
        if(coll.gameObject.tag == "Floor" || coll.gameObject.tag == "Box")
        {
            isGrounded = true;
        }
    }
    void OnCollisionExit(Collision coll)
    {
        if(coll.gameObject.tag == "Floor" || coll.gameObject.tag == "Box")
        {
            isGrounded = false;
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

        // Update UI if this is the Firewall player
        if (isFirewall && GameUIManager.Instance != null)
        {
            GameUIManager.Instance.UpdateLivesDisplay(newLives);
        }
    }

    [ClientRpc]
    void GameOverClientRpc(bool firewallWon)
    {
        if (firewallWon)
        {
            Debug.Log("PLAYERS WIN! All thieves caught!");
        }
        else
        {
            Debug.Log("GAME OVER! Firewall ran out of lives.");
        }

        // Show game over screen
        if (GameUIManager.Instance != null)
        {
            GameUIManager.Instance.ShowGameOver(firewallWon);
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
            this.transform.position = new Vector3(0f, 30f, -10f); // Move Host (Hacker)
            playerCamera.transform.Rotate(75f, 0, 0);
        }
        else
        {
            isFirewall = true; // Client is Firewall
            Debug.Log("This player is the FIREWALL (Client) - finds thieves");
            this.transform.position = new Vector3(0f, 2f, -10f); // Move Client (Firewall)

            // Initialize lives display for Firewall
            if (GameUIManager.Instance != null)
            {
                GameUIManager.Instance.UpdateLivesDisplay(lives.Value);
            }
        }

        if (!IsOwner) return;

        audioListener.enabled = true;
        playerCamera.enabled = true;
    }
}