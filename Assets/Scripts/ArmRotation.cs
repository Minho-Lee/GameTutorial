using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmRotation : MonoBehaviour {

	public int rotationOffset = 0;
	// Update is called once per frame
	void Update () {
		// Subtracting position of player from the mouse position.
		Vector3 difference = Camera.main.ScreenToWorldPoint (Input.mousePosition) - this.transform.position;
		difference.Normalize ();		// The sum of the vector will be equal to 1.

		float rotZ = Mathf.Atan2 (difference.y, difference.x) * Mathf.Rad2Deg;	// Find the angle in degrees
		transform.rotation = Quaternion.Euler(0f, 0f, rotZ + rotationOffset);
		//Debug.Log (rotZ);
	}
}
