/* README:
 * Controls camera movement
 * Set waypoints by placing waypoint prefabs called "Camera Waypoint(n)" where n > 0
 * Make sure there are no gaps in numbers or the waypoints after the gap will not be found
 * 
 * target is the object for the camera to focus on
 * speed is how fast (or how smoothly) the camera follows the target
 * distance is how far the camera tries to stay from the target (distance can also be set per waypoint)
 * loop toggles whether the camera will loop back to the first waypoint after reaching the last and vice versa
 * auto_waypoint toggles whether the camera will automatically find the first waypoint or start at #1
 * reverse_distance is the distance the player must travel towards the camera before it changes direction
 * 
 * If a waypoint is farther than distance from the player it will not transition to the next waypoint properly
 * because the start of the path to the next waypoint will be out of range
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public GameObject target;

	public float speed;
	public float distance;
	public bool loop;
	public bool auto_waypoint;
	public float reverse_distance;

	Transform tf;

	int num_waypoints = 0;
	public List<Transform> waypoints;
	int previous = 0;
	int next = 1;
	bool reversed = false;
	bool queue_reversal = false;
	Vector3 pos_reversed;

	Vector3 position_prev;
	Vector3 position_next;

    // Start is called before the first frame update
    void Start()
    {
		tf = GetComponent<Transform>();

		//Search for waypoints if they are not defined
		if (waypoints.Count == 0)
		{
			GameObject w = GameObject.Find("Camera Waypoint (1)");
			while (w != null)
			{
				num_waypoints++;
				waypoints.Add(w.transform);
				string name = "Camera Waypoint (" + (num_waypoints + 1) + ")";
				w = GameObject.Find(name);
			}
		}
		if (auto_waypoint) { ResetWaypoints(); }
		tf.position = FindPoint();
		position_prev = tf.position;
		position_next = tf.position;
	}

	// Update is called once per frame
	void Update()
    {
		//Draw debug lines
		for (int c = 0; c < num_waypoints - 1; c++)
		{
			Debug.DrawLine(waypoints[c].position, waypoints[c + 1].position, Color.blue, 0.1f);
		}
		if (loop)
		{
			Debug.DrawLine(waypoints[0].position, waypoints[num_waypoints - 1].position, Color.blue, 0.1f);
		}

		//Take the average of last two positions to help smooth camera
		position_prev = position_next;
		position_next = Vector3.Lerp(tf.position, FindPoint(), speed);
		tf.position = (position_next + position_prev) / 2.0f;

		//Apply rotation to camera
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation((target.transform.position - tf.position).normalized, target.transform.up), speed);
	}





	//Find the point along the waypoints that is closest but at least distance away from the player
	private Vector3 FindPoint()
	{
		if (num_waypoints == 1) { return waypoints[0].position; }
		UpdateDirection();
		Vector3 point;
		if (FindPointBetweenWaypoints(previous, next, out point))
		{
			return point;
		}
		else
		{
			//No point found between waypoints, either update waypoints or set position to appropriate extreme
			Vector3 player_pos = target.GetComponent<Transform>().position;
			float distance_previous = Mathf.Abs((waypoints[previous].position - player_pos).magnitude);
			float distance_next = Mathf.Abs((waypoints[next].position - player_pos).magnitude);

			Vector3 point_adjusted;
			if (distance_next <= distance_previous)
			{
				if (!reversed)
				{
					if (!loop && next == num_waypoints - 1) { return waypoints[next].position; }
					else if (FindPointBetweenWaypoints(previous + 1, next + 1, out point_adjusted)) { previous++; next++; point = point_adjusted; }
				}
				else
				{
					if (!loop && next == 0) { return waypoints[next].position; }
					else if (FindPointBetweenWaypoints(previous - 1, next - 1, out point_adjusted)) { previous--; next--; point = point_adjusted; }
				}
			}
			else
			{
				if (!reversed)
				{
					if (!loop && previous == 0) { return waypoints[previous].position; }
					else if (FindPointBetweenWaypoints(previous - 1, next - 1, out point_adjusted)) { previous--; next--; point = point_adjusted; }
				}
				else
				{
					if (!loop && previous == num_waypoints - 1) { return waypoints[previous].position; }
					else if (FindPointBetweenWaypoints(previous + 1, next + 1, out point_adjusted)) { previous++; next++; point = point_adjusted; }
				}
			}
			if (next < 0) { next += num_waypoints; }
			else if (next >= num_waypoints) { next -= num_waypoints; }
			if (previous < 0) { previous += num_waypoints; }
			else if (previous >= num_waypoints) { previous -= num_waypoints; }
			UpdateDirection();
			return point;
		}
	}

	/// <summary>
	/// Sets point to a point that lies between the two waypoints
	/// Returns true if this point is exactly distance away from the player
	/// Returns false if this point is greater than distance away from the player
	/// </summary>
	private bool FindPointBetweenWaypoints(int prev, int nex, out Vector3 point)
	{
		if (nex < 0) { nex += num_waypoints; }
		else if (nex >= num_waypoints) { nex -= num_waypoints; }
		if (prev < 0) { prev += num_waypoints; }
		else if (prev >= num_waypoints) { prev -= num_waypoints; }
		
		//Find point along waypoint that is distance from the target (Code from CS4600 Raytracing assignment)
		Vector3 player_pos = target.GetComponent<Transform>().position;
		Vector3 player_direction = player_pos - waypoints[prev].position;
		Vector3 waypoint_direction = Vector3.Normalize(waypoints[nex].position - waypoints[prev].position);
		float player_distance_waypoint = Vector3.Dot(player_direction, waypoint_direction);

		//Calculate camera distance from player
		float distance_prev = (player_pos - waypoints[prev].position).magnitude;
		float distance_next = (player_pos - waypoints[next].position).magnitude;
		float distance_frac_next = distance_prev / (distance_next + distance_prev);
		float distance_frac_prev = distance_next / (distance_next + distance_prev);
		float distance_avg = 0;
		if (waypoints[nex].GetComponent<CameraWaypoint>().distance > 0) { distance_avg += distance_frac_next * waypoints[nex].GetComponent<CameraWaypoint>().distance; }
		else { distance_avg += distance_frac_next * distance; }
		if (waypoints[prev].GetComponent<CameraWaypoint>().distance > 0) { distance_avg += distance_frac_prev * waypoints[prev].GetComponent<CameraWaypoint>().distance; }
		else { distance_avg += distance_frac_prev * distance; }

		if (player_distance_waypoint >= distance_avg)
		{
			float point_dist_squared = Vector3.Dot(player_direction, player_direction) - player_distance_waypoint * player_distance_waypoint;
			if (point_dist_squared <= (distance_avg * distance_avg))
			{
				float distance_along_line = Mathf.Sqrt(distance_avg * distance_avg - point_dist_squared);
				point = waypoints[prev].position + (player_distance_waypoint - distance_along_line) * waypoint_direction;
				return true;
			}
			else
			{
				//If closest point is farther than distance
				point = waypoints[prev].position + player_distance_waypoint * waypoint_direction;
				return false;
			}
		}
		else
		{
			//If closest point is closer than distance
			point = waypoints[prev].position;
			return false;
		}
	}


	/// <summary>
	/// Resets the waypoints to the two closest to the player
	/// </summary>
	private void ResetWaypoints()
	{
		if (num_waypoints <= 2) { return; }
		Vector3 player_pos = target.GetComponent<Transform>().position;
		int min_index = -1;
		float min_distance = float.MaxValue;
		for(int w = 1; w < num_waypoints - 1; w++)
		{
			float distance_current = Mathf.Abs((waypoints[w].position - player_pos).magnitude);
			if (distance_current < min_distance)
			{
				min_index = w;
				min_distance = distance_current;
			}
		}

		float distance_prev = Mathf.Abs((waypoints[min_index - 1].position - player_pos).magnitude);
		float distance_next = Mathf.Abs((waypoints[min_index + 1].position - player_pos).magnitude);
		if (distance_next <= distance_prev)
		{
			previous = min_index;
			next = min_index + 1;
		}
		else
		{
			previous = min_index - 1;
			next = min_index;
		}
		UpdateDirection();
	}


	/// <summary>
	/// Updates the camera direction to match the player's direction
	/// </summary>
	private void UpdateDirection()
	{
		Vector3 dir_player = -target.GetComponent<Transform>().forward;
		Vector3 dir_normal = Vector3.Normalize(waypoints[next].position - waypoints[previous].position) - dir_player;
		Vector3 dir_reversed = Vector3.Normalize(waypoints[previous].position - waypoints[next].position) - dir_player;		

		if (queue_reversal)
		{
			Vector3 player_diff = target.transform.position - pos_reversed;
			Vector3 dir_diff = dir_player - Vector3.Normalize(player_diff);

			if (Mathf.Abs(dir_reversed.magnitude) >= Mathf.Abs(dir_normal.magnitude))
			{
				queue_reversal = false;
			}
			else
			{
				if (dir_diff.magnitude > 1)
				{
					pos_reversed = target.transform.position;
				}
				else if (player_diff.magnitude > reverse_distance)
				{
					queue_reversal = false;
					Reverse();
				}
			}
		}
		else
		{
			pos_reversed = target.transform.position;
			if (Mathf.Abs(dir_reversed.magnitude) < Mathf.Abs(dir_normal.magnitude))
			{
				queue_reversal = true;
			}
		}
	}


	private void Reverse()
	{
		int i = previous;
		previous = next;
		next = i;
		reversed = !reversed;
	}
}
