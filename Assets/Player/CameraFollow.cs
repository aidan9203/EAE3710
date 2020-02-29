using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public GameObject target;

	public float speed;
	public float distance;
	public bool loop;
	public float reverse_distance;

	Transform tf;

	int num_waypoints = 0;
	public List<Vector3> waypoints;
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
				waypoints.Add(w.transform.position);
				string name = "Camera Waypoint (" + (num_waypoints + 1) + ")";
				w = GameObject.Find(name);
			}
		}

		ResetWaypoints();
		tf.position = FindPoint();
		position_prev = tf.position;
		position_next = tf.position;
    }

    // Update is called once per frame
    void Update()
    {
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
		if (num_waypoints == 1) { return waypoints[0]; }
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
			float distance_previous = Mathf.Abs((waypoints[previous] - player_pos).magnitude);
			float distance_next = Mathf.Abs((waypoints[next] - player_pos).magnitude);

			Vector3 point_adjusted;
			if (distance_next <= distance_previous)
			{
				if (!reversed)
				{
					if (!loop && next == num_waypoints - 1) { return waypoints[next]; }
					else if (FindPointBetweenWaypoints(previous + 1, next + 1, out point_adjusted)) { previous++; next++; point = point_adjusted; }
				}
				else
				{
					if (!loop && next == 0) { return waypoints[next]; }
					else if (FindPointBetweenWaypoints(previous - 1, next - 1, out point_adjusted)) { previous--; next--; point = point_adjusted; }
				}
			}
			else
			{
				if (!reversed)
				{
					if (!loop && previous == 0) { return waypoints[previous]; }
					else if (FindPointBetweenWaypoints(previous - 1, next - 1, out point_adjusted)) { previous--; next--; point = point_adjusted; }
				}
				else
				{
					if (!loop && previous == num_waypoints - 1) { return waypoints[previous]; }
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
		Vector3 player_direction = player_pos - waypoints[prev];
		Vector3 waypoint_direction = Vector3.Normalize(waypoints[nex] - waypoints[prev]);
		float player_distance_waypoint = Vector3.Dot(player_direction, waypoint_direction);

		if (player_distance_waypoint >= distance)
		{
			float point_dist_squared = Vector3.Dot(player_direction, player_direction) - player_distance_waypoint * player_distance_waypoint;
			if (point_dist_squared <= (distance * distance))
			{
				float distance_along_line = Mathf.Sqrt(distance * distance - point_dist_squared);
				point = waypoints[prev] + (player_distance_waypoint - distance_along_line) * waypoint_direction;
				return true;
			}
			else
			{
				//If closest point is farther than distance
				point = waypoints[prev] + player_distance_waypoint * waypoint_direction;
				return false;
			}
		}
		else
		{
			//If closest point is closer than distance
			point = waypoints[prev];
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
			float distance_current = Mathf.Abs((waypoints[w] - player_pos).magnitude);
			if (distance_current < min_distance)
			{
				min_index = w;
				min_distance = distance_current;
			}
		}

		float distance_prev = Mathf.Abs((waypoints[min_index - 1] - player_pos).magnitude);
		float distance_next = Mathf.Abs((waypoints[min_index + 1] - player_pos).magnitude);
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
		Vector3 dir_normal = Vector3.Normalize(waypoints[next] - waypoints[previous]) - dir_player;
		Vector3 dir_reversed = Vector3.Normalize(waypoints[previous] - waypoints[next]) - dir_player;		

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
