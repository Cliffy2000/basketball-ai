using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTraining1v2 : MonoBehaviour {
    // public float shootForce = 0.25f;
    // public float shootDirection = 0f;
    public GameObject ball;
    public GameObject hand;
    private Rigidbody2D ball_rigidbody2D;
    private BallTraining1 ball_script;
    private Rigidbody2D player_rigidbody2D;

    public Gene gene;

    private float startTime;
    private float shootForce;
    public float shootDirection;
    private float movementX;
    private float shootOrNot;

    public bool holding = true;
    public bool grounded = false;

    void Start() {
        player_rigidbody2D = GetComponent<Rigidbody2D>();
        startTime = Time.time;
    }

    public void setBall(GameObject ball) {
        this.ball = ball;
        ball_rigidbody2D = ball.GetComponent<Rigidbody2D>();
        ball_script = ball.GetComponent<BallTraining1>();
    }

    // Update is called once per frame
    void Update() {
        // First use the gene to determine the actions taken
        // Game state is { playerX, playerSpeed}
        // Converts playerX from [-30, 30] to [-1, 1]
        // Converts playerX from [-12, 12] to [-1, 1]
        float[] gameState = new float[] {
            player_rigidbody2D.position.x / 30,
            player_rigidbody2D.velocity.x / 10,
            (Time.time - startTime) / 7.5f
        };

        float[] gameInput = gene.feedForward(gameState);


        shootForce = gameInput[0];
        shootDirection = gameInput[1];
        movementX = gameInput[2];
        shootOrNot = gameInput[3];


        shootDirection *= 720;
        if (movementX > 0.1) movementX = 1;
        else if (movementX < -0.1) movementX = -1;
        else movementX = 0;

        if (holding) {
            ball_rigidbody2D.transform.position = hand.transform.position;
            ball_rigidbody2D.velocity = player_rigidbody2D.velocity;
        }
    }

    private void FixedUpdate() {
        if (shootOrNot > -0.1f && shootOrNot < 0.1f && grounded && holding) {
            holding = false;
            Vector2 shoot = (
                ((shootForce + 1) / 4f) * 
                new Vector2(
                    Mathf.Sin(shootDirection * Mathf.Deg2Rad), 
                    Mathf.Cos(shootDirection * Mathf.Deg2Rad)
                    )
                );
            ball_rigidbody2D.AddForce(shoot, ForceMode2D.Impulse);

            if (Time.time - startTime > 2f) {
                ball_script.score = 5f;
            }
        }

        if (grounded) {
            player_rigidbody2D.velocity = new Vector2(movementX * 10, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            grounded = true;
        }
    }
}
