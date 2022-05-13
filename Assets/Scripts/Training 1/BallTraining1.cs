using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTraining1 : MonoBehaviour
{
    public float score = 1f;
    private bool lockScore = false;
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
            if (Time.time - hoopTopTimer <= 1f && !lockScore) {
                score = 100f;
                ball_SpriteRenderer.color = new Color(0, 0.75f, 0, 1);
                lockScore = true;
            } else hoopTopTimer = -1f;
        }
 
        if (collision.name == "Eval-Top") evalTopTimer = Time.time;
        if (collision.name == "Eval-Bottom") {
            if (Time.time - evalTopTimer <= 1f && !lockScore) {
                score = (transform.position.x / -20f) * 40;
                lockScore = true;
            }
        }

    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.collider.gameObject.CompareTag("Hoop") && !lockScore) {
            score = Mathf.Max(score, 75f);
        }


        if (collision.gameObject.CompareTag("Ground") && !lockScore) {
            score = 3;
            lockScore = true;
        }
    }
}
