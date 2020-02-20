using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeCollider : MonoBehaviour {
    [Range(0, 1)]
    public float trackingSpeed = 0.1f;
    [Range(0, 1)]
    public float backstabSensitivity = 0.9f;

    private Transform parentTransform;

    void Start() {
        parentTransform = transform.parent;
    }

    private void OnTriggerEnter(Collider collider) {
        if(collider.CompareTag("Player")) {
            Debug.Log("Player entered cone");
        }
    }

    private void OnTriggerStay(Collider collider) {
        if (collider.CompareTag("Player")) {
            // Rotate towards player
            Quaternion newAngle = Quaternion.LookRotation(collider.gameObject.transform.position - parentTransform.position);
            parentTransform.rotation = Quaternion.Slerp(parentTransform.rotation, newAngle, trackingSpeed);
        }
    }
}
