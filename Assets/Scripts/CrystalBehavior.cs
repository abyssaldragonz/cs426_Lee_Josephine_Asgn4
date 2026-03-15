using Unity.VisualScripting;
using UnityEngine;

public class CrystalBehavior : MonoBehaviour
{

    [SerializeField] private GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //this method is called whenever player is colliding with the spotlight
    private void OnTriggerStay(Collider other) {
        if (other.gameObject.tag == "Player") {
            Debug.Log("Spotlight: collision with player detected");
            var playerRB = other.GetComponent<Rigidbody>();
            // stop player from moving
            other.GameObject().GetComponent<PlayerMovement>().enabled = false; 
            // other.GameObject().SetActive(false);
            playerRB.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "Player") {
            var playerRB = other.GetComponent<Rigidbody>();
            // let player move
            other.GameObject().GetComponent<PlayerMovement>().enabled = true; 
            // other.GameObject().SetActive(true);
            playerRB.constraints = RigidbodyConstraints.None;
            playerRB.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }
}
