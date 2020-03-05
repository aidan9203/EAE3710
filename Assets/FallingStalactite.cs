/* README:
 * Add this script to an object you want to fall when hit with the drill
 * The object must have a rigidbody attatched with gravity set to false and kinematic set to true
 * The gravtiy of this object will be set to the player's gravity when hit
 */

 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingStalactite : MonoBehaviour
{
    Rigidbody rb;
    public Vector3 gravity;
    bool fallen = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fallen)
        {
            rb.velocity += 10 * gravity * Time.deltaTime;
        }
    }

    public void OnTriggerStay(Collider collider)
    {
        if (collider.tag == "Drill")
        {
            rb.isKinematic = false;
            fallen = true;
        }
    }
}
