using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArmControl : MonoBehaviour {
	// Update is called once per frame
	public int rotationOffset = 90;

	public GameObject body;
	private PlayerController player_Script;

	private void Start() {
		player_Script = body.GetComponent<PlayerController>();
	}

	// Update is called once per frame
	private void Update() {
		// subtracting the position of the player from the mouse position
		Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
		difference.Normalize();     // normalizing the vector. Meaning that all the sum of the vector will be equal to 1

		float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;   // find the angle in degrees

		if (player_Script.dribbling) {
			// rotz is between 0 and -180 if the arm is lower half
			if (rotZ > 0) {
				if (rotZ > 90) rotZ = 179.99f;
				else rotZ = 0.01f;
			}
		}

		transform.rotation = Quaternion.Euler(0f, 0f, rotZ + rotationOffset);
	}
}
