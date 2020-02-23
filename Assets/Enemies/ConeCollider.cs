using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeCollider : MonoBehaviour {
    public GameObject projectile;
    [Range(0, 1)]
    public float trackingSpeed = 0.1f;
    public float projectileSpeed = 3.0f;
    private Transform parentTransform;

    void Start() {
        parentTransform = transform.parent;
    }

    private void OnTriggerEnter(Collider collider) {
        if(collider.CompareTag("PlayerModel")) {
            GameObject spawnedProjectile = Instantiate(projectile, parentTransform.forward + parentTransform.position, parentTransform.rotation);
            spawnedProjectile.GetComponent<Rigidbody>().velocity = spawnedProjectile.transform.forward * projectileSpeed;
        }
    }

    private void OnTriggerStay(Collider collider) {
        if (collider.CompareTag("PlayerModel")) {
            // Rotate towards player
            Quaternion newAngle = Quaternion.LookRotation(collider.gameObject.transform.position - parentTransform.position);
            parentTransform.rotation = Quaternion.Slerp(parentTransform.rotation, newAngle, trackingSpeed);
        }
    }
}
