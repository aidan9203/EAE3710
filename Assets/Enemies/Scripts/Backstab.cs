using UnityEngine;
using UnityEngine.Events;


public class Backstab : MonoBehaviour
{
    [Range(0, 1)]
    public float backstabSensitivity = 0.8f;
    public GameObject spawnOnDeath;

    private Transform playerTransform;
    private GameObject parentReference;
    private bool deathItemSpawned = false;

    public UnityEvent deathEvent;

    void Start() {
        parentReference = transform.parent.gameObject;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

	void OnTriggerEnter(Collider collider)
	{
        if(IsBehindEnemy())
        {
            if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                parentReference.GetComponent<SentryController>().ChangeSkullVisiblity(true);
            }
        }
	}

	private void OnTriggerStay(Collider collider)
	{
		if (collider.CompareTag("Drill") && IsBehindEnemy())
		{
            SpawnDeathItem();
            deathEvent?.Invoke();
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
        if (collision.collider.CompareTag("FallingStalactite"))
        {
            deathEvent?.Invoke();
            Destroy(parentReference);
        }
    }

    private bool IsBehindEnemy()
    {
        // When we changed the mesh, for whatever reason it's default angle is set 90 degrees from the player. So instead of checking if
        // the angles are the same, we check if they're close to right angles with each other
        if (Vector3.Dot(transform.forward.normalized, -(playerTransform.forward).normalized) < (1 - backstabSensitivity))
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
