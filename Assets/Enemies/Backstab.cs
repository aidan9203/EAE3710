using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Backstab : MonoBehaviour
{
    public Transform playerTransform;
    [Range(0, 1)]
    public float backstabSensitivity = 0.9f;
    public GameObject spawnOnDeath;

    private GameObject parentReference;
    private bool deathItemSpawned = false;

    void Start() {
        parentReference = transform.parent.gameObject;    
    }

	void OnTriggerEnter(Collider collider)
	{
        if(IsBehindEnemy())
        {
            if (collider.CompareTag("Drill"))
            {
                SpawnDeathItem();
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
            SpawnDeathItem();
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

    private void SpawnDeathItem() {
        if(spawnOnDeath != null && !deathItemSpawned) {
            deathItemSpawned = true;
            Instantiate(spawnOnDeath, parentReference.transform.position, spawnOnDeath.transform.rotation);
        }
    }
}
