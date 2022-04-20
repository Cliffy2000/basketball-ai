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
    public float runParam = 12f;
    public float jumpParam = 15f;
    public float shootParam = 10f;
    public float dribbleParam = 4f;
    private float inputX;

    // Game Logic variables
    private bool grounded = true;
    public bool holding = false;
    private float shootTime = 0f;
    private readonly float SHOOTTIME = 1.5f;
    private readonly float SHOOTTIMEMIN = 0.1f;
    private readonly float BASESHOOTTIME = 0.5f;
    private float layupTime = 0f;
    private readonly float LAYUPTIME = 0.5f;
    public bool forceShoot = false;
    private readonly float forceShootError = 0.25f;
    public bool dribbling = false;
    public bool stopped = false;
    public float rotZ;


    private void Start() {
        player_Rigidbody2D = GetComponent<Rigidbody2D>();
        player_Animator = GetComponent<Animator>();
        ball_Rigidbody2D = ball.GetComponent<Rigidbody2D>();
        hand_Collider = hand.GetComponent<Collider2D>();
    }

    // Update is called once per frame
    private void Update() {
        Debug.Log("Update");
        inputX = Input.GetAxisRaw("Horizontal");

        // Flips player facing according to mouse location
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 difference = mouse - transform.position;
        rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        bool faceRight = Mathf.Abs(rotZ) <= 90;

        Vector3 scale = transform.localScale;
        // inverts the sprite if the facing is opposite to mouse direction
        if ((faceRight && scale.x < 0) || (!faceRight && scale.x > 0)) {
            transform.localScale = new Vector3(scale.x * -1, scale.y, scale.z);
        }


        // =====================

        if (Mathf.Abs(player_Rigidbody2D.velocity.x) >= 0.01 && holding && grounded && !stopped && shootTime==0f) {
            Debug.Log("start dribble trigger" + grounded+" "+transform.position.y);
            // Start dribbling if the player is on the ground with the ball
            holding = false;
            dribbling = true;
            ball_Rigidbody2D.AddForce(new(0, -dribbleParam));
        }

        // =====================

        if (Input.GetMouseButtonDown(0) && (holding||dribbling)) {
            shootTime = Time.time;
        }
        
        if (Input.GetMouseButtonUp(0)) {
            float timeDiff = Time.time - shootTime;
            // Mouse down is click
            if (timeDiff < SHOOTTIMEMIN && shootTime != 0f) {
                // Stand still but not shoot
                // Disable dribbling
                holding = true;
                dribbling = false;
                stopped = true;
                hand_Collider.isTrigger = true;
            }

            // Mouse is held down
            else if (holding && shootTime != 0f) {
                // Shoot the ball
                // Calculate shooting direction
                holding = false;
                stopped = false;
                Vector2 shootDirection = new(difference.x, difference.y);
                shootDirection.Normalize();

                // Scale the force applied on the ball with the mouse down time
                float shootForceScale = Mathf.Min(Time.time - shootTime, SHOOTTIME) + BASESHOOTTIME;
                Vector2 shootForce = shootForceScale * shootParam * shootDirection;
                ball_Rigidbody2D.AddForce(shootForce);
            }
            shootTime = 0f;
        }

        if (holding) {
            // Set the position, speed and angular velocity to the same as the hand
            // Angular velocity is included to hopefully make swinging the arm have an impact
            ball.transform.position = hand.transform.position;
            ball_Rigidbody2D.velocity = player_Rigidbody2D.velocity;
        }


        // defaulting to single jump
        if (Input.GetButtonDown("Jump") && grounded) {
            if (dribbling) {
                // needs to be in layup
                player_Rigidbody2D.AddForce(new(0, jumpParam), ForceMode2D.Impulse);
            } else {
                stopped = false;
                player_Rigidbody2D.AddForce(new(inputX * jumpParam, jumpParam), ForceMode2D.Impulse);
            }
        } else if (grounded && !stopped) {
            player_Rigidbody2D.velocity = new(inputX * runParam, player_Rigidbody2D.velocity.y);
        } else if (stopped){
            player_Rigidbody2D.velocity = new(0, player_Rigidbody2D.velocity.y);
        }

        // Set parameters for animation controller
        // Must stay at the end of Update function
        player_Animator.SetFloat("Speed", Mathf.Abs(player_Rigidbody2D.velocity.x));
        player_Animator.SetBool("Jump", !grounded);
        player_Animator.SetBool("FaceForward", faceRight == (player_Rigidbody2D.velocity.x > 0));
    }

    private void FixedUpdate() {
        Debug.Log("Fixed Update");
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            grounded = true;

            // Force a random shot if the player is falling to the ground with ball
            // The force is also randomized if the mouse is not held down
            if (forceShoot && holding) {
                holding = false;
                Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3 difference = mouse - transform.position;
                if (shootTime < SHOOTTIMEMIN) {
                    shootTime = Random.Range(0, BASESHOOTTIME);
                }
                shootTime += BASESHOOTTIME;
                Vector2 shootDirection = new(difference.x, difference.y);
                shootDirection.Normalize();
                // TODO: change the forced shot strength
                Vector2 forcedShot = new(shootTime * Random.Range(1f-forceShootError, 1f + forceShootError) * shootParam * 2.5f * shootDirection.x, 
                    shootTime * Random.Range(1f - forceShootError, 1f + forceShootError) * shootParam * 2.5f * shootDirection.y);
                ball_Rigidbody2D.AddForce(forcedShot);

                shootTime = 0f;
            }

            forceShoot = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            Debug.Log("Ground collision exit");
            stopped = false;
            grounded = false;
            if (holding) {
                forceShoot = true;
            }
        }
    }
}
