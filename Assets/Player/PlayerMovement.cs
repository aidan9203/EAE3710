using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public float speed;

	public Transform[] corners = new Transform[4];

	public GameObject drill;
	public bool drill_enable;
	Vector3 drill_speed = Vector3.zero;

	Vector3 gravity = Vector3.down;

	Transform tf;
	Rigidbody rb;

	float rotation = 0;
	float rotation_d = 0;
	float rotation_offset = 0;
	bool fixer_upside_down = false;

	bool gravity_change = false;

	public bool alive = true;

	Vector3 normal = Vector3.up;

	public List<string> keys = new List<string>();

	// Start is called before the first frame update
	void Start()
    {
		tf = GetComponent<Transform>();
		rb = GetComponent<Rigidbody>();
		rb.centerOfMass = new Vector3(0, -0.5f, 0);
		drill.SetActive(drill_enable);
    }

    // Update is called once per frame
    void Update()
    {
		if (!alive)
		{
			this.gameObject.SetActive(false);
		}

		//Drilling
		drill.SetActive(drill_enable);
		if (drill_enable)
		{
			drill.transform.localEulerAngles += drill_speed * Time.deltaTime;
			if (Input.GetAxisRaw("Attack") > 0)
			{
				drill_speed = Vector3.Lerp(drill_speed, new Vector3(0, 0, 300), 0.1f);
				drill.tag = "Drill";
			}
			else
			{
				drill_speed = Vector3.Lerp(drill_speed, Vector3.zero, 0.1f);
				drill.tag = "Untagged";
			}
		}

		float input_vertical = 0;
		float input_horizontal = 0;
		float input_rotate = 0;

		//Basic motion
		if (Input.GetKey(KeyCode.W)) { input_vertical += 1; }
		if (Input.GetKey(KeyCode.S)) { input_vertical -= 1; }
		//if (Input.GetKey(KeyCode.D)) { input_horizontal += 1; }
		//if (Input.GetKey(KeyCode.A)) { input_horizontal -= 1; }

		if (Input.GetKey(KeyCode.E)) { input_rotate += 1; }
		if (Input.GetKey(KeyCode.Q)) { input_rotate -= 1; }

		rotation += 180.0f * input_rotate * Time.deltaTime;

		float move_horizontal = input_horizontal * Mathf.Cos(Mathf.Deg2Rad * (rotation + rotation_offset)) + input_vertical * Mathf.Sin(Mathf.Deg2Rad * (rotation + rotation_offset));
		float move_vertical = input_vertical * Mathf.Cos(Mathf.Deg2Rad * (rotation + rotation_offset)) - input_horizontal * Mathf.Sin(Mathf.Deg2Rad * (rotation + rotation_offset));

		//Orient the player with gravity
		if (!gravity_change)
		{
			RaycastHit c0, c1, c2, c3;
			Physics.Raycast(corners[0].position, -tf.up, out c0, 1.0f, ~LayerMask.GetMask("Player"));
			Physics.Raycast(corners[1].position, -tf.up, out c1, 1.0f, ~LayerMask.GetMask("Player"));
			Physics.Raycast(corners[2].position, -tf.up, out c2, 1.0f, ~LayerMask.GetMask("Player"));
			Physics.Raycast(corners[3].position, -tf.up, out c3, 1.0f, ~LayerMask.GetMask("Player"));
			int hits = (c0.collider != null ? 1 : 0) + (c1.collider != null ? 1 : 0) + (c2.collider != null ? 1 : 0) + (c3.collider != null ? 1 : 0);
			normal += (c0.collider != null ? c0.normal : Vector3.zero);
			normal += (c1.collider != null ? c1.normal : Vector3.zero);
			normal += (c2.collider != null ? c2.normal : Vector3.zero);
			normal += (c3.collider != null ? c3.normal : Vector3.zero);
			if (hits > 0) { normal = normal.normalized; }
			else { normal = -gravity; }
		}
		
		tf.up = Vector3.Lerp(tf.up, normal, 0.05f);
		if (Mathf.Abs((tf.up + gravity).magnitude) < 0.2f) { gravity_change = false; }
		rotation_d = Vector2.Lerp(new Vector2(rotation_d, 0), new Vector2(rotation_offset, 0), 0.05f).x;
		if (fixer_upside_down)
		{
			if (tf.up.x - normal.x < 0.1f && tf.up.x - normal.x > -0.1f
				&& tf.up.y - normal.y < 0.1f && tf.up.y - normal.y > -0.1f
				&& tf.up.z - normal.z < 0.1f && tf.up.z - normal.z > -0.1f)
				{ fixer_upside_down = false; }
			tf.RotateAround(tf.position, tf.up, rotation);
		}
		else
		{
			tf.RotateAround(tf.position, tf.up, rotation + rotation_d);
		}

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
		fixer_upside_down = false;
		if (gravity.x > 0 && g.z > 0) { rotation_offset -= 90; }
		if (gravity.z > 0 && g.x > 0) { rotation_offset += 90; }

		if (gravity.x > 0 && g.z < 0) { rotation_offset += 90; }
		if (gravity.z > 0 && g.x < 0) { rotation_offset -= 90; }

		if (gravity.x < 0 && g.z > 0) { rotation_offset += 90; }
		if (gravity.z < 0 && g.x > 0) { rotation_offset -= 90; }

		if (gravity.x < 0 && g.z < 0) { rotation_offset -= 90; }
		if (gravity.z < 0 && g.x < 0) { rotation_offset += 90; }

		if (gravity.x != 0 && g.y > 0) { rotation_offset -= 180; fixer_upside_down = true; }
		if (gravity.y > 0 && g.x != 0) { rotation_offset += 180; fixer_upside_down = true; }
		gravity = g;
		normal = -g;
		gravity_change = true;
	}

	public Vector3 GetGravity()
	{
		return gravity;
	}


	public void OnCollisionEnter(Collision collision)
	{
		//Pick up the drill
		if (collision.gameObject.tag == "Drill")
		{
			drill_enable = true;
			GameObject.Destroy(collision.gameObject);
		}
	}
}
