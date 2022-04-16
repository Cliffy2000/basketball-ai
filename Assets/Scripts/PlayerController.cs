using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Player components
    private Rigidbody2D player_Rigidbody2D;
    private Animator player_Animator;

    // Other GameObjects
    public GameObject hand;
    public GameObject ball;

    // Player parameters
    public float runParam = 10f;
    public float jumpParam = 15f;

    private float inputX;
    private bool grounded = true;

    private void Start() {
        player_Rigidbody2D = GetComponent<Rigidbody2D>();
        player_Animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update() {
        inputX = Input.GetAxisRaw("Horizontal");

        // defaulting to single jump
        if(Input.GetButtonDown("Jump") && grounded) {
            player_Rigidbody2D.AddForce(new Vector2(0, jumpParam), ForceMode2D.Impulse);
        }

        Vector2 v = player_Rigidbody2D.velocity;
        v.x = inputX * runParam;

        // Flips player facing according to mouse location
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        //difference.Normalize();
        float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        bool faceRight = Mathf.Abs(rotZ) <= 90;

        Vector3 scale = transform.localScale;
        
        if (faceRight && scale.x < 0) {
            // triggers on facing right
            transform.localScale = new Vector3(scale.x * -1, scale.y, scale.z);
        } else if (!faceRight && scale.x > 0) {
            transform.localScale = new Vector3(scale.x * -1, scale.y, scale.z);
        }

        player_Rigidbody2D.velocity = v;

        // Set parameters for animation controller
        player_Animator.SetFloat("Speed", v.magnitude);
        player_Animator.SetBool("Jump", !grounded);
        player_Animator.SetBool("FaceForward", faceRight == (v.x>0));
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            grounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            grounded = false;
        }
    }
}
