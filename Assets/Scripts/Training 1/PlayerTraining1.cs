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

    public bool holding = true;
    private bool allowShoot = false;
    // Start is called before the first frame update
    void Start() {
        ball_rigidbody2D = ball.GetComponent<Rigidbody2D>();
        player_rigidbody2D = GetComponent<Rigidbody2D>();
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
        if (allowShoot) {
            holding = false;
            allowShoot = false;
            float[] posXNode = new float[] { (player_rigidbody2D.position.x - 10) / 5 } ;
            float[] actions = gene.feedForward(posXNode);

            armDirection = actions[1] * 360;
            Vector2 shoot = ((actions[0] / 2.5f) + 0.2f) * new Vector2(Mathf.Sin(armDirection * Mathf.Deg2Rad), Mathf.Cos(armDirection * Mathf.Deg2Rad));
            ball_rigidbody2D.velocity = new(0, 0);
            ball_rigidbody2D.AddForce(shoot, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            allowShoot = true;
        }
    }
}
