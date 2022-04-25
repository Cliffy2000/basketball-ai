using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {
    // Timer to check hoop top and bottom collision order
    private float hoopTopTimer = -1f;
    private bool reset = false;
    private float greenTime;
    private readonly float GREENTIME = 1f;
    private bool green = false;

    public GameObject hand;
    public GameObject body;

    private SpriteRenderer ball_SpriteRenderer;
    private Rigidbody2D ball_Rigidbody2D;
    private PlayerController player_Script;
    private Rigidbody2D player_Rigidbody2D;
    private Collider2D hand_Collider;

    private void Start() {
        ball_SpriteRenderer = GetComponent<SpriteRenderer>();
        ball_Rigidbody2D = GetComponent<Rigidbody2D>();
        player_Script = body.GetComponent<PlayerController>();
        player_Rigidbody2D = body.GetComponent<Rigidbody2D>();
        hand_Collider = hand.GetComponent<Collider2D>();
    }

    void Update() {
        // Reset the ball position and clear velocity
        if (Input.GetKeyDown(KeyCode.R)) {
            reset = true;
            hand_Collider.isTrigger = true;
        }

        if (green) {
            // Resets ball color after enought time has passed
            if (Time.time - greenTime > GREENTIME) {
                ball_SpriteRenderer.color = new Color(1, 1, 1, 1);
                green = false;
            }
        }
        
        if (player_Script.holding) {
            transform.position = hand.transform.position;
        }

        // if dribbling, constrain ball x to hand x
        if (player_Script.dribbling) {
            transform.position = new(hand.transform.position.x, transform.position.y);
            // in case the ball goes above the hand let the ball pass through
            if (transform.position.y > hand.transform.position.y) {
                hand_Collider.isTrigger = true;
            } else {
                hand_Collider.isTrigger = false;
            }
        }
    }

    public void FixedUpdate() {
        if (reset) {
            // resets the ball if r button was pressed
            reset = false;
            // Resets the ball position and related vectors
            // Also restores collision
            Vector3 handPos = hand.transform.position;
            ball_Rigidbody2D.position = new Vector2(10, 4);
            ball_Rigidbody2D.velocity = new Vector2(0, 0);
            ball_Rigidbody2D.angularVelocity = 0f;
        }

        if (player_Script.holding) {
            ball_Rigidbody2D.velocity = player_Rigidbody2D.velocity;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        // Checks hoop collision with to triggers
        if (collision.CompareTag("HoopTop")) hoopTopTimer = Time.time;
        if (collision.CompareTag("HoopBottom")) {
            if (Time.time - hoopTopTimer <= 1f) {
                ball_SpriteRenderer.color = new Color(0, 0.75f, 0, 1);
                green = true;
                greenTime = Time.time;
            } else hoopTopTimer = -1f;
        }

        if (collision.CompareTag("Hand")) {
            // Catch the ball
            if (!player_Script.dribbling) {
                player_Script.holding = true;
                hand_Collider.isTrigger = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (player_Script.dribbling && (transform.position.y < hand.transform.position.y)) {
            hand_Collider.isTrigger = false; 
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        // Activates only when the hand is not a trigger (dribbling)
        // Gives the ball downward dribbling force
        if (collision.gameObject.CompareTag("Hand")  && player_Script.dribbling) {
            float dribbleScale = 0.5f + Mathf.Abs(player_Script.rotZ + 90) / 60;
            ball_Rigidbody2D.AddForce(new(0, -player_Script.dribbleParam*dribbleScale), ForceMode2D.Impulse);
        }

        if (collision.gameObject.CompareTag("Ground") && player_Script.dribbling) {
            ball_Rigidbody2D.AddForce(new(0, player_Script.dribbleParam / 3), ForceMode2D.Impulse);
        }
    }
}
