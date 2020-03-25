/* Readme:
 * Add this script to any object that you want to change the player's gravity
 * The direction of gravity can be set with gravity_x, gravtiy_y, and gravity_z which correspond to the game world axes
 * The direction of gravity must be in only one direction (i.e. if gravity_x=-1, gravity_y=0 and gravtiy_z=0)
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityChange : MonoBehaviour
{
    public float gravity_x;
    public float gravity_y;
    public float gravity_z;

    public bool trigger_only;

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
        if (!trigger_only)
        {
            if (collision.collider.tag == "PlayerTread")
            {
                collision.collider.transform.parent.GetComponent<PlayerMovement>().ChangeGravity(new Vector3(gravity_x, gravity_y, gravity_z));
            }
            if (collision.collider.tag == "Breakable")
            {
                collision.gameObject.GetComponent<Breakable>().ChangeGravity(new Vector3(gravity_x, gravity_y, gravity_z));
            }
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
            gameObject.GetComponent<Breakable>().ChangeGravity(new Vector3(gravity_x, gravity_y, gravity_z));
        }
    }
}
