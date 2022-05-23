using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTraining1 : MonoBehaviour
{
    private SpriteRenderer ball_spriteRenderer;
    private Rigidbody2D ball_rigidbody2D;
    public float score = 1f;
    private bool board = false;
    private bool hoop = false;
    private bool eval = false;

    private float hoopTopTimer = -1f;
    // Start is called before the first frame update
    void Start()
    {
        ball_spriteRenderer = GetComponent<SpriteRenderer>();
        ball_rigidbody2D = GetComponent<Rigidbody2D>();
        score = 1f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.gameObject.name == "Hoop-Front") {
            if (!hoop && score < 100f) {
                score += 15;
                hoop = true;
            }
        }

        if (collision.collider.gameObject.name == "Board") {
            if (!board && score < 100f) {
                score += 10;
                board = true;
            }
        }

    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("HoopTop")) hoopTopTimer = Time.time;
        if (collision.CompareTag("HoopBottom")) {
            if (Time.time - hoopTopTimer <= 1f) {
                score = 100f;
                ball_spriteRenderer.color = new Color(0, 0.75f, 0, 1);
            } else hoopTopTimer = -1f;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.name == "Eval" && !eval && score < 100f) {
            float exitScore = (-transform.position.x - 6.7f) * 2 + 20;
            score += exitScore;
            eval = true;
        }
    }
}
