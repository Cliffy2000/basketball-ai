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
    public Gene gene;
    public float armDirection = 0;
    public float randomV;
    private bool prevdecision = false;
    private int changeCounter = 0;

    public bool holding = true;
    private bool grounded = false;
    // Start is called before the first frame update
    void Start() {
        ball_rigidbody2D = ball.GetComponent<Rigidbody2D>();
        player_rigidbody2D = GetComponent<Rigidbody2D>();
        randomV = Random.Range(-6, -14);
    }

    public void setBall(GameObject ball) {
        this.ball = ball;
        ball_rigidbody2D = ball.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update() {
        if (holding) {
            ball_rigidbody2D.transform.position = hand.transform.position;
        }
    }

    private void FixedUpdate() {
        if (grounded) {
            player_rigidbody2D.velocity = new Vector2(randomV, 0);
        }
        
        float[] posXNode = new float[] { (player_rigidbody2D.position.x + 15f) / 40, player_rigidbody2D.velocity.x } ;
        float[] actions = gene.feedForward(posXNode);

        if ((actions[2] > 0 && !prevdecision) || (actions[2] < 0 && prevdecision)) {
            changeCounter += 1;
            prevdecision = !prevdecision;
            if (changeCounter >= 2) {
                Debug.Log("change detected");
            }
        }

        if (actions[2] > 0f && grounded && holding) {
            if (transform.position.x < 10) {
                Debug.Log("SUCCESS");
            }
            holding = false;
            armDirection = actions[1] * 360;
            Vector2 shoot = ((actions[0] / 2.5f) + 0.2f) * new Vector2(Mathf.Sin(armDirection * Mathf.Deg2Rad), Mathf.Cos(armDirection * Mathf.Deg2Rad));
            ball_rigidbody2D.velocity = new(0, 0);
            ball_rigidbody2D.AddForce(shoot, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            grounded = true;
        }
    }
}
