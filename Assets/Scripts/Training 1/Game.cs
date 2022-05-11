using UnityEngine;
using System.IO;
using UnityEditor.Scripting.Python;
using UnityEditor;
using System.Linq;

public class Game : MonoBehaviour
{
    private float genStartTime = 0f;
    private float genTime = 4.5f;
    private int populationSize = 60;
    private int timeScale = 20;
    private Gene[] genes;
    private int generation = 1;
    private int[] geneShape = new int[] {1,3,2};
    private int[] newShape;

    public GameObject player;
    public GameObject ball;
    private GameObject[] players;
    private GameObject[] balls;

    string starting_path = @"Data/starting.txt";
    string output_path = @"Data/result.txt";
    string input_path = @"Data/nextGen.txt";


    private bool isRunning = false;

    private void Start()
    {
        Time.timeScale = timeScale;
        genes = new Gene[populationSize];
        players = new GameObject[populationSize];
        balls = new GameObject[populationSize];

        var lines = File.ReadAllLines(starting_path);

        for (var i = 0; i < populationSize; i++)
        {
            genes[i] = new Gene(geneShape, generation);
            genes[i].fromText(lines[i]);
        }

        // Generate random params
        for (var i = 0; i < populationSize; i++)
        {
            // spawn player between -10 and 10
            players[i] = Instantiate(player, new Vector3(10, 0, 0), Quaternion.identity);
            players[i].GetComponent<PlayerTraining1>().gene = genes[i];
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

            if (totalScore > 3000) {
                Time.timeScale = 1;
            }

            if (generation % 5 == 0 || Time.timeScale == 1) {
                Debug.Log("Generation: " + generation + " Score: " + totalScore);
            }

            string[] geneText = new string[populationSize];
            for (var i = 0; i < populationSize; i++)
            {
                geneText[i] = genes[i].ToString();
            }
            File.WriteAllLines(output_path, geneText);

            string scriptPath = Path.Combine(Application.dataPath, "Scripts/Training 1/main.py");
            PythonRunner.RunFile(scriptPath);

            //Destoy previous population
            for (var i = 0; i < populationSize; i++)
            {   
                Destroy(players[i]);
                Destroy(balls[i]);
            }

            var lines = File.ReadAllLines(input_path);
            for (var i = 0; i < populationSize; i++)
            {
                genes[i] = new Gene(geneShape, generation);
                genes[i].fromText(lines[i]);

                players[i] = Instantiate(player, new Vector3(10, 0, 0), Quaternion.identity);
                players[i].GetComponent<PlayerTraining1>().gene = genes[i];
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
    public float score;
    float[] gene; // an array of all the weights


    public Gene(int[] geneShape, int generation) {
        // Create the gene and calculate the network shape
        this.geneShape = geneShape;
        this.generation = generation;
        // Move this outside of gene creation
        /*
        for (var i = 0; i < geneShape.Length-1; i++) {
            netShape[i] = new int[] { this.geneShape[i], this.geneShape[i + 1] };
        }
        */
    }

    public void fromText(string fileText) {
        // Stores the input text string as an 1D array of floats inside the gene
        string[] geneText = fileText.Split(' ');
        gene = new float[geneText.Length];

        for (var i=0; i<geneText.Length; i++) {
            gene[i] = float.Parse(geneText[i]);
        }
    }
    
    public float[] feedForward(float[] gameState) {
        // store a value for each node
        float[] nodeVals = new float[geneShape.Sum()];

        int filledNodeCount = 0; // number of nodes with filled values
        int passedEdges = 0; // number of used weights
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
                    // Identifies the corresponding node value to get
                    // filledNodeCount - n is the first node in the next layer
                    int prevNodeIndex = filledNodeCount - n  - nodesInPrev + p;
                    nodeVal += gene[passedEdges] * nodeVals[prevNodeIndex];
                    passedEdges++;
                }
                nodeVals[filledNodeCount] = nodeVal;
                filledNodeCount++;
            }
        }

        return nodeVals.Skip(nodeVals.Length - geneShape.Last()).ToArray();
    }

    public override string ToString() {
        string geneText = string.Join(" ", gene) + " " + score.ToString();
        return geneText;
    }
}

