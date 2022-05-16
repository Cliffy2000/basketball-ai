using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTraining1 : MonoBehaviour
{
    // public float shootForce = 0.25f;
    // public float shootDirection = 0f;
    public GameObject ball;
    public GameObject hand;
    private Rigidbody2D ball_rigidbody2D;
    private Rigidbody2D player_rigidbody2D;
    private BallTraining1 ball_Script;
    public Gene gene;
    public float vX;
    public float armDirection = 0;
    public bool shootPos = false;

    public bool holding = true;
    public bool grounded = false;
    // Start is called before the first frame update
    void Start() {
        ball_rigidbody2D = ball.GetComponent<Rigidbody2D>();
        player_rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void setBall(GameObject ball) {
        this.ball = ball;
        ball_rigidbody2D = ball.GetComponent<Rigidbody2D>();
        ball_Script = ball.GetComponent<BallTraining1>();
    }

    // Update is called once per frame
    void Update() {
        if (holding) {
            ball_rigidbody2D.transform.position = hand.transform.position;
        }
    }

    private void FixedUpdate() {
        float[] posXNode = new float[] { (player_rigidbody2D.position.x + 25f) / 25} ;
        float[] actions = gene.feedForward(posXNode);
        armDirection = actions[1] * 1440;

        if (actions[2] > 0f && grounded && holding) {
            holding = false;
            Vector2 shoot = ((actions[0] / 2.5f) + 0.15f) * new Vector2(Mathf.Sin(armDirection * Mathf.Deg2Rad), Mathf.Cos(armDirection * Mathf.Deg2Rad));
            ball_rigidbody2D.velocity = new(0, 0);
            ball_rigidbody2D.AddForce(shoot, ForceMode2D.Impulse);
            if (transform.position.x < 9) {
                ball_Script.score = 2;
                ball_Script.lockScore = true;
            }
        }

        if (grounded) {
            if (actions[3] > 0) {
                vX = -11;
            } else if (actions[3] < -0) {
                vX = 11;
            }
            player_rigidbody2D.velocity = new Vector2(vX, 0);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            grounded = true;
        }
    }
}
