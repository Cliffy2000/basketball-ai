using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallTraining2 : MonoBehaviour
{
    private SpriteRenderer ball_spriteRenderer;

    private GameObject player;
    private PlayerTraining2 player_script;

    private float hoopTopTimer = -1f;

    public void Start() {
        ball_spriteRenderer = GetComponent<SpriteRenderer>();
    }


    public void Update() {
        
    }

    public void setPlayer(GameObject player) {
        this.player = player;
        player_script = this.player.GetComponent<PlayerTraining2>();
    }


    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject.name == "Eval") {
            float exitScore = (-transform.position.x - 6.7f) * 2 + 20;
            player_script.gene.score = Mathf.Max(player_script.gene.score, exitScore);
        }

        if (collision.CompareTag("HoopTop")) hoopTopTimer = Time.time;
        if (collision.CompareTag("HoopBottom")) {
            if (Time.time - hoopTopTimer <= 1f) {
                player_script.gene.score = 100f;
                ball_spriteRenderer.color = new Color(0, 0.75f, 0, 1);
            } else hoopTopTimer = -1f;
        }
    }
}
