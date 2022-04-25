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
    public float shootParam = 0.16f;
    public float dribbleParam = 5f;
    public float inputX;
    private bool mouseButton0 = false;
    private Vector3 mouse;

    // Game Logic variables
    public bool holding = false;
    private bool shoot = false;
    private bool forceShoot = false;
    private bool layup = false;
    public bool jump = false;      // waiting on jump action to go through
    public bool grounded = false;  // shows if the player is on the ground
    public bool stopped = false;   // shows if the player can dribble
    public bool dribbling = false;  // indicator to show if the player is dribbling

    private float shootTime = 0f;
    private readonly float SHOOTTIME = 1.5f;
    private readonly float SHOOTTIMEMIN = 0.1f;
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

        if (Input.GetButtonDown("Jump") && grounded) {
            jump = true;
        }


        // Checks mouse clicks
        if (Input.GetMouseButtonDown(0) && (holding || dribbling)) {
            shootTime = Time.time;
            mouseButton0 = true;
        }

        if (Input.GetMouseButtonUp(0)) {
            if (shootTime != 0) shootTime = Time.time - shootTime;
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
    }


    private void FixedUpdate() {
        // Shooting sequence (normal shot, forced shot, layup)
        // forced shots have error scalers
        Vector3 difference = mouse - transform.position;
        Vector2 shootDirection = new(difference.x, difference.y);
        shootDirection.Normalize();

        if (shoot) {
            if (forceShoot) {
                if (shootTime == 0f) shootTime = SHOOTTIMEMIN;
                // TODO: change force parameter or set ball velocity
                shootTime = Random.Range(Mathf.Min(shootTime, SHOOTTIME), SHOOTTIME);
                shootDirection = new Vector2(shootDirection.x * (1 + Random.Range(-forceShootError, forceShootError)),
                    shootDirection.y * (1 + Random.Range(-forceShootError, forceShootError)));
                shootDirection.Normalize();
                shootTime = 3 * (SHOOTTIME + BASESHOOTTIME) - Random.Range(-SHOOTTIME, -shootTime);
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
            }
        }

        if (!stopped && grounded) {
            player_Rigidbody2D.velocity = new(runParam * inputX, player_Rigidbody2D.velocity.y);
        } else if (stopped && grounded) {
            player_Rigidbody2D.velocity = new(0, 0);
        }
        
        player_Rigidbody2D.AddForce(jumpForce, ForceMode2D.Impulse);
    }


    private void OnCollisionEnter2D(Collision2D collision) {
        grounded = true;
        if (shoot || layup || stopped) {
            forceShoot = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {

    }
}
