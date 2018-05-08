using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]

public class Tiling : MonoBehaviour {
	public int offsetX = 2;					//offset to avoid errors

	public bool hasRightBuddy = false;		//Used for checking for instantiating
	public bool hasLeftBuddy = false;		//Used for checking for instantiating

	public bool reverseScale = false;		//Used if the object is not tilable

	private float spriteWidth = 0f;			//The width of our element
	private Camera cam;
	private Transform myTransform;

	void Awake () {
		cam = Camera.main;
		myTransform = this.transform;
		//Debug.Log (myTransform.position);
	}

	// Use this for initialization
	void Start () {
		SpriteRenderer sRenderer = this.GetComponent<SpriteRenderer> ();
		spriteWidth = sRenderer.sprite.bounds.size.x;
	}
	
	// Update is called once per frame
	void Update () {
		//Does it need buddies? If not, do nothing.
		if (hasLeftBuddy == false || hasRightBuddy == false) {
			//calculate camera's extent (half the width) of what the camera can see in world's coordinates
			float camHorizantalExtent = cam.orthographicSize * Screen.width / Screen.height;

			//calculate the x pos where camera can see the edge of the sprite (element)
			float edgeVisiblePosRight = (myTransform.position.x + spriteWidth/2) - camHorizantalExtent;
			float edgeVisiblePosLeft = (myTransform.position.x - spriteWidth / 2) + camHorizantalExtent;

			//Checking to see if we can see the edges of the element and making new buddies if yes
			if (cam.transform.position.x >= edgeVisiblePosRight - offsetX && hasRightBuddy == false) {
				MakeNewBuddy (1);
				hasRightBuddy = true;
			} else if (cam.transform.position.x <= edgeVisiblePosLeft + offsetX && hasLeftBuddy == false) {
				MakeNewBuddy (-1);
				hasLeftBuddy = true;
			}
		}
	}

	void MakeNewBuddy (int rightOrLeft){
		//Calculating new position for our new buddy
		Vector3 newPos = new Vector3 (myTransform.position.x + spriteWidth * rightOrLeft, 
			myTransform.position.y, myTransform.position.z);
		//Instantiating a newBuddy and casting into 'Transform' class
		Transform newBuddy = Instantiate (myTransform, newPos, myTransform.rotation) as Transform;

		//if not tilable, then we reverse the x size to get rid of ugly seams in borders
		if (reverseScale == true) {
			newBuddy.localScale = new Vector3 (newBuddy.localScale.x * -1, newBuddy.localScale.y, newBuddy.localScale.z);
		}

		newBuddy.parent = myTransform.parent;
		if (rightOrLeft > 0) {
			newBuddy.GetComponent<Tiling> ().hasLeftBuddy = true;
		} else {
			newBuddy.GetComponent<Tiling> ().hasRightBuddy = true;
		}
	}
}
