using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	Vector3 gravity = Vector3.down;

	Transform tf;
	Rigidbody rb;

	float rotation = 0;

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

		float move_horizontal = input_horizontal * Mathf.Cos(Mathf.Deg2Rad * rotation) + input_vertical * Mathf.Sin(Mathf.Deg2Rad * rotation);
		float move_vertical = input_vertical * Mathf.Cos(Mathf.Deg2Rad * rotation) - input_horizontal * Mathf.Sin(Mathf.Deg2Rad * rotation);

		//Orient the player with gravity
		tf.up = Vector3.Lerp(tf.up, -gravity, 0.05f);
		tf.RotateAround(tf.position, tf.up, rotation);

		//Apply motion relative to gravity
		float vel_x = 3 * (-Mathf.Abs(gravity.y) * move_horizontal - Mathf.Abs(gravity.z) * move_horizontal);
		float vel_y = 3 * (-gravity.x * move_horizontal - gravity.z * move_vertical);
		float vel_z = 3 * (gravity.y * move_vertical - Mathf.Abs(gravity.x) * move_vertical);

		rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(vel_x, vel_y, vel_z), 0.1f);

		//Apply smooth motion
		/*
		float angle = tf.eulerAngles.y + 180.0f * rotate * Time.deltaTime;
		tf.eulerAngles = new Vector3(tf.eulerAngles.x, angle, tf.eulerAngles.z);
		float angle_rad = Mathf.Deg2Rad * angle;
		float vel_x = -3 * (Mathf.Cos(angle_rad) * move_horizontal + Mathf.Sin(angle_rad) * move_vertical);
		float vel_z = 3 * (Mathf.Sin(angle_rad) * move_horizontal - Mathf.Cos(angle_rad) * move_vertical);
		rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(vel_x, rb.velocity.y, vel_z), 0.1f);
		*/
	}


	/// <summary>
	/// Sets the direction of the player's gravity
	/// </summary>
	public void ChangeGravity(Vector3 g)
	{
		g.Normalize();
		if (gravity.x > 0 && g.z > 0) { rotation -= 90; }
		if (gravity.z > 0 && g.x > 0) { rotation += 90; }

		if (gravity.x > 0 && g.z < 0) { rotation += 90; }
		if (gravity.z > 0 && g.x < 0) { rotation -= 90; }

		if (gravity.x < 0 && g.z > 0) { rotation += 90; }
		if (gravity.z < 0 && g.x > 0) { rotation -= 90; }

		if (gravity.x < 0 && g.z < 0) { rotation -= 90; }
		if (gravity.z < 0 && g.x < 0) { rotation += 90; }

		if (gravity.x != 0 && g.y > 0) { rotation += 180; }
		if (gravity.y > 0 && g.x != 0) { rotation += 180; }
		gravity = g;
	}
}
