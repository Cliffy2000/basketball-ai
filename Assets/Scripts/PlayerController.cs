using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D p_Rigidbody2D;
    private SpriteRenderer p_SpriteRenderer;

    public float runSpeed = 10f;
    public float jumpForce = 15f;
    float movement;

    private void Start()
    {
        p_Rigidbody2D = GetComponent<Rigidbody2D>();
        p_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    private void Update()
    {
        movement = Input.GetAxisRaw("Horizontal");

        // The model would lean a little on horizontal move, thus jump check must not be at 0
        if (Input.GetButtonDown("Jump") && p_Rigidbody2D.velocity.y >= -0.01)
        {
            p_Rigidbody2D.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
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

    }


}
