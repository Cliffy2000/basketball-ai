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
    public bool jump = false;      // waiting on jump action to go through
    public bool grounded = false;  // shows if the player is on the ground
    public bool stopped = false;   // shows if the player can dribble
    public bool dribbling = false;  // indicator to show if the player is dribbling

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

        if (Input.GetButtonDown("Jump") && grounded && !dribbling) {
            // jump if on the ground and not dribbling
            jump = true;
        }

        if (Mathf.Abs(player_Rigidbody2D.velocity.x) >= 0.01 && holding && !stopped && shootTime ==0f) {
            holding = false;
            dribbling = true;
            ball_Rigidbody2D.AddForce(new(0, -dribbleParam));
        }

        // Checks mouse clicks
        if (Input.GetMouseButtonDown(0) && (holding || dribbling)) {
            shootTime = Time.time;
            mouseButton0 = true;
        }

        if (Input.GetMouseButtonUp(0)) {
            Debug.Log("Mouse up");
            if (shootTime != 0) shootTime = Time.time - shootTime;
            Debug.Log(shootTime); 
            // check click time length
            if (shootTime < SHOOTTIMEMIN && shootTime != 0) {
                // click is switch from dribbling to stop and hold
                dribbling = false;
                holding = true;
                stopped = true;
                hand_Collider.isTrigger = true;
                shootTime = 0f;
            } else if (shootTime > SHOOTTIMEMIN) {
                holding = false;
                shoot = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            // resets player properties
            reset = true;
            holding = false;
            dribbling = false;
            stopped = false;
            shoot = false;
            shootTime = 0;
            forceShoot = false;
            layup = false;
            jump = false;
        }
    }


    private void FixedUpdate() {
        // Shooting sequence (normal shot, forced shot, layup)
        // forced shots have error scalers
        Vector3 difference = mouse - transform.position;
        Vector2 shootDirection = new(difference.x, difference.y);
        shootDirection.Normalize();

        if (shoot) {
            holding = false;
            stopped = false;
            if (forceShoot) {
                // TODO: change force parameter or set ball velocity
                shootDirection = new Vector2(shootDirection.x * (1 + Random.Range(-forceShootError, forceShootError)),
                    shootDirection.y * (1 + Random.Range(-forceShootError, forceShootError)));
                shootDirection.Normalize();
                shootTime = Random.Range(2 * SHOOTTIME, 3 * SHOOTTIME);
            } else {
                shootTime = Mathf.Min(shootTime, SHOOTTIME) + BASESHOOTTIME;
            }
            Vector2 shootForce = new(shootTime * shootParam * shootDirection.x, shootTime * shootParam * shootDirection.y);
            ball_Rigidbody2D.AddForce(shootForce, ForceMode2D.Impulse);
            shootTime = 0;
            shoot = false;
            forceShoot = false;
        }

        Vector2 jumpForce = new(0f, 0f);
        if (jump && grounded) {
            jump = false;
            grounded = false;
            jumpForce = new(jumpParam * inputX / 2, jumpParam);
            // Jump actions
            if (dribbling) {
                holding = true;
                dribbling = false;
            } else if (holding) {
                forceShoot = true;
            }
        }

        if (!stopped && grounded) {
            player_Rigidbody2D.velocity = new(runParam * inputX, player_Rigidbody2D.velocity.y);
        } else if (stopped && grounded) {
            player_Rigidbody2D.velocity = new(0, 0);
        }
        
        player_Rigidbody2D.AddForce(jumpForce, ForceMode2D.Impulse);

        if (reset) {
            reset = false;
            player_Rigidbody2D.velocity = new(0, 0);
            player_Rigidbody2D.position = new(15, -5);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision) {
        grounded = true;
        if (shoot || layup || stopped) {
            shoot = true;
            forceShoot = true;
        }
        if (forceShoot == true) {
            shoot = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {

    }
}
