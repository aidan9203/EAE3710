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

    private int tempI = 0; // Used for logging if player is visible
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate direction to player
        Vector3 direction = playerTransform.position - transform.position;
        
        float angle = Vector3.Angle(direction, transform.forward);
        if(angle < viewAngle && Vector3.Distance(transform.position, playerTransform.position) < viewDistance) {
            Debug.Log("Player visible " + tempI);
            tempI++;

            // Rotate towards player
            Quaternion newAngle = Quaternion.LookRotation(playerTransform.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, newAngle, trackingSpeed);
        }
    }



	void OnTriggerEnter(Collider collider)
	{
        if(IsBehindEnemy())
        {
            if (collider.gameObject.tag == "Drill")
            {
                Destroy(transform.gameObject);
            }
            else if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                GetComponent<SentryController>().ChangeSkullVisiblity(true);
            }
        }
	}

	private void OnTriggerStay(Collider collider)
	{
		if (collider.gameObject.tag == "Drill" && IsBehindEnemy())
		{
			Destroy(transform.gameObject);
		}
	}

	private void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			GetComponent<SentryController>().ChangeSkullVisiblity(false);
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
