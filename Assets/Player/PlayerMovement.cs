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
		RaycastHit h0, h1, h2, h3;
		Physics.Raycast(tf.position + new Vector3(0.75f, -0.5f, 0.35f), gravity, out h0, 1.0f, ~LayerMask.GetMask("Ignore Raycast"));
		Physics.Raycast(tf.position + new Vector3(0.75f, -0.5f, -0.35f), gravity, out h1, 1.0f, ~LayerMask.GetMask("Ignore Raycast"));
		Physics.Raycast(tf.position + new Vector3(-0.75f, -0.5f, 0.35f), gravity, out h2, 1.0f, ~LayerMask.GetMask("Ignore Raycast"));
		Physics.Raycast(tf.position + new Vector3(-0.75f, -0.5f, -0.35f), gravity, out h3, 1.0f, ~LayerMask.GetMask("Ignore Raycast"));
		int hits = (h0.collider != null ? 1 : 0) + (h1.collider != null ? 1 : 0) + (h2.collider != null ? 1 : 0) + (h3.collider != null ? 1 : 0);
		normal += (h0.collider != null ? h0.normal : Vector3.zero);
		normal += (h1.collider != null ? h1.normal : Vector3.zero);
		normal += (h2.collider != null ? h2.normal : Vector3.zero);
		normal += (h3.collider != null ? h3.normal : Vector3.zero);
		if (hits > 0) { normal /= hits; }
		else { normal = -gravity; }

		tf.up = Vector3.Lerp(tf.up, normal, 0.01f);
		rotation_d = Vector2.Lerp(new Vector2(rotation_d, 0), new Vector2(rotation_offset, 0), 0.05f).x;
		tf.RotateAround(tf.position, tf.up, rotation + rotation_d);

		//Apply motion relative to orientation/gravity
		float vel_x = speed * (-Mathf.Abs(tf.up.y) * move_horizontal - Mathf.Abs(tf.up.z) * move_horizontal) + (Mathf.Abs(rb.velocity.x) + 9.81f * Time.deltaTime) * gravity.x;
		float vel_y = speed * (tf.up.x * move_horizontal + tf.up.z * move_vertical) + (Mathf.Abs(rb.velocity.y) + 9.81f * Time.deltaTime) * gravity.y;
		float vel_z = speed * (-tf.up.y * move_vertical - Mathf.Abs(tf.up.x) * move_vertical) + (Mathf.Abs(rb.velocity.z) + 9.81f * Time.deltaTime) * gravity.z;

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
	}
}
