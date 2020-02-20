using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    void Start() {
        Debug.Log(transform.position);
    }

    void FixedUpdate() {
        
    }

    void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.CompareTag("PlayerModel")) {
            collision.gameObject.GetComponent<PlayerMovement>().alive = false;
        }
        // Collided with a wall or something
        Destroy(gameObject);
    }
}
