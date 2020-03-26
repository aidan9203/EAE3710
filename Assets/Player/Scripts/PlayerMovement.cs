/* README:
 * Controls player movement, gravity, and drilling
 * camera_prefab is the camera that will be spawned
 * speed is the move speed
 * corners[] is the four corners used to calculate angle with the ground (empty gameObjects)
 * keys[] is the list of key codes the player has collected
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

	Animation walk_animation;
	AudioSource[] walk_sounds;
	float walk_timer = 0;

	Vector3 checkpoint_pos;
	Quaternion checkpoint_rot;
	Vector3 checkpoint_gravity;

	public string finish_level;

	// Start is called before the first frame update
	void Start()
	{
		tf = GetComponent<Transform>();
		rb = GetComponent<Rigidbody>();
		rb.centerOfMass = new Vector3(0, -0.5f, 0);
		drill.SetActive(drill_enable);
		cam = GameObject.Instantiate(camera_prefab, tf.position, Quaternion.Euler(Vector3.zero));
		cam.GetComponent<CameraFollow>().target = this.gameObject;
		walk_animation = GetComponent<Animation>();
		walk_sounds = GetComponents<AudioSource>();
		checkpoint_pos = transform.position;
		checkpoint_rot = transform.rotation;
		checkpoint_gravity = Vector3.down;
	}

	// Update is called once per frame
	void Update()
	{
		if (!alive)
		{
			alive = true;
			gravity = checkpoint_gravity;
			normal = -checkpoint_gravity;
			gravity_change = true;
			transform.position = checkpoint_pos - 0.1f * checkpoint_gravity;
			transform.rotation = checkpoint_rot;
			cam.GetComponent<CameraFollow>().ResetWaypoints();
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

		//Interpolate rotation
		walk_timer += Time.deltaTime;
		if (input_horizontal != 0 || input_vertical != 0)
		{
			walk_animation.Play();
			walk_animation["Walk Cycle"].speed = rb.velocity.magnitude * 0.5f;
			move_dir_current = move_dir;
			if (walk_timer > 0.3f)
			{
				//Play walk sound
				walk_timer = 0;
				int clip = Random.Range(0, walk_sounds.Length);
				walk_sounds[clip].pitch = Random.Range(0.8f, 1.2f);
				walk_sounds[clip].Play();
			}
		}
		else
		{
			walk_animation.Stop();
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

		tf.rotation = Quaternion.Lerp(tf.rotation, Quaternion.LookRotation(Vector3.Cross(normal, move_dir_current), normal), 0.2f);

		if (Mathf.Abs((tf.up + gravity).magnitude) < 0.2f) { gravity_change = false; }

		//Apply motion relative to orientation/gravity
		float vel_x = 100 * speed * move_dir.x * Time.deltaTime + (Mathf.Abs(rb.velocity.x) + 20 * Time.deltaTime) * gravity.x;
		float vel_y = 100 * speed * move_dir.y * Time.deltaTime + (Mathf.Abs(rb.velocity.y) + 20 * Time.deltaTime) * gravity.y;
		float vel_z = 100 * speed * move_dir.z * Time.deltaTime + (Mathf.Abs(rb.velocity.z) + 20 * Time.deltaTime) * gravity.z;
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
		else if (collision.gameObject.tag == "Finish")
		{
			SceneManager.LoadScene(finish_level);
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