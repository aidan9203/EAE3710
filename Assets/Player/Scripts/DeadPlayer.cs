using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadPlayer : MonoBehaviour
{
    public Vector3 gravity = Vector3.zero;

    Rigidbody body, head, leg_a, leg_b, leg_c, leg_d;

    // Start is called before the first frame update
    void Start()
    {
        body = transform.GetChild(0).GetComponent<Rigidbody>();
        head = transform.GetChild(1).GetComponent<Rigidbody>();
        leg_a = transform.GetChild(2).GetComponent<Rigidbody>();
        leg_b = transform.GetChild(3).GetComponent<Rigidbody>();
        leg_c = transform.GetChild(4).GetComponent<Rigidbody>();
        leg_d = transform.GetChild(5).GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        body.velocity += 10 * gravity * Time.deltaTime;
        head.velocity += 10 * gravity * Time.deltaTime;
        leg_a.velocity += 10 * gravity * Time.deltaTime;
        leg_b.velocity += 10 * gravity * Time.deltaTime;
        leg_c.velocity += 10 * gravity * Time.deltaTime;
        leg_d.velocity += 10 * gravity * Time.deltaTime;
    }
}
