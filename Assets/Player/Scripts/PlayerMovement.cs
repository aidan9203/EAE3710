/* README:
 * Controls player movement, gravity, and drilling
 * camera_prefab is the camera that will be spawned
 * speed is the move speed
 * corners[] is the four corners used to calculate angle with the ground (empty gameObjects)
 * keys[] is the list of key codes the player has collected
 */

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

	public List<string> keys = new List<string>();

	string animation_name = "idle";
	Animation animations;
	AudioSource[] sounds;
	float sound_timer = 0;
	float animation_timer = 0;

	Vector3 checkpoint_pos;
	Quaternion checkpoint_rot;
	Vector3 checkpoint_gravity;

	public GameObject prefab_dead_player;

	public bool frozen;

	// Start is called before the first frame update
	void Start()
	{
		tf = GetComponent<Transform>();
		rb = GetComponent<Rigidbody>();
		rb.centerOfMass = new Vector3(0, -0.5f, 0);
		drill.SetActive(drill_enable);
		cam = GameObject.Instantiate(camera_prefab, tf.position, Quaternion.Euler(Vector3.zero));
		//cam.GetComponent<CameraFollow>().target = this.gameObject;
		cam.GetComponent<CameraControlled>().target = this.gameObject;
		animations = GetComponent<Animation>();
		sounds = GetComponents<AudioSource>();
		checkpoint_pos = transform.position;
		checkpoint_rot = transform.rotation;
		checkpoint_gravity = Vector3.down;

		animations["Fast_-90"].speed = 2.0f;
		animations["Fast_+90"].speed = 2.0f;

		Cursor.visible = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (!alive)
		{
			GameObject dead_body = GameObject.Instantiate(prefab_dead_player, transform.position, transform.rotation);
			dead_body.GetComponent<DeadPlayer>().gravity = gravity;
			alive = true;
			gravity = checkpoint_gravity;
			normal = -checkpoint_gravity;
			gravity_change = true;
			transform.position = checkpoint_pos - 0.1f * checkpoint_gravity;
			transform.rotation = checkpoint_rot;
			//cam.GetComponent<CameraFollow>().ResetWaypoints();
		}

		//Drilling
		drill.SetActive(drill_enable);
		if (drill_enable)
		{
			drill.transform.localEulerAngles += drill_speed * Time.deltaTime;
			if (Input.GetAxisRaw("Attack") > 0)
			{
				if (!Battery.drains.ContainsKey("drill")) { Battery.drains.Add("drill", 2); }
				if (Battery.charge > 0)
				{
					drill_speed = Vector3.Lerp(drill_speed, new Vector3(0, 0, 300), 10 * Time.deltaTime);
					drill.tag = "Drill";
					sounds[3].Play();
				}
				else
				{
					drill_speed = Vector3.Lerp(drill_speed, Vector3.zero, 10 * Time.deltaTime);
					drill.tag = "Untagged";
					sounds[3].Stop();
				}
			}
			else
			{
				if (Battery.drains.ContainsKey("drill")) { Battery.drains.Remove("drill"); }
				drill_speed = Vector3.Lerp(drill_speed, Vector3.zero, 10 * Time.deltaTime);
				drill.tag = "Untagged";
				sounds[3].Stop();
			}
		}

		float input_vertical = -Input.GetAxis("Vertical");
		float input_horizontal = Input.GetAxis("Horizontal");

		//Basic motion
		if (Input.GetKey(KeyCode.W)) { input_vertical += 1; }
		if (Input.GetKey(KeyCode.S)) { input_vertical -= 1; }
		if (Input.GetKey(KeyCode.D)) { input_horizontal += 1; }
		if (Input.GetKey(KeyCode.A)) { input_horizontal -= 1; }

		if (frozen) { input_horizontal = 0; input_vertical = 0; }
		input_vertical = Mathf.Clamp(input_vertical, -1, 1);
		input_horizontal = Mathf.Clamp(input_horizontal, -1, 1);

		//Calculate the forward direction based on the camera's direction
		Vector3 move_dir_current = -transform.right;
		Vector3 move_dir_forward = cam.transform.forward;
		Vector3 move_dir_right = cam.transform.right;
		if (gravity.x != 0) { move_dir_forward.x = 0; move_dir_right.x = 0; move_dir_current.x = 0; }
		else if (gravity.y != 0) { move_dir_forward.y = 0; move_dir_right.y = 0; move_dir_current.y = 0; }
		else if (gravity.z != 0) { move_dir_forward.z = 0; move_dir_right.z = 0; move_dir_current.z = 0; }
		move_dir_forward = move_dir_forward.normalized;
		move_dir_right = move_dir_right.normalized;
		move_dir_current = move_dir_current.normalized;

		Vector3 move_dir = (move_dir_forward * input_vertical + move_dir_right * input_horizontal).normalized;
		if (input_horizontal != 0 || input_vertical != 0)
		{
			move_dir_current = move_dir;
		}

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

		tf.rotation = Quaternion.Lerp(tf.rotation, Quaternion.LookRotation(Vector3.Cross(normal, move_dir_current), normal), 0.1f);

		if (Mathf.Abs((tf.up + gravity).magnitude) < 0.2f) { gravity_change = false; }

		//Apply motion relative to orientation/gravity
		float vel_x = speed * move_dir.x + (Mathf.Abs(rb.velocity.x) + 20 * Time.deltaTime) * gravity.x;
		float vel_y = speed * move_dir.y + (Mathf.Abs(rb.velocity.y) + 20 * Time.deltaTime) * gravity.y;
		float vel_z = speed * move_dir.z + (Mathf.Abs(rb.velocity.z) + 20 * Time.deltaTime) * gravity.z;
		rb.velocity = new Vector3(vel_x, vel_y, vel_z);

		//Animation and sound
		sound_timer += Time.deltaTime;
		animation_timer -= Time.deltaTime;
		if (animation_timer <= 0)
		{
			//Turning animations
			if ((transform.forward - move_dir_current).magnitude < 1.2f)
			{
				animation_name = "Hold_RightTurn";
			}
			else if ((transform.forward - move_dir_current).magnitude > 1.6f)
			{
				animation_name = "Hold_LeftTurn";
			}
			//Walking animation
			else if (input_horizontal != 0 || input_vertical != 0)
			{
				animation_name = "WalkCycle";
				animations["WalkCycle"].speed = rb.velocity.magnitude * 0.5f;
				if (sound_timer > 0.3f)
				{
					//Play walk sound
					sound_timer = 0;
					int clip = Random.Range(0, 3);
					sounds[clip].pitch = Random.Range(0.8f, 1.2f);
					sounds[clip].Play();
				}
			}
			//Idle animation
			else
			{
				animation_name = "idle";
			}
		}
		animations.Play(animation_name);
	}


	/// <summary>
	/// Sets the direction of the player's gravity
	/// </summary>
	public void ChangeGravity(Vector3 g)
	{
		g.Normalize();
		if (gravity.Equals(g)) { return; }

		//Gravity change animation
		if (Vector3.SignedAngle(transform.forward, g, transform.right) > 0) { animation_name = "Fast_+90"; }
		else { animation_name = "Fast_-90"; }
		animation_timer = 0.7f;

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

	public void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Checkpoint")
		{
			checkpoint_pos = transform.position;
			checkpoint_rot = transform.rotation;
			checkpoint_gravity = gravity;
		}
	}
}