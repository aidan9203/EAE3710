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
