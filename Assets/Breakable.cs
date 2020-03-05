/* README:
 * Add this script to each piece of a breakable object
 * destroy_tag is the tag of the object that can destroy it
 * 
 * This piece will need a rigidbody with gravity set to false and kinematic set to true
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
	public string destroy_tag;

    Rigidbody rb;
    Vector3 gravity = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity += 10 * gravity * Time.deltaTime;
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == destroy_tag) {
            rb.isKinematic = false;
            gravity = collider.transform.parent.GetComponent<PlayerMovement>().GetGravity();
        }
    }

	public void  OnCollisionEnter(Collision collision)
	{
		if (collision.collider.tag == destroy_tag)
		{
			rb.isKinematic = false;
			gravity = collision.collider.transform.parent.GetComponent<PlayerMovement>().GetGravity();
		}
	}

	public void ChangeGravity(Vector3 g)
    {
        g.Normalize();
        gravity = g;
    }
}
