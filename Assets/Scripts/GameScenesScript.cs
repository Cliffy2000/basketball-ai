using UnityEngine;

public class GameScenesScript : MonoBehaviour {
	
	public GameObject player;
	public GameObject ball;
	private GameObject[] players = new GameObject[2];
	private Rigidbody2D[] balls = new Rigidbody2D[2];

	private void Start() {

		for (var i = 0; i < 2; i++)
        {
			Debug.Log("Creating player " + i);
			players[i] = Instantiate(player, new Vector3(i * 2.0f, 0, 0), Quaternion.identity);
			balls[i] = Instantiate(ball, new Vector3(i * 2.0f, 0, 0), Quaternion.identity).GetComponent<Rigidbody2D>();
        }
	}

	private void Update() {
		for (var i = 0; i < 2; i++)
		{
			players[i].GetComponent<PlayerTraining1>().tryShoot(balls[i]);
		}
	}
}
