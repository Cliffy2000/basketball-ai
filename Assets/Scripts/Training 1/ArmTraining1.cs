using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmTraining1 : MonoBehaviour
{
    public GameObject body;
    private PlayerTraining1 player_Script;
    // Start is called before the first frame update
    void Start()
    {
        player_Script = body.GetComponent<PlayerTraining1>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 180 - player_Script.shootDirection);
    }
}
