using UnityEngine;

public class PlayerController : MonoBehaviour {
	// Player components
	private Rigidbody2D player_Rigidbody2D;
	private Animator player_Animator;

	// Other GameObjects
	public GameObject hand;
	public GameObject ball;
	private Rigidbody2D ball_Rigidbody2D;
	private Collider2D hand_Collider;

	// Player parameters
	private float runParam = 18f;
	private float jumpParam = 18f;
	private float shootParam = 0.5f;
	public float dribbleParam = 0.25f;
	private float inputX;
	private Vector3 mouse;

	// Game Logic variables
	private bool jump = false;
	private bool grounded = false;
	public bool dribbling = false;
	public bool stopped = false;
	public bool holding = false;
	private bool shoot = false;
	private bool layup = false;
	private bool forceShoot = false;
	private bool allowShoot = true;

	private float shootStart;
	private float shootTime = 0f;
	private float minShootTime = 0.2f;
	private float maxShootTime = 1.2f;
	private float maxLayupTime = 1f;
	private float layupTime = 0f;

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

		if (Input.GetKeyDown(KeyCode.R)) {
			player_Rigidbody2D.position = new Vector2(15, -5);
			player_Rigidbody2D.velocity = new Vector2(0, 0);

			grounded = false;
			jump = false;
			dribbling = false;
			layup = false;
			stopped = false;
			forceShoot = false;
			hand_Collider.isTrigger = true;
		}

		if (Input.GetButtonDown("Jump")) {
			// cannot jump if !grounded/dribbling
			jump = true;
		}

		if (Input.GetMouseButtonDown(0)) {
			shootStart = Time.time;
		}
		
		if (Input.GetMouseButton(0)) {
			shootTime = Time.time - shootStart;
			if (dribbling && shootTime >= minShootTime) {
				layup = true;
				holding = true;
				dribbling = false;
			}
        }


		if (Input.GetMouseButtonUp(0)) {
			// switch to stop and hold
			shootTime = Time.time - shootStart;
			if (shootTime <= minShootTime && (holding || dribbling)) {
				holding = true;
				stopped = true;
				dribbling = false;
			}
		}

		if (allowShoot) {
			if ((Input.GetMouseButtonUp(0) && shootTime > minShootTime) || (layup && shootTime >= maxLayupTime) || shootTime >= maxShootTime) {
				shoot = true;
				if (!(Input.GetMouseButtonUp(0) && shootTime > minShootTime)) {
					forceShoot = true;
				}
			}
		}

		if (holding) {
			hand_Collider.isTrigger = true;
        }
		if (Input.GetMouseButtonUp(0)) {
			allowShoot = true;
        }
	}
	 

	private void FixedUpdate() {
		// Shooting sequence (normal shot, forced shot, layup)
		// forced shots have error scalers
		Vector3 difference = mouse - transform.position;
		Vector2 shootDirection = new(difference.x, difference.y);
		shootDirection.Normalize();
		
		if (jump) {
			jump = false;
			if (grounded && !dribbling) {
				Vector2 jumpForce = new Vector2(0, jumpParam);
				if (stopped) {
					jumpForce.x = inputX * runParam / 1.5f;
				}
				grounded = false;
				player_Rigidbody2D.AddForce(jumpForce, ForceMode2D.Impulse);
			}
		}

		if (inputX != 0) {
			if (grounded) {
				float vX = inputX * runParam;
				if (stopped) {
					vX = 0f;
                }
				player_Rigidbody2D.velocity = new Vector2(vX, player_Rigidbody2D.velocity.y);
            }
		}

		if (holding & shoot) {
			if (shootTime > 1.5f) {
				Debug.Log("shoot force error");
            }
			if (forceShoot && Input.GetMouseButton(0)) {
				allowShoot = false;
            }
			shoot = false;
			holding = false;
			shoot = false;
			stopped = false;
			Vector2 shootForce = shootTime * shootParam * shootDirection;
			ball_Rigidbody2D.AddForce(shootForce, ForceMode2D.Impulse);
			shootTime = -1f;
        }
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
