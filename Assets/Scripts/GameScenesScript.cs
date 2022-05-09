using UnityEngine;
using System.IO;

public class GameScenesScript : MonoBehaviour {
	private float genStartTime = 0f;
	private float genTime = 3f;
	private float populationSize = 50;
	private Gene[] genes = new Gene[50];

	public GameObject player;
	public GameObject ball;
	private GameObject[] players = new GameObject[50];
	private GameObject[] balls = new GameObject[50];

	string path = @"C:\Users\cliff\Desktop\Basketball-AI\Assets\data.txt";


	private void Start() {
		for (var i = 0; i < populationSize; i++) {
			genes[i] = new Gene();
        }

		// Generate random params
		for (var i = 0; i < populationSize; i++) {
			players[i] = Instantiate(player, new Vector3(i * 0.0f, 0, 0), Quaternion.identity);
			players[i].GetComponent<PlayerTraining1>().shootDirection = genes[i].g1;
			players[i].GetComponent<PlayerTraining1>().shootForce = genes[i].g2;
			balls[i] = Instantiate(ball, new Vector3(i * 0.0f, 0, 0), Quaternion.identity);
			players[i].GetComponent<PlayerTraining1>().setBall(balls[i]);
        }

		genStartTime = Time.time;
	}

	private void Update() {
		if (Time.time - genStartTime > genTime) {
			for (var i = 0; i < populationSize; i++) {
				genes[i].score = balls[i].GetComponent<BallTraining1>().score;
			}

			string[] geneText = new string[50];
			for (var i = 0; i < populationSize; i++) {
				geneText[i] = genes[i].ToString();
			}
			File.WriteAllLines(path, geneText);
		}
	}
}

public class Gene {
	public float g1;
	public float g2;
	public float score;
	int generation;

	public Gene() {
		g1 = Random.Range(0, 360);
		g2 = Random.Range(0.2f, 0.4f);
		score = 0f;
		generation = 1;
    }

    public override string ToString() {
		return g1 + " " + g2 + " " + score;
    }
}

