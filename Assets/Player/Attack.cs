using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [Range(0, 1)]
    public float backstabSensitivity = 0.5f;
    public float backstabDistance = 1.0f;
    // Update is called once per frame
    void Update()
    {
        // Testing for backstab. If we're going to have more weapons,
        // we can include some additional checks in here for those weapons
        if(Input.GetKeyDown(KeyCode.Space)) {
            RaycastHit hit;
            // NOTE: This is currently set to backwards, because right now the player model is inverted in relation to the parent transform
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out hit, backstabDistance)) {
                GameObject hitObject = hit.collider.gameObject;
                if(hitObject.CompareTag("Enemy")) {
                    // Check to see if behind the enemy
                    Transform enemyTransform = hitObject.GetComponent<Transform>();

                    if (Vector3.Dot(enemyTransform.forward.normalized, -(transform.forward).normalized) > backstabSensitivity) {
                        //  Play animation here
                        Destroy(hitObject);
                    }
                }
            }
        }
    }
}
