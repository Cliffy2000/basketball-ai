using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public float delay = 30f;
    private float hoopTop = -1f;

    private SpriteRenderer ball;

    private void Start()
    {
        ball = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        delay -= Time.deltaTime;
        if (delay <= 0)
        {
            Debug.Log("destroying");
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HoopTop"))
        {
            hoopTop = Time.time;
        }

        if (collision.CompareTag("HoopBottom"))
        {
            if (Time.time - hoopTop <= 1f)
            {
                ball.color = new Color(0, 1, 0, 1);
            }
            else
            {
                hoopTop = -1f;
            }
        }
    }
}
