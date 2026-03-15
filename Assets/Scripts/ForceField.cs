using Unity.Services.Lobbies.Models;
using UnityEditor.Callbacks;
using UnityEngine;

public class ForceField : MonoBehaviour
{

    [SerializeField] private GameObject player;
    private Rigidbody playerRB; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRB = player.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // ========== PHASING THROUGH THE FORCE FIELDS ==================
        if (Input.GetKeyDown(KeyCode.Q))
        {
            // Debug.Log($"Q pressed!");
            // TODO: CALL FUNCTION
        }
        // ==============================================================
    }
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Player") {
            Debug.Log("Force Field: collision with player detected");
            // playerRB.linearVelocity = new Vector3(10f, 10f, 100f);
            playerRB.linearVelocity -= player.transform.forward * 100f * Time.deltaTime;
            playerRB.linearVelocity += player.transform.up * 100f * Time.deltaTime;
        }
    }
}
