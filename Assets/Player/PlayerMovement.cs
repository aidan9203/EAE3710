using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public float speed;

	public Transform[] corners = new Transform[4];

	Vector3 gravity = Vector3.down;

	Transform tf;
	Rigidbody rb;

	float rotation = 0;
	float rotation_d = 0;
	float rotation_offset = 0;

	bool gravity_change = false;

	Vector3 normal = Vector3.up;

	// Start is called before the first frame update
	void Start()
    {
		tf = GetComponent<Transform>();
		rb = GetComponent<Rigidbody>();
		rb.centerOfMass = new Vector3(0, -0.5f, 0);
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
		if (!gravity_change)
		{
			RaycastHit c0, c1, c2, c3;
			Physics.Raycast(corners[0].position, -tf.up, out c0, 1.0f, ~LayerMask.GetMask("Ignore Raycast"));
			Physics.Raycast(corners[1].position, -tf.up, out c1, 1.0f, ~LayerMask.GetMask("Ignore Raycast"));
			Physics.Raycast(corners[2].position, -tf.up, out c2, 1.0f, ~LayerMask.GetMask("Ignore Raycast"));
			Physics.Raycast(corners[3].position, -tf.up, out c3, 1.0f, ~LayerMask.GetMask("Ignore Raycast"));
			int hits = (c0.collider != null ? 1 : 0) + (c1.collider != null ? 1 : 0) + (c2.collider != null ? 1 : 0) + (c3.collider != null ? 1 : 0);
			normal += (c0.collider != null ? c0.normal : Vector3.zero);
			normal += (c1.collider != null ? c1.normal : Vector3.zero);
			normal += (c2.collider != null ? c2.normal : Vector3.zero);
			normal += (c3.collider != null ? c3.normal : Vector3.zero);
			if (hits > 0) { normal /= hits; }
			else { normal = -gravity; }
		}
		
		tf.up = Vector3.Lerp(tf.up, normal, 0.05f);
		if (Mathf.Abs((tf.up + gravity).magnitude) < 0.2f) { gravity_change = false; }
		rotation_d = Vector2.Lerp(new Vector2(rotation_d, 0), new Vector2(rotation_offset, 0), 0.05f).x;
		tf.RotateAround(tf.position, tf.up, rotation + rotation_d);

		//Apply motion relative to orientation/gravity
		float vel_x = speed * (-Mathf.Abs(tf.up.y) * move_horizontal - Mathf.Abs(tf.up.z) * move_horizontal) + (Mathf.Abs(rb.velocity.x) + 20 * Time.deltaTime) * gravity.x;
		float vel_y = speed * (tf.up.x * move_horizontal + tf.up.z * move_vertical) + (Mathf.Abs(rb.velocity.y) + 20 * Time.deltaTime) * gravity.y;
		float vel_z = speed * (-tf.up.y * move_vertical - Mathf.Abs(tf.up.x) * move_vertical) + (Mathf.Abs(rb.velocity.z) + 20 * Time.deltaTime) * gravity.z;

		rb.velocity = new Vector3(vel_x, vel_y, vel_z);
	}


	/// <summary>
	/// Sets the direction of the player's gravity
	/// </summary>
	public void ChangeGravity(Vector3 g)
	{
		g.Normalize();
		if (gravity.Equals(g)) { return; }
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
		normal = -g;
		gravity_change = true;
	}
}
