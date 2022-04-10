using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {
    private float hoopTop = -1f;
    public bool holding = false;
    private bool shooting = false;
    private float shootTime;
    private float SHOOTTIME = 1f;

    public GameObject hand;
    public GameObject arm;

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
        //Debug.Log(Physics.GetIgnoreLayerCollision(7, 9));
        if (Input.GetKeyDown(KeyCode.R)) {
            Vector3 handPos = hand.transform.position;
            ball_Rigidbody2D.MovePosition(new Vector2(handPos.x, handPos.y));
        }

        if (Input.GetMouseButtonUp(0)) {
            holding = false;
            shooting = true;
        }

        if (holding) {
            transform.position = hand.transform.position;
        }

        if (this.shooting) {
            Physics2D.IgnoreLayerCollision(7, 9);
            shootTime -= Time.deltaTime;
            if (shootTime <= 0) {
                Physics2D.IgnoreLayerCollision(7, 9, false);
                shootTime = SHOOTTIME;
                shooting = false;
            }
        }
        Debug.Log(shootTime);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("HoopTop")) hoopTop = Time.time;
        if (collision.CompareTag("HoopBottom")) {
            if (Time.time - hoopTop <= 1f) ball_SpriteRenderer.color = new Color(0, 1, 0, 1);
            else hoopTop = -1f;
        }

        if (collision.CompareTag("Hand")) this.holding = true;
    }
}
