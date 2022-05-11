using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTraining1 : MonoBehaviour
{
    public float score = 1f;
    private bool touchGround = false;
    private float hoopTopTimer = -1f;
    private float evalTopTimer = -1f;

    private SpriteRenderer ball_SpriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        ball_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("HoopTop")) hoopTopTimer = Time.time;
        if (collision.CompareTag("HoopBottom")) {
            if (Time.time - hoopTopTimer <= 1f && score <= 1f) {
                ball_SpriteRenderer.color = new Color(0, 0.75f, 0, 1);
                if (touchGround) {
                    score = 80f;
                } else {
                    score = 100f;
                }
            } else hoopTopTimer = -1f;
        }

        if (collision.name == "Eval-Top") evalTopTimer = Time.time;
        if (collision.name == "Eval-Bottom") {
            if (Time.time - evalTopTimer <= 1f && score <= 1f) {
                float s = (transform.position.x + 20f) * 2;
                if (touchGround) {
                    s *= 0.75f;
                }
                score = s;
            }
        }

        if (collision.name == "Ground" && score == 0f) {
            touchGround = true;
        }
    }
}
