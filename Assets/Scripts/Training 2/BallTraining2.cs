using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTraining2 : MonoBehaviour
{
    private SpriteRenderer ball_spriteRenderer;

    void Start()
    {
        ball_spriteRenderer = GetComponent<SpriteRenderer>();
    }


    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D collision) {
        
    }


    private void OnCollisionEnter2D(Collision2D collision) {
        
    }
}
