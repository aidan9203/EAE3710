using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBreakable : MonoBehaviour
{
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
        if (collider.tag == "Drill") {
            rb.isKinematic = false;
            gravity = -collider.transform.parent.GetComponent<PlayerMovement>().GetGravity();
        }
    }

    public void ChangeGravity(Vector3 g)
    {
        g.Normalize();
        gravity = g;
    }
}
