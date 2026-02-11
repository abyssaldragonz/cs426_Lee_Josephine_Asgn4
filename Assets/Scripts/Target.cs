using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// adding namespaces
using Unity.Netcode;

public class Target : NetworkBehaviour
{

    //this method is called whenever a collision is detected
    private void OnCollisionEnter(Collision collision)
    {
        // printing if collision is detected on the console
        if (collision.gameObject.tag == "Bullet") {
            Debug.Log("Collision Detected");

            // if the collision is detected destroy the object
            DestroyTargetServerRpc();

            // also destroy bullet
            GameObject col = collision.gameObject;
            // col.GetComponent<NetworkObject>().Despawn(true);
            Destroy(col);
        }
    }

    // client can not spawn or destroy objects
    // so we need to use ServerRpc
    // we also need to add RequireOwnership = false
    // because we want to destroy the object even if the client is not the owner
    [ServerRpc(RequireOwnership = false)]
    public void DestroyTargetServerRpc()
    {
        //despawn
        GetComponent<NetworkObject>().Despawn(true);
        //after collision is detected destroy the gameobject
        Destroy(gameObject);
    }
}