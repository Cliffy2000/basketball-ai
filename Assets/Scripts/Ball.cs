using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {
    // Timer to check hoop top and bottom collision order
    private float hoopTopTimer = -1f;

    // Boolean to mark if the ball is held in hand
    public bool holding = false;

    // Set of variables to record if the ball is being shot and how long it has been
    private bool shooting = false;
    private float shootTime;
    private readonly float SHOOTTIME = 1f;

    public float shootForce = 125f;

    public GameObject hand;
    public GameObject arm;
    public GameObject body;

    private SpriteRenderer ball_SpriteRenderer;
    private Rigidbody2D ball_Rigidbody2D;
    private CircleCollider2D ball_CircleCollider2D;

    private void Start() {
        ball_SpriteRenderer = GetComponent<SpriteRenderer>();
        ball_Rigidbody2D = GetComponent<Rigidbody2D>();
        ball_CircleCollider2D = GetComponent<CircleCollider2D>();

        shootTime = SHOOTTIME;
    }

    void Update() {
        // Reset the ball position and clear velocity
        if (Input.GetKeyDown(KeyCode.R)) {
            Vector3 handPos = hand.transform.position;
            ball_Rigidbody2D.MovePosition(new Vector2(handPos.x, 5));
            ball_Rigidbody2D.velocity = new Vector2(0, 0);
        }

        // Shoot the ball on mouse left release
        if (Input.GetMouseButtonUp(0) && holding) {
            holding = false;
            shooting = true;
            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 n = new Vector2(mouse.x - ball_Rigidbody2D.position.x, mouse.y - ball_Rigidbody2D.position.y);
            n.Normalize();
            ball_Rigidbody2D.AddForce(n * shootForce);
        }

        if (holding) {
            transform.position = hand.transform.position;
            ball_Rigidbody2D.velocity = body.GetComponent<Rigidbody2D>().velocity;
        }

        if (this.shooting) {
            shootTime -= Time.deltaTime;
            if (shootTime <= 0) {
                Physics2D.IgnoreLayerCollision(7, 9, false);
                shootTime = SHOOTTIME;
                shooting = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("HoopTop")) hoopTopTimer = Time.time;
        if (collision.CompareTag("HoopBottom")) {
            if (Time.time - hoopTopTimer <= 1f) ball_SpriteRenderer.color = new Color(0, 0.75f, 0, 1);
            else hoopTopTimer = -1f;
        }

        if (collision.CompareTag("Hand")) {
            this.holding = true;
            Physics2D.IgnoreLayerCollision(7, 9);
        }
    }
}
