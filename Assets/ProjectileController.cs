using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.CompareTag("Player")) {
            collision.gameObject.SetActive(false);
        }
        // Collided with a wall or something
        else {
            Destroy(gameObject);
        }
    }
}
