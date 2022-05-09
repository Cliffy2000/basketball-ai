using UnityEngine;
using System.IO;
using UnityEditor.Scripting.Python;
using UnityEditor;


public class GameScenesScript : MonoBehaviour
{
    private float genStartTime = 0f;
    private float genTime = 3f;
    private int populationSize = 50;
    private Gene[] genes;

    public GameObject player;
    public GameObject ball;
    private GameObject[] players;
    private GameObject[] balls;

    string output_path = @"Assets/data.txt";
    string input_path = @"Assets/nextGen.txt";


    private bool isRunning = false;

    private void Start()
    {
        genes = new Gene[populationSize];
        players = new GameObject[populationSize];
        balls = new GameObject[populationSize];

        for (var i = 0; i < populationSize; i++)
        {
            genes[i] = new Gene();
        }

        // Generate random params
        for (var i = 0; i < populationSize; i++)
        {
            players[i] = Instantiate(player, new Vector3(i * 0.0f, 0, 0), Quaternion.identity);
            players[i].GetComponent<PlayerTraining1>().shootDirection = genes[i].g1;
            players[i].GetComponent<PlayerTraining1>().shootForce = genes[i].g2;
            balls[i] = Instantiate(ball, new Vector3(i * 0.0f, 0, 0), Quaternion.identity);
            players[i].GetComponent<PlayerTraining1>().setBall(balls[i]);
        }

        genStartTime = Time.time;
    }

    private void Update()
    {
        if (Time.time - genStartTime > genTime && !isRunning)
        {
            isRunning = true;
            for (var i = 0; i < populationSize; i++)
            {
                genes[i].score = balls[i].GetComponent<BallTraining1>().score;
            }

            string[] geneText = new string[populationSize];
            for (var i = 0; i < populationSize; i++)
            {
                geneText[i] = genes[i].ToString();
            }
            File.WriteAllLines(output_path, geneText);


            Debug.Log("Python started");

            string scriptPath = Path.Combine(Application.dataPath, "main.py");
            PythonRunner.RunFile(scriptPath);

            Debug.Log("Python finished");


            //Destoy previous population
            for (var i = 0; i < populationSize; i++)
            {   
                Destroy(players[i]);
                Destroy(balls[i]);
            }

            var lines = File.ReadAllLines(input_path);
            for (var i = 0; i < populationSize; i++)
            {
                genes[i] = new Gene(float.Parse(lines[i].Split(' ')[0]), float.Parse(lines[i].Split(' ')[1]));

                players[i] = Instantiate(player, new Vector3(i * 0.0f, 0, 0), Quaternion.identity);
                players[i].GetComponent<PlayerTraining1>().shootDirection = genes[i].g1;
                players[i].GetComponent<PlayerTraining1>().shootForce = genes[i].g2;
                balls[i] = Instantiate(ball, new Vector3(i * 0.0f, 0, 0), Quaternion.identity);
                players[i].GetComponent<PlayerTraining1>().setBall(balls[i]);
            }

            genStartTime = Time.time;
            isRunning = false;
        }
    }
}

public class Gene
{
    public float g1;
    public float g2;
    public float score;
    int generation;

    public Gene()
    {
        g1 = Random.Range(0, 360);
        g2 = Random.Range(0.2f, 0.4f);
        score = 0f;
        generation = 1;
    }

    public Gene(float g1, float g2)
    {
        this.g1 = g1;
        this.g2 = g2;
    }

    public override string ToString()
    {
        return g1 + " " + g2 + " " + score;
    }
}

