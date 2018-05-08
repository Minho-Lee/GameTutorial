using UnityEngine;
using System.Collections;
using Pathfinding;

[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (Seeker))]

public class EnemyAI : MonoBehaviour {

	// What to chase?
	public Transform target;

	// How many times we will update our path each second
	public float updateRate = 2f;

	// Caching
	private Seeker seeker;
	private Rigidbody2D rb;

	// The calculated path
	public Path path;

	// AI's speed per second
	public float speed = 300f;
	public ForceMode2D fMode;

	[HideInInspector]
	public bool pathIsEnded = false;

	// The max distance from AI to waypoint for it to continue to the next waypoint
	public float nextWaypointDist = 3f;

	// Waypoint we are currently moving towards (index)
	private int currentWaypoint = 0;

	private bool searchingForPlayer = false;

	void Start () { 
		seeker = GetComponent<Seeker> ();
		rb = GetComponent<Rigidbody2D> ();
		 
		if (target == null) {
			if (!searchingForPlayer) {
				searchingForPlayer = true;
				StartCoroutine (SearchForPlayer ());
			}
			return;
		}
		// Start a new path position to the target position, return the result to OnPathComplete method
		seeker.StartPath (transform.position, target.position, OnPathComplete);

		StartCoroutine (UpdatePath ());
	}

	IEnumerator SearchForPlayer () {
		// Don't set 'target' directly since it may not exists resulting in 'null'
		GameObject sResult = GameObject.FindGameObjectWithTag ("Player");

		if (sResult == null) {
			// Search again twice every second
			yield return new WaitForSeconds (0.5f);
			StartCoroutine (SearchForPlayer ());
		} else {
			//Debug.Log ("Search Result : " + sResult.name);
			searchingForPlayer = false;
			target = sResult.transform;
			StartCoroutine (UpdatePath ());
			yield return false;
		}
	}

	IEnumerator UpdatePath () {
		if (target == null) {
			if (!searchingForPlayer) {
				searchingForPlayer = true;
				StartCoroutine (SearchForPlayer ());
			}
			yield return false;
		}
		if (target != null) {
			// Start a new path position to the target position, return the result to OnPathComplete method
			seeker.StartPath (transform.position, target.position, OnPathComplete);

			yield return new WaitForSeconds (1f / updateRate);
			// Calls itself after a delay
			StartCoroutine (UpdatePath ());
		}
	}

	public void OnPathComplete (Path p) {
		Debug.Log ("We got a path. Did we get an error? " + p.error);
		if (!p.error) {
			path = p;
			currentWaypoint = 0;
		}
	}

	// If you want to move something using PHYSICS, use FixedUpdate() NOT Update()
	void FixedUpdate () {
		if (target == null) {
			if (!searchingForPlayer) {
				searchingForPlayer = true;
				StartCoroutine (SearchForPlayer ());
			}
			return;
		}
		// TODO: Always look at player (such as missiles)

		if (path == null) {
			return;
		}

		if (currentWaypoint >= path.vectorPath.Count) {
			if (pathIsEnded) 
				return;
			
			Debug.Log ("End of path reached!");
			pathIsEnded = true;
		}
		pathIsEnded = false;

		// Direction to the next waypoint
		Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized; // Direction vector
		dir *= speed * Time.fixedDeltaTime;

		// Move the AI
		rb.AddForce(dir, fMode);

		float dist = Vector3.Distance (transform.position, path.vectorPath [currentWaypoint]);
		if (dist < nextWaypointDist) {
			currentWaypoint++;
			return;
		}
	}
}
