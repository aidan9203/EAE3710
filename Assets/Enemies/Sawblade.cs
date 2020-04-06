using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sawblade : MonoBehaviour
{
    public Vector3 distance;
    public float speed;
    public float offset;

    Vector3 start;
    float timer;

    // Start is called before the first frame update
    void Start()
    {
        start = transform.position;
        timer = offset;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawLine(start, start + distance, Color.red, 0.1f);

        //Linear motion
        timer += Time.deltaTime;
        if (timer > speed) { timer -= speed; }

        float frac = timer / speed;
        if (frac > 0.5f) { frac = 1 - frac; }
        
        transform.position = start + 2 * frac * distance;

        //Rotational motion
        transform.RotateAround(transform.position, transform.up, -250 * Time.deltaTime);
    }


    public void OnCollisionEnter(Collision other)
    {
        if (other.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.collider.gameObject.GetComponent<PlayerMovement>().alive = false;
        }
    }
}
