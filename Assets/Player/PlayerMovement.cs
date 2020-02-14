using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public float speed;

	Vector3 gravity = Vector3.down;

	Transform tf;
	Rigidbody rb;

	float rotation = 0;
	float rotation_d = 0;
	float rotation_offset = 0;

    // Start is called before the first frame update
    void Start()
    {
		tf = GetComponent<Transform>();
		rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
		float input_vertical = 0;
		float input_horizontal = 0;
		float input_rotate = 0;

		//Basic motion
		if (Input.GetKey(KeyCode.W)) { input_vertical += 1; }
		if (Input.GetKey(KeyCode.S)) { input_vertical -= 1; }
		if (Input.GetKey(KeyCode.D)) { input_horizontal += 1; }
		if (Input.GetKey(KeyCode.A)) { input_horizontal -= 1; }

		if (Input.GetKey(KeyCode.E)) { input_rotate += 1; }
		if (Input.GetKey(KeyCode.Q)) { input_rotate -= 1; }

		rotation += 180.0f * input_rotate * Time.deltaTime;

		float move_horizontal = input_horizontal * Mathf.Cos(Mathf.Deg2Rad * (rotation + rotation_offset)) + input_vertical * Mathf.Sin(Mathf.Deg2Rad * (rotation + rotation_offset));
		float move_vertical = input_vertical * Mathf.Cos(Mathf.Deg2Rad * (rotation + rotation_offset)) - input_horizontal * Mathf.Sin(Mathf.Deg2Rad * (rotation + rotation_offset));

		//Orient the player with gravity
		tf.up = Vector3.Lerp(tf.up, -gravity, 0.05f);
		rotation_d = Vector2.Lerp(new Vector2(rotation_d, 0), new Vector2(rotation_offset, 0), 0.05f).x;
		tf.RotateAround(tf.position, tf.up, rotation + rotation_d);

		//Apply motion relative to gravity
		float vel_x = speed * (-Mathf.Abs(gravity.y) * move_horizontal - Mathf.Abs(gravity.z) * move_horizontal) + (Mathf.Abs(rb.velocity.x) + 9.81f * Time.deltaTime) * gravity.x;
		float vel_y = speed * (-gravity.x * move_horizontal - gravity.z * move_vertical) + (Mathf.Abs(rb.velocity.y) + 9.81f * Time.deltaTime) * gravity.y;
		float vel_z = speed * (gravity.y * move_vertical - Mathf.Abs(gravity.x) * move_vertical) + (Mathf.Abs(rb.velocity.z) + 9.81f * Time.deltaTime) * gravity.z;

		rb.velocity = new Vector3(vel_x, vel_y, vel_z);
	}


	/// <summary>
	/// Sets the direction of the player's gravity
	/// </summary>
	public void ChangeGravity(Vector3 g)
	{
		g.Normalize();
		if (gravity.x > 0 && g.z > 0) { rotation_offset -= 90; }
		if (gravity.z > 0 && g.x > 0) { rotation_offset += 90; }

		if (gravity.x > 0 && g.z < 0) { rotation_offset += 90; }
		if (gravity.z > 0 && g.x < 0) { rotation_offset -= 90; }

		if (gravity.x < 0 && g.z > 0) { rotation_offset += 90; }
		if (gravity.z < 0 && g.x > 0) { rotation_offset -= 90; }

		if (gravity.x < 0 && g.z < 0) { rotation_offset -= 90; }
		if (gravity.z < 0 && g.x < 0) { rotation_offset += 90; }

		if (gravity.x != 0 && g.y > 0) { rotation_offset += 180; }
		if (gravity.y > 0 && g.x != 0) { rotation_offset -= 180; }
		gravity = g;
	}
}
