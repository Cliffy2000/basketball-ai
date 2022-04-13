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
    private float movement;

    private int jumpCount = 0;
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

        // Flips player facing according to mouse location
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        difference.Normalize();     // normalizing the vector. Meaning that all the sum of the vector will be equal to 1
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;   // find the angle in degrees

        bool faceRight = Mathf.Abs(rotZ) <= 90;

        Vector3 scale = transform.localScale;
        
        if (faceRight && scale.x < 0) {
            // triggers on facing right
            transform.localScale = new Vector3(scale.x * -1, scale.y, scale.z);
        } else if (!faceRight && scale.x > 0) {
            transform.localScale = new Vector3(scale.x * -1, scale.y, scale.z);
        }

        p_Rigidbody2D.velocity = v;

        p_Animator.SetFloat("Speed", v.magnitude);
        p_Animator.SetBool("Jump", jumpCount > 0);
        p_Animator.SetBool("FaceForward", faceRight == (v.x>0));
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            jumpCount = 0;
        }
    }
}
