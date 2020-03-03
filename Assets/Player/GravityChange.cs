using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityChange : MonoBehaviour
{
    public float gravity_x;
    public float gravity_y;
    public float gravity_z;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "PlayerTread")
        {
            collision.collider.transform.parent.GetComponent<PlayerMovement>().ChangeGravity(new Vector3(gravity_x, gravity_y, gravity_z));
        }
        if (collision.collider.tag == "Breakable")
        {
            collision.gameObject.GetComponent<WallBreakable>().ChangeGravity(new Vector3(gravity_x, gravity_y, gravity_z));
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "PlayerTread")
        {
            collider.transform.parent.GetComponent<PlayerMovement>().ChangeGravity(new Vector3(gravity_x, gravity_y, gravity_z));
        }
        if (collider.tag == "Breakable")
        {
            gameObject.GetComponent<WallBreakable>().ChangeGravity(new Vector3(gravity_x, gravity_y, gravity_z));
        }
    }
}
