using Unity.Services.Lobbies.Models;
using UnityEditor.Callbacks;
using UnityEngine;

public class CrystalBehavior : MonoBehaviour
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
        
    }

    //this method is called whenever player is colliding with the spotlight
    private void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "Player") {
            Debug.Log("Spotlight: collision with player detected");
            // player.GetComponent<PlayerMovement>().enabled = false; // stop player from moving
            // player.SetActive(false);
            // playerRB.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
            // player.GetComponent<PlayerMovement>().enabled = true; // let player move
            // player.SetActive(true);
            // playerRB.constraints = RigidbodyConstraints.None;
            // playerRB.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }
}
