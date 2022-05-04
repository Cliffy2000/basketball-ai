using System.Collections;
using System.Collections.Generic;
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
    private float shootParam = 0.13f;
    private float dribbleParam = 0.1f;
    private float inputX;
    private Vector3 mouse;

    // Game Logic variables
    private bool jump = false;
    private bool grounded = false;
    public bool dribbling = false;
    private bool stopped = true;

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
            stopped = false;
        }

        if (Input.GetButtonDown("Jump")) {
            // cannot jump if !grounded/dribbling
            jump = true;
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
                player_Rigidbody2D.AddForce(jumpForce, ForceMode2D.Impulse);
            }
        }

        if (inputX != 0) {
            if (!stopped && grounded) {
                player_Rigidbody2D.velocity = new Vector2(inputX * runParam, player_Rigidbody2D.velocity.y);
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
