using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.CompareTag("Player")) {
            collision.gameObject.GetComponent<PlayerMovement>().alive = false;
        }
        // Collided with a wall or something
        Destroy(gameObject);
    }
}
