using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    public Transform playerTransform;
    public float viewAngle = 20.0f;
    public float viewDistance = 10.0f;
    [Range(0, 1)]
    public float trackingSpeed = 0.1f;
    [Range(0, 1)]
    public float backstabSensitivity = 0.9f;
    public GameObject spawnOnDeath;

    private GameObject parentReference;

    void Start() {
        parentReference = transform.parent.gameObject;    
    }

	void OnTriggerEnter(Collider collider)
	{
        if(IsBehindEnemy())
        {
            if (collider.CompareTag("Drill"))
            {
                Destroy(parentReference);
            }
            else if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                parentReference.GetComponent<SentryController>().ChangeSkullVisiblity(true);
            }
        }
	}

	private void OnTriggerStay(Collider collider)
	{
		if (collider.tag == "Drill" && IsBehindEnemy())
		{
			Destroy(parentReference);
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			parentReference.GetComponent<SentryController>().ChangeSkullVisiblity(false);
		}
	}

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "FallingStalactite")
        {
            Destroy(parentReference);
        }
    }

    private bool IsBehindEnemy()
    {
        if (Vector3.Dot(transform.forward.normalized, -(playerTransform.forward).normalized) > backstabSensitivity)
        {
            //  Play animation here
            return true;
        }

        return false;
    }
}
