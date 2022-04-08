using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public float ballInterval = 4f;
    private float currentBallInterval;
    public GameObject ballPrefab;

    private void Start()
    {
        currentBallInterval = ballInterval;
    }

    // Update is called once per frame
    void Update()
    {
        spawnBall();
    }

    void spawnBall()
    {
        currentBallInterval -= Time.deltaTime;
        if (currentBallInterval <= 0)
        {
            Instantiate(ballPrefab, new Vector3(0,5,0), Quaternion.identity);
            currentBallInterval = ballInterval;
        }
    }
}
