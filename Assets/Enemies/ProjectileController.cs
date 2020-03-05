using UnityEngine;
using UnityEngine.Events;

public class ProjectileController : MonoBehaviour
{
    public UnityEvent playerHitEvent;

    void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.CompareTag("Player")) {
            collision.gameObject.GetComponent<PlayerMovement>().alive = false;
            playerHitEvent?.Invoke();
        }
        // Collided with a wall or something
        Destroy(gameObject);
    }
}
