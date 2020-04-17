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

    static float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!trigger_only)
        {
            if (collision.collider.tag == "Player" && timer >= 5.0f)
            {
                timer = 0;
                collision.collider.GetComponent<PlayerMovement>().ChangeGravity(new Vector3(gravity_x, gravity_y, gravity_z));
            }
            else if (collision.collider.tag == "Breakable")
            {
                var breakable = collision.gameObject.GetComponent<Breakable>();
                if (breakable != null)
                {
                    collision.gameObject.GetComponent<Breakable>().ChangeGravity(new Vector3(gravity_x, gravity_y, gravity_z));
                }
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (trigger_only)
        {
            if (collider.tag == "Player" && timer >= 5.0f)
            {
                collider.GetComponent<PlayerMovement>().ChangeGravity(new Vector3(gravity_x, gravity_y, gravity_z));
                timer = 0;
            }
            else if (collider.tag == "Breakable")
            {
                // Changed this because the breakable wall was intersecting with another wall that was tagged here
                var breakable = gameObject.GetComponent<Breakable>();
                if (breakable != null)
                {
                    breakable.ChangeGravity(new Vector3(gravity_x, gravity_y, gravity_z));
                }
            }
        }
    }
}
