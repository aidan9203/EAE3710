using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityChange : MonoBehaviour
{
    public int gravity_x;
    public int gravity_y;
    public int gravity_z;

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
        if (collision.collider.tag == "Player")
        {
            collision.collider.transform.parent.GetComponent<PlayerMovement>().ChangeGravity(new Vector3(gravity_x, gravity_y, gravity_z));
        }
        if (collision.collider.tag == "Breakable")
        {
            collision.gameObject.GetComponent<WallBreakable>().ChangeGravity(new Vector3(gravity_x, gravity_y, gravity_z));
        }
    }
}
