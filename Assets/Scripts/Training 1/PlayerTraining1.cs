using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTraining1 : MonoBehaviour
{
    public float shootForce = 0.25f;
    public float shootDirection = 0f;
    public GameObject ball;
    public GameObject hand;
    private Rigidbody2D ball_rigidbody2D;
    private Rigidbody2D player_rigidbody2D;

    private bool reset = false;
    public bool holding = true;
    private bool allowShoot = false;
    // Start is called before the first frame update
    void Start()
    {
        ball_rigidbody2D = ball.GetComponent<Rigidbody2D>();
        player_rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void setBall(GameObject ball)
    {
        this.ball = ball;
        ball_rigidbody2D = ball.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            reset = true;
        }
        if (holding) {
            ball_rigidbody2D.transform.position = hand.transform.position;
        }
    }

    private void FixedUpdate() {
        if (reset) {
            shootDirection = Random.Range(0, 360);
            shootForce = Random.Range(0.2f, 0.4f);
            reset = false;
            holding = true;
            allowShoot = false;
            player_rigidbody2D.MovePosition(new(transform.position.x, transform.position.y + 5));
            ball_rigidbody2D.velocity = new Vector2(0, 0);
            ball_rigidbody2D.angularVelocity = 0;
        }
        if (allowShoot) {
            holding = false;
            allowShoot = false;
            Vector2 shoot = shootForce * new Vector2(Mathf.Sin(shootDirection * Mathf.Deg2Rad), Mathf.Cos(shootDirection * Mathf.Deg2Rad));
            ball_rigidbody2D.AddForce(shoot, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            allowShoot = true;
        }
    }

    public void tryShoot(Rigidbody2D b) {
        
    }
}
