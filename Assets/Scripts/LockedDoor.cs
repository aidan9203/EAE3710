﻿/* README:
 * Add this script to any object you want to be unlocked by the player
 * code is a key string needed to unlock this object
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedDoor : MonoBehaviour
{
    public string code;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player" && collision.gameObject.GetComponent<PlayerMovement>().keys.Contains(code))
        {
            GameObject.Destroy(this.gameObject);
        }
        else if (collision.collider.tag == "PlayerTread" && collision.collider.transform.parent.gameObject.GetComponent<PlayerMovement>().keys.Contains(code))
        {
            GameObject.Destroy(this.gameObject);
        }
    }
}
