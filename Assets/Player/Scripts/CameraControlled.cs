using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlled : MonoBehaviour
{
    public GameObject target;
    public float speed_move;
    public float speed_rotate;
    public float sensitivity;
    public float distance;
    public float upward_offset;

    Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        direction = Vector3.Normalize(target.transform.right + new Vector3(0, 1.0f, 0));
        transform.position = target.transform.position + direction * distance;
        transform.rotation = Quaternion.LookRotation(((target.transform.position + target.transform.up * upward_offset) - transform.position).normalized, target.transform.up);
    }

    // Update is called once per frame
    void Update()
    {
        float input_vertical = Input.GetAxis("Look Vertical") + Input.GetAxis("Look Vertical Controller");
        float input_horizontal = Input.GetAxis("Look Horizontal") + Input.GetAxis("Look Horizontal Controller");

        direction = Quaternion.AngleAxis(sensitivity * input_horizontal * Time.deltaTime, target.transform.up) * direction;
        direction = Quaternion.AngleAxis(sensitivity * input_vertical * Time.deltaTime, transform.right) * direction;
        direction = direction.normalized;

        float dist = distance;
        RaycastHit hit;
        if (Physics.Raycast(target.transform.position + target.transform.up * upward_offset, direction, out hit, distance, ~LayerMask.GetMask("Player")))
        {
            dist = hit.distance - 0.1f;
        }

        transform.position = Vector3.Lerp(transform.position, target.transform.position + target.transform.up * upward_offset + direction * dist, speed_move);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(((target.transform.position + target.transform.up * upward_offset) - transform.position).normalized, target.transform.up), speed_rotate);
    }
}
