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
        if (Input.GetKeyDown(KeyCode.R)) {
            ball_Rigidbody2D.position = new Vector2(12, 3);
            ball_Rigidbody2D.velocity = new Vector2(0, 0);
            ball_Rigidbody2D.angularVelocity = 0;
        }
        
        if (green) {
            // Resets ball color after enought time has passed
            if (Time.time - greenTime > GREENTIME) {
                ball_SpriteRenderer.color = new Color(1, 1, 1, 1);
                green = false;
            }
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


    }

    private void OnTriggerExit2D(Collider2D collision) {

    }

    private void OnCollisionEnter2D(Collision2D collision) {
        // Activates only when the hand is not a trigger (dribbling)
        // Gives the ball downward dribbling force

    }
}
