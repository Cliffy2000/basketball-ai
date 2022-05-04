using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	// Player components
	private Rigidbody2D player_Rigidbody2D;
	private Animator player_Animator;

	// Other GameObjects
	public GameObject hand;
	public GameObject ball;
	private Rigidbody2D ball_Rigidbody2D;
	private Collider2D hand_Collider;

	// Player parameters
	public float runParam = 15f;
	public float jumpParam = 20f;
	public float shootParam = 0.13f;
	public float dribbleParam = 0.1f;
	public float inputX;
	private bool mouseButton0 = false;
	private Vector3 mouse;
	private bool reset = false;

	// Game Logic variables
	public bool holding = false;
	private bool shoot = false;
	public bool forceShoot = false;
	private bool layup = false;
	public bool jump = false;
	public bool grounded = false;
	public bool stopped = false;
	public bool dribbling = false;

	private float shootTime = 0f;
	private readonly float SHOOTTIME = 1.5f;
	private readonly float SHOOTTIMEMIN = 0.2f;
	private readonly float BASESHOOTTIME = 0.5f;
	private readonly float forceShootError = 0.3f;
	// stopped checks if the player is able to continue run around(dribblle)
	public float rotZ;


	private void Start() {
		player_Rigidbody2D = GetComponent<Rigidbody2D>();
		player_Animator = GetComponent<Animator>();
		ball_Rigidbody2D = ball.GetComponent<Rigidbody2D>();
		hand_Collider = hand.GetComponent<Collider2D>();
	}

	// Update is called once per frame
	// Input detection should be in Update()
	private void Update() {
		inputX = Input.GetAxisRaw("Horizontal");
		mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

		// Calculates the mouse facing and inverts sprite if it is opposite
		Vector3 difference = mouse - transform.position;
		rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
		bool faceRight = Mathf.Abs(rotZ) <= 90;

		Vector3 scale = transform.localScale;
		if (faceRight != (scale.x > 0)) {
			transform.localScale = new(scale.x * -1, scale.y, scale.z);
		}

		if (Input.GetButton("Jump")) {
			jump = true;
		}
	}


	private void FixedUpdate() {
		// Shooting sequence (normal shot, forced shot, layup)
		// forced shots have error scalers
		Vector3 difference = mouse - transform.position;
		Vector2 shootDirection = new(difference.x, difference.y);
		shootDirection.Normalize();

	}


	private void OnCollisionEnter2D(Collision2D collision) {
		if (collision.gameObject.CompareTag("Ground")) {
			grounded = true;
		}
	}

	private void OnCollisionExit2D(Collision2D collision) {
		if (collision.gameObject.CompareTag("Ground")) {
			grounded = false;
		}
	}
}
