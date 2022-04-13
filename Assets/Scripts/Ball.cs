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
    private readonly float SHOOTTIME = 1.5f;
    public float maxShootForce = 12f;
    private float shootTimeAfter;
    private readonly float SHOOTTIMEAFTER = 1f;

    private float greenTime;
    private readonly float GREENTIME = 1f;
    private bool green = false;

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

        shootTimeAfter = SHOOTTIMEAFTER;
    }

    void Update() {
        // Reset the ball position and clear velocity
        if (Input.GetKeyDown(KeyCode.R)) {
            Vector3 handPos = hand.transform.position;
            ball_Rigidbody2D.MovePosition(new Vector2(handPos.x, 5));
            ball_Rigidbody2D.velocity = new Vector2(0, 0);
            ball_Rigidbody2D.angularVelocity = 0f;
        }

        if (Input.GetMouseButtonDown(0) && holding) {
            shootTime = Time.time;
        }

        // Shoot the ball on mouse left release
        if (Input.GetMouseButtonUp(0)) {
            if (holding) {
                holding = false;
                shooting = true;
                Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 direction = new Vector2(mouse.x - ball_Rigidbody2D.position.x, mouse.y - ball_Rigidbody2D.position.y);
                direction.Normalize();

                float timeElapsed = Time.time - shootTime;
                if (timeElapsed > SHOOTTIME) timeElapsed = SHOOTTIME;
                timeElapsed += 0.5f;

                Vector2 f = direction * maxShootForce * timeElapsed;
                ball_Rigidbody2D.AddForce(f);
                Debug.Log(direction.magnitude);
                Debug.Log(f.magnitude);
                shootTime = -1;
            }
        }

        if (holding) {
            transform.position = hand.transform.position;
            ball_Rigidbody2D.velocity = body.GetComponent<Rigidbody2D>().velocity;
        }

        if (shooting) {
            shootTimeAfter -= Time.deltaTime;
            if (shootTimeAfter <= 0) {
                Physics2D.IgnoreLayerCollision(7, 9, false);
                shootTimeAfter = SHOOTTIMEAFTER;
                shooting = false;
            }
        }

        if (green) {
            if (Time.time - greenTime > GREENTIME) {
                ball_SpriteRenderer.color = new Color(1, 1, 1, 1);
                green = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("HoopTop")) hoopTopTimer = Time.time;
        if (collision.CompareTag("HoopBottom")) {
            if (Time.time - hoopTopTimer <= 1f) {
                ball_SpriteRenderer.color = new Color(0, 0.75f, 0, 1);
                green = true;
                greenTime = Time.time;
            } else hoopTopTimer = -1f;
        }

        if (collision.CompareTag("Hand")) {
            this.holding = true;
            Physics2D.IgnoreLayerCollision(7, 9);
        }
    }
}
