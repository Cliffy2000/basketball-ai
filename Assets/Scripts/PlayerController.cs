using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D p_Rigidbody2D;
    private SpriteRenderer p_SpriteRenderer;
    private Animator p_Animator;

    public float runSpeed = 10f;
    public float jumpForce = 15f;
    public float movement;

    public int jumpCount = 0;
    public readonly int maxJumpCount = 1;

    private void Start()
    {
        p_Rigidbody2D = GetComponent<Rigidbody2D>();
        p_SpriteRenderer = GetComponent<SpriteRenderer>();
        p_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        movement = Input.GetAxisRaw("Horizontal");

        // TODO: rework jump check
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount)
        {
            p_Rigidbody2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            jumpCount++;
        }

        /*
        Vector2 newPosition = p_Rigidbody2D.position;
        newPosition.x += movement * Time.fixedDeltaTime * runSpeed;
        p_Rigidbody2D.MovePosition(newPosition);
        */

        Vector2 v = p_Rigidbody2D.velocity;
        v.x = movement * runSpeed;

        Vector3 scale = transform.localScale;
        if ((v.x > 0 && scale.x < 0) || (v.x < 0 && scale.x > 0))
        {
            transform.localScale = new Vector3(scale.x * -1, scale.y, scale.z);
        }

        p_Rigidbody2D.velocity = v;

        p_Animator.SetBool("Run", (Mathf.Abs(v.x) >= 0.01));
        p_Animator.SetBool("Jump", jumpCount > 0);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            jumpCount = 0;
        }
    }
}
