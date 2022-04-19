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

    // Player parameters
    public float runParam = 10f;
    public float jumpParam = 15f;
    public float shootParam = 12f;
    public float dribbleParam = 2f;
    private float inputX;

    // Game Logic variables
    private bool grounded = true;
    public bool holding = false;
    private bool shooting = false;
    private float shootTime = 0f;
    private readonly float SHOOTTIME = 1.5f;
    private readonly float SHOOTTIMEMIN = 0.1f;
    private readonly float BASESHOOTTIME = 0.5f;
    private float shootTImeAfter;
    private readonly float SHOOTTIMEAFTER = 1f;
    public bool dribbling = false;
    private bool stopped = false;


    private void Start() {
        player_Rigidbody2D = GetComponent<Rigidbody2D>();
        player_Animator = GetComponent<Animator>();
        ball_Rigidbody2D = ball.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update() {
        inputX = Input.GetAxisRaw("Horizontal");

        // defaulting to single jump
        if(Input.GetButtonDown("Jump") && grounded) {
            player_Rigidbody2D.AddForce(new Vector2(0, jumpParam), ForceMode2D.Impulse);
        }

        Vector2 v = new(inputX * runParam, player_Rigidbody2D.velocity.y);
        player_Rigidbody2D.velocity = v;

        // Flips player facing according to mouse location
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 difference = mouse - transform.position;
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        bool faceRight = Mathf.Abs(rotZ) <= 90;

        Vector3 scale = transform.localScale;
        // inverts the sprite if the facing is opposite to mouse direction
        if ((faceRight && scale.x < 0) || (!faceRight && scale.x > 0)) {
            transform.localScale = new Vector3(scale.x * -1, scale.y, scale.z);
        }

        // Set parameters for animation controller
        player_Animator.SetFloat("Speed", v.magnitude);
        player_Animator.SetBool("Jump", !grounded);
        player_Animator.SetBool("FaceForward", faceRight == (v.x>0));

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
                Debug.Log(dribbling);
            }

            // Mouse is held down
            else if (holding && shootTime != 0f) {
                Debug.Log(Time.time - shootTime);
                // Shoot the ball
                // Calculate shooting direction
                holding = false;
                shooting = true;
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

        if (shooting) {
            // Resets parameters and collisions after the ball is shot
            shootTImeAfter -= Time.deltaTime;
            if (shootTImeAfter <= 0) {
                // Enough time has passed
                Physics2D.IgnoreLayerCollision(7, 9, false);
                shootTImeAfter = SHOOTTIMEAFTER;
                shooting = false;
            }
        }

        if (dribbling) {
            if (ball.transform.position.y >= hand.transform.position.y) {
                ball_Rigidbody2D.AddForce(new(0, -dribbleParam));
            }
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
