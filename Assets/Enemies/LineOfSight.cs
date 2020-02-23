using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour {
    public GameObject projectile;
    [Range(0, 1)]
    public float trackingSpeed = 0.1f;
    public float projectileSpeed = 3.0f;
    private Transform parentTransform;

    void Start() {
        parentTransform = transform.parent;
    }

    private void OnTriggerEnter(Collider collider) {
        if(collider.CompareTag("Player")) {
            GameObject spawnedProjectile = Instantiate(projectile, parentTransform.forward + parentTransform.position, parentTransform.rotation);
            spawnedProjectile.GetComponent<Rigidbody>().velocity = spawnedProjectile.transform.forward * projectileSpeed;
        }
    }

    private void OnTriggerStay(Collider collider) {
        // Follow the player while they are in line of sight
        if (collider.CompareTag("Player")) {
            Quaternion newAngle = Quaternion.LookRotation(collider.gameObject.transform.position - parentTransform.position);
            parentTransform.rotation = Quaternion.Slerp(parentTransform.rotation, newAngle, trackingSpeed);
        }
    }
}
