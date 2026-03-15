using UnityEngine;
using TMPro;
using UnityEngine.UI;
using TMPro.EditorUtilities;

public class CoinBehavior : MonoBehaviour
{
    [SerializeField] public GameObject item_icon; // icon to show up in UI
    [SerializeField] private RectTransform inventory;

 
    //this method is called whenever a collision is detected
    private void OnCollisionEnter(Collision collision) {
        //on collision adding point to the score
        if (collision.gameObject.tag == "Player") {
            // add to inventory
            GameObject newItem = Instantiate(item_icon);
            newItem.transform.SetParent(inventory, true);

            // printing if collision is detected on the console
            Debug.Log("Coin: Collision with player detected");
            //after collision is detected destroy the gameobject
            Destroy(gameObject);
        }
    }
}
