using UnityEngine;
using System;
using UnityEditor.Scripting.Python;
using UnityEditor;
using System.Linq;

public class Game : MonoBehaviour
{
    private float genStartTime = 0f;
    private float genTime = 3f;
    private int populationSize = 60;
    private int timeScale = 4;
    private Gene[] genes;
    private int generation = 0;

    public GameObject player;
    public GameObject ball;
    private GameObject[] players;
    private GameObject[] balls;

    string output_path = @"Data/result.txt";
    string input_path = @"Data/nextGen.txt";


    private bool isRunning = false;

    private void Start()
    {
        Time.timeScale = timeScale;
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
            generation += 1;
            float totalScore = 0f;
            for (var i = 0; i < populationSize; i++)
            {
                genes[i].score = balls[i].GetComponent<BallTraining1>().score;
                totalScore += genes[i].score;
            }

            Debug.Log("Generation: " + generation + " Score: " + totalScore);

            string[] geneText = new string[populationSize];
            for (var i = 0; i < populationSize; i++)
            {
                geneText[i] = genes[i].ToString();
            }
            File.WriteAllLines(output_path, geneText);


            Debug.Log("Python started");

            string scriptPath = Path.Combine(Application.dataPath, "Scripts/Training 1/main.py");
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
    int generation;
    int[] geneShape;
    int[][] netShape;
    public float score;
    float maxScore;
    float[] gene;


    public Gene(int[] geneShape, int[][] netShape) {
        // Create the gene and calculate the network shape
        this.geneShape = geneShape;
        this.netShape = netShape;
        // Move this outside of gene creation
        /*
        for (var i = 0; i < geneShape.Length-1; i++) {
            netShape[i] = new int[] { this.geneShape[i], this.geneShape[i + 1] };
        }
        */
        this.maxScore = 100;
    }

    public void convertWeights(string input) {
        // Stores the input text string as an 1D array of floats inside the gene
        string[] geneText = input.Split(' ');
        gene = new float[geneText.Length];

        for (var i=0; i<geneText.Length; i++) {
            gene[i] = float.Parse(geneText[i]);
        }
    }
    
    public void feedForward(float[] gameState) {
        // store a value for each node
        float[] nodeVals = new float[geneShape.Sum()];

        int filledNodeCount = 0; // number of nodes with filled values
        for (var i=0; i<geneShape[0]; i++) {
            // Insert the gameState into nodeVals as first layer
            nodeVals[i] = gameState[i];
            filledNodeCount++;
        }

        for (var nextLayer=1; nextLayer<geneShape.Length; nextLayer++) {
            // Go through each next layer,
            // calculate the weighted sums as each node value
            int nodesInNext = geneShape[nextLayer];
            int nodesInPrev = geneShape[nextLayer - 1];
            // Calculate the value of each node in the next layer
            for (var n=0; n<nodesInNext; n++) {
                float nodeVal = 0;
                // sum the weighted values from the previous layer
                for (var p=0; p<nodesInPrev; p++) {
                    int prevNodeIndex = filledNodeCount - nodesInPrev + p;
                    nodeVal += gene[prevNodeIndex] * nodeVals[prevNodeIndex];
                }
            }
        }
    }
}

