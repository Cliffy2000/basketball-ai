using UnityEngine;

public class GameScenesScript : MonoBehaviour {
	
	public GameObject player;
	public GameObject ball;
	private GameObject[] players = new GameObject[50];
	private GameObject[] balls = new GameObject[50];

	private void Start() {

		for (var i = 0; i < 50; i++)
        {
			Debug.Log("Creating player " + i);
			players[i] = Instantiate(player, new Vector3(i * 0.0f, 0, 0), Quaternion.identity);
			balls[i] = Instantiate(ball, new Vector3(i * 0.0f, 0, 0), Quaternion.identity);
			players[i].GetComponent<PlayerTraining1>().setBall(balls[i]);
        }
	}

	private void Update() {
		for (var i = 0; i < 2; i++)
		{
			//players[i].GetComponent<PlayerTraining1>().tryShoot(balls[i]);
		}
	}
}
