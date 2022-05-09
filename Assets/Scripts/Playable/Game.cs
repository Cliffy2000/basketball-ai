using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public GameObject border;
    public GameObject hoop;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("second scene playing");
    }
}


public class Gene {
    float shootNode;
    float shootDirection;
    float playerX;

    public Gene(float playerX) {
        this.playerX = playerX;
    }
}