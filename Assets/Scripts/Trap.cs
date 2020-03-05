/* README:
 * Add this script to any object you want to kill the player on contact
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


	public void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			other.gameObject.GetComponent<PlayerMovement>().alive = false;
		}
	}
}
