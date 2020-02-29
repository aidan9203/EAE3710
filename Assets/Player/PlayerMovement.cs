using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public GameObject camera_prefab;
	GameObject cam;

	public float speed;

	public Transform[] corners = new Transform[4];

	public GameObject drill;
	public bool drill_enable;
	Vector3 drill_speed = Vector3.zero;

	Vector3 gravity = Vector3.down;

	Transform tf;
	Rigidbody rb;

	bool gravity_change = false;

	public bool alive = true;

	Vector3 normal = Vector3.up;
	float rotation = 0;

	public List<string> keys = new List<string>();

	// Start is called before the first frame update
	void Start()
    {
		tf = GetComponent<Transform>();
		rb = GetComponent<Rigidbody>();
		rb.centerOfMass = new Vector3(0, -0.5f, 0);
		drill.SetActive(drill_enable);
		cam = GameObject.Instantiate(camera_prefab, tf.position, Quaternion.Euler(Vector3.zero));
		cam.GetComponent<CameraFollow>().target = this.gameObject;
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

		float input_vertical = -Input.GetAxis("Vertical");
		float input_horizontal = Input.GetAxis("Horizontal");

		//Basic motion
		if (Input.GetKey(KeyCode.W)) { input_vertical += 1; }
		if (Input.GetKey(KeyCode.S)) { input_vertical -= 1; }
		if (Input.GetKey(KeyCode.D)) { input_horizontal += 1; }
		if (Input.GetKey(KeyCode.A)) { input_horizontal -= 1; }

		input_vertical = Mathf.Clamp(input_vertical, -1, 1);
		input_horizontal = Mathf.Clamp(input_horizontal, -1, 1);

		float input_rotate = input_rotate = cam.transform.eulerAngles.y;

		//Set and interpolate rotation
		if (input_horizontal != 0 || input_vertical != 0)
		{
			float rotation_goal = Mathf.Rad2Deg * Mathf.Atan2(-input_horizontal * Mathf.Cos(Mathf.Deg2Rad * input_rotate) - input_vertical * Mathf.Sin(Mathf.Deg2Rad * input_rotate),
				-input_vertical * Mathf.Cos(Mathf.Deg2Rad * input_rotate) + input_horizontal * Mathf.Sin(Mathf.Deg2Rad * input_rotate));
			if (Mathf.Abs(rotation - rotation_goal) <= 180)
			{
				if (rotation < rotation_goal)
				{
					rotation += 500 * Time.deltaTime;
					if (rotation > rotation_goal) { rotation = rotation_goal; }
				}
				else
				{
					rotation -= 500 * Time.deltaTime;
					if (rotation < rotation_goal) { rotation = rotation_goal; }
				}
			}
			else
			{
				if (rotation < rotation_goal)
				{
					rotation -= 500 * Time.deltaTime;
					if (rotation < -180) { rotation += 360; }
				}
				else
				{
					rotation += 500 * Time.deltaTime;
					if (rotation > 180) { rotation -= 360; }
				}
			}
		}

		float move_vertical = Mathf.Max(Mathf.Abs(input_horizontal), Mathf.Abs(input_vertical)) * Mathf.Cos(Mathf.Deg2Rad * rotation);
		float move_horizontal = Mathf.Max(Mathf.Abs(input_horizontal), Mathf.Abs(input_vertical)) * Mathf.Sin(Mathf.Deg2Rad * rotation);

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
		tf.RotateAround(tf.position, tf.up, rotation);
		
		if (Mathf.Abs((tf.up + gravity).magnitude) < 0.2f) { gravity_change = false; }

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
