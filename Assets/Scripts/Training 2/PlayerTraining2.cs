using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTraining2 : MonoBehaviour {
    // Included functionalities:
    //   Dribbling, stopping, shooting, jump-shooting
    // Other features:
    //   Shooting further away or while dribbling would have less accuracy
    //   Shooting is instantanious

    private float runParam = 16f;
    private float jumpParam = 180f;
    private float shootParam = 0.3f;
    public float dribbleParam = 0.0002f;

    public GameObject ball;
    public GameObject hand;

    private Rigidbody2D player_rigidbody2D;
    private Rigidbody2D ball_rigidbody2D;
    private BallTraining2 ball_script;

    public Gene gene;

    private float shootForce;
    private float shootDirection;
    private float movementX;
    private float jump;

    public bool holding = true;
    private bool dribbling = false;
    private bool grounded = false;
    private bool stopped = false;

    private bool pendingShoot = false;

    public void setBall(GameObject ball) {
        // Assigns the ball to the corresponding player and initializes
        // related variables
        this.ball = ball;
        ball_rigidbody2D = ball.GetComponent<Rigidbody2D>();
        ball_script = ball.GetComponent<BallTraining2>();
    }


    private void Start() {
        player_rigidbody2D = GetComponent<Rigidbody2D>();
    }


    private void Update() {
        // First use the gene to determine the actions taken
        // Game state is { playerX, grounded, stopped }
        float[] gameState = new float[] { 
            player_rigidbody2D.position.x,
            System.Convert.ToSingle(grounded),
            System.Convert.ToSingle(stopped)
        };
        // Game input is { shootForce, shootDirection, movementX, jump }
        float[] gameInput = gene.feedForward(gameState);

        shootForce = gameInput[0];
        shootDirection = gameInput[1];
        movementX = gameInput[2];
        jump = gameInput[3];

        if (shootForce < 0) shootForce = -1;
        shootDirection *= 1440;
        if (movementX > 0.25) movementX = 1;
        else if (movementX < -0.25) movementX = -1;
        if (jump > 0) jump = 1;
        else jump = 0;

        if (holding) {
            ball_rigidbody2D.transform.position = hand.transform.position;
            ball_rigidbody2D.velocity = player_rigidbody2D.velocity;
        }

        if (dribbling && shootForce == -1) {
            // stop dribbling and hold the ball
            dribbling = false;
            holding = true;
            stopped = true;
        }

        if (movementX != 0 && holding && grounded && !stopped) {
            holding = false;
            dribbling = true;
            // TODO: add initial dribbling force
        }
    }

    private void FixedUpdate() {
        if (jump == 1 && grounded && !dribbling) {
            Vector2 jumpForce = new Vector2(0, jumpParam);
            if (stopped) {
                jumpForce.x = movementX * jumpParam / 2;
            }
            grounded = false;
            player_rigidbody2D.AddForce(jumpForce, ForceMode2D.Impulse);
        }

        if (shootForce > 0) {
            // shoot the ball
        }
    }


    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.name == "Ground") {
            grounded = false;
        }
    }
}
