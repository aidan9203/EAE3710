using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.CompareTag("PlayerModel")) {
            collision.gameObject.SetActive(false);
        }
        // Collided with a wall or something
        Destroy(gameObject);
    }
}
