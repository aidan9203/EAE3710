using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    // What is the angle behind the enemy you can be for a backstab to work?
    [Range(0, 1)]
    public float backstabSensitivity = 0.9f;
    private GameObject currentEnemy = null;

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Enemy")) {
            currentEnemy = other.gameObject;
            if(IsBehindEnemy(currentEnemy.GetComponent<Transform>()))
            {
                currentEnemy.GetComponent<SentryController>().ChangeSkullVisiblity(true);
            }
        }
    }

    private void OnTriggerStay(Collider other) {
        if(other.gameObject.CompareTag("Enemy")) {
            // Check to see if behind the enemy
            Transform enemyTransform = other.GetComponent<Transform>();
            if(Input.GetKeyDown(KeyCode.Space)) {
                if (IsBehindEnemy(enemyTransform)) {
                    //  Play animation here
                    Destroy(other.gameObject);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if(other.gameObject == currentEnemy) {
            currentEnemy.GetComponent<SentryController>().ChangeSkullVisiblity(false);
        }
    }

    private bool IsBehindEnemy(Transform enemyTransform)
    {
        if (Vector3.Dot(enemyTransform.forward.normalized, -(transform.forward).normalized) > backstabSensitivity)
        {
            //  Play animation here
            return true;
        }

        return false;
    }
}
