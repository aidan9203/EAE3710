using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerMovement>().keys.Add(code);
            GameObject.Destroy(this.gameObject);
        }
        else if (collision.collider.tag == "PlayerTread")
        {
            collision.collider.transform.parent.gameObject.GetComponent<PlayerMovement>().keys.Add(code);
            GameObject.Destroy(this.gameObject);
        }
    }
}
