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
    private Animator player_animator;
    private Rigidbody2D ball_rigidbody2D;
    private BallTraining2 ball_script;

    public GeneV2 gene;

    private float shootForce;
    private float shootDirection;
    private float movementX;
    private float jump;

    public bool holding = true;
    private bool dribbling = false;
    private bool grounded = false;
    private bool stopped = false;

    private float error = 0;
    private bool pendingShoot = false;
    private bool forceShoot = false;
    private bool faceRight = false;

    public void setBall(GameObject ball) {
        // Assigns the ball to the corresponding player and initializes
        // related variables
        this.ball = ball;
        ball_rigidbody2D = ball.GetComponent<Rigidbody2D>();
        ball_script = ball.GetComponent<BallTraining2>();
    }


    private void Start() {
        player_rigidbody2D = GetComponent<Rigidbody2D>();
        player_animator = GetComponent<Animator>();
    }


    private void Update() {
        // First use the gene to determine the actions taken
        // Game state is { playerX, grounded, stopped }
        // Converts playerX from [-30, 30] to [-1, 1]
        float[] gameState = new float[] { 
            player_rigidbody2D.position.x / 30,
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

        if (dribbling && shootForce <= -0.5) {
            // stop dribbling and hold the ball
            dribbling = false;
            holding = true;
            stopped = true;
        }

        if (movementX != 0 && holding && grounded && !stopped) {
            // TODO: re-enable for dribbling animation
            // holding = false;
            dribbling = true;
            // TODO: add initial dribbling force
        }

        player_animator.SetFloat("Speed", Mathf.Abs(player_animator.velocity.x));
        player_animator.SetBool("Jump", !grounded);
        // enable jump shoot animation here
        // player_animator.SetBool("FaceForward", faceRight == (player_animator.velocity.x > 0));
    }

    private void FixedUpdate() {
        if (jump == 1 && grounded && !dribbling) {
            // jump up and conditionally add sideways displacement
            Vector2 jumpForce = new Vector2(0, jumpParam);
            if (stopped) {
                jumpForce.x = movementX * jumpParam / 2;
            }
            grounded = false;
            player_rigidbody2D.AddForce(jumpForce, ForceMode2D.Impulse);
        }

        if (movementX != 0) {
            if (grounded) {
                float vX = movementX * runParam;
                if (stopped) {
                    vX = 0f;
                }
                player_rigidbody2D.velocity = new Vector2(vX, player_rigidbody2D.velocity.y);
            }
        }

        if (shootForce > 0 && holding) {
            // shoot the ball
            if (player_rigidbody2D.position.x > -10) {
                // the increase range is [0, 0.2]
                // which results in a [-0.2, 0.2] fluctuation range
                error = (player_rigidbody2D.position.x + 10) / 100;
            }
            if (forceShoot) {
                error += 0.3f;
            }
            shootForce += Random.Range(-error, error);
            Vector2 shoot = shootParam * shootForce * new Vector2(Mathf.Sin(shootDirection * Mathf.Deg2Rad), Mathf.Cos(shootDirection * Mathf.Deg2Rad));
            ball_rigidbody2D.AddForce(shoot, ForceMode2D.Impulse);
            holding = false;
            stopped = false;
            pendingShoot = false;
            forceShoot = false;
            error = 0;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.name == "Ground") {
            grounded = true;
        }
        if (pendingShoot) {
            pendingShoot = false;
            forceShoot = true;
        }
    }


    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.name == "Ground") {
            grounded = false;
            if (holding) {
                pendingShoot = true;
            }
        }
    }
}
