using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LineOfSight : MonoBehaviour {
    public GameObject projectile;
    [Range(0, 1)]
    public float trackingSpeed = 0.1f;
    public float projectileSpeed = 3.0f;
    [Range(1, 5)]
    public float resetTime = 2.0f;
    private Transform parentTransform;
    private Transform initialTransform;
    private bool playerVisible;

    public UnityEvent playerEnteredEvent;
    public UnityEvent viewResetEvent;
    

    void Start() {
        initialTransform = transform;
        parentTransform = transform.parent;
    }

    private void OnTriggerEnter(Collider collider) {
        if(collider.CompareTag("Player")) {
            playerEnteredEvent?.Invoke();
            GameObject spawnedProjectile = Instantiate(projectile, parentTransform.forward + parentTransform.position, parentTransform.rotation);
            spawnedProjectile.GetComponent<Rigidbody>().velocity = spawnedProjectile.transform.forward * projectileSpeed;
            playerVisible = true;
        }
    }

    private void OnTriggerStay(Collider collider) {
        // Follow the player while they are in line of sight
        if (collider.CompareTag("Player")) {
            Quaternion newAngle = Quaternion.LookRotation(collider.gameObject.transform.position - parentTransform.position);
            parentTransform.rotation = Quaternion.Slerp(parentTransform.rotation, newAngle, trackingSpeed);
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("Player")) {
            playerVisible = false;
            StartCoroutine("ResetView");
        }
    }

    IEnumerator ResetView() {
        yield return new WaitForSeconds(resetTime);
        if (!playerVisible) {
            viewResetEvent?.Invoke();
        }
    }
}
