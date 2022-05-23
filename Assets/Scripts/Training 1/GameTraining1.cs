using UnityEngine;
using System.IO;
using UnityEditor.Scripting.Python;
using UnityEditor;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameTraining1 : MonoBehaviour
{
    private float genStartTime = 0f;
    private float genTime = 5f;
    private int populationSize = 250;
    //private int iterationSize = 250;
    private int iterationLimit = 3;
    private int iterationNum = 0;
    private int timeScale = 1;
    private Gene[] genes;
    private int generation = 0;
    private int[] geneShape = new int[] {3, 3, 4, 4};


    public GameObject player;
    public GameObject ball;
    private GameObject[] players;
    private GameObject[] balls;

    string starting_path = @"Data/starting.txt";
    string output_path = @"Data/result.txt";
    string input_path = @"Data/nextGen.txt";
    string bestGen_path = @"Data/best.txt";
    string score_path = @"Data/scores.txt";
    string python_path = @"Scripts/Training 1/main.py";


    private bool isRunning = false;

    private void Start()
    {
        Time.timeScale = timeScale;
        genes = new Gene[populationSize];
        players = new GameObject[populationSize];
        balls = new GameObject[populationSize];

        string scriptPath = Path.Combine(Application.dataPath, "Scripts/Training 1/setup.py");
        PythonRunner.RunFile(scriptPath);

        var lines = File.ReadAllLines(starting_path);

        for (var i = 0; i < populationSize; i++)
        {
            genes[i] = new Gene(geneShape, generation);
            genes[i].fromText(lines[i]);
        }

        // Generate random params

        createIteration();

        genStartTime = Time.time;
    }

    private void Update()
    {

        if (Time.time - genStartTime > genTime && !isRunning) {
            iterationNum += 1;
            if (iterationNum >= iterationLimit) {
                float totalScore = 0;
                isRunning = true;
                string[] geneText = new string[populationSize];
                for (var i = 0; i < populationSize; i++) {
                    genes[i].score += balls[i].GetComponent<BallTraining1>().score;
                    totalScore += genes[i].score;
                }
                for (var i = 0; i < populationSize; i++) {
                    geneText[i] = genes[i].ToString();
                }
                File.WriteAllLines(output_path, geneText);
                generation += 1;

                Debug.Log("Generation " + generation + " complete. Score: " + totalScore);
                
                string scriptPath = Path.Combine(Application.dataPath, python_path);
                PythonRunner.RunFile(scriptPath, "base");
                //PythonRunner.RunFile(scriptPath, "adaptiveMutation");
                //PythonRunner.RunFile(scriptPath, "dynamicMutation");
                //PythonRunner.RunFile(scriptPath, "tournamentCrossover");
                //PythonRunner.RunFile(scriptPath, "WeightedCrossover");


                //Destoy previous population
                for (var i = 0; i < populationSize; i++) {
                    Destroy(players[i]);
                    Destroy(balls[i]);
                }

                var lines = File.ReadAllLines(input_path);
                iterationNum = 0;
                // generate new population array from file
                for (var i = 0; i < populationSize; i++) {
                    genes[i] = new Gene(geneShape, generation);
                    genes[i].fromText(lines[i]);
                }

                createIteration();

                genStartTime = Time.time;
                isRunning = false;
            } 
            else {
                for (var i = 0; i < populationSize; i++) {
                    genes[i].score += balls[i].GetComponent<BallTraining1>().score;
                }
                for (var i = 0; i < populationSize; i++) {
                    Destroy(players[i]);
                    Destroy(balls[i]);
                }

                // create balls and players for current iteration
                createIteration();

                genStartTime = Time.time;
            }
        } 
    }

    private void createIteration() {
        for (var i = 0; i < populationSize; i++) {
            players[i] = Instantiate(player, new Vector3(Random.Range(-20f, 20f), -9f, 0), Quaternion.identity);
            players[i].GetComponent<PlayerTraining1v2>().gene = genes[i];
            balls[i] = Instantiate(ball, new Vector3(0, 0, 2), Quaternion.identity);
            players[i].GetComponent<PlayerTraining1v2>().setBall(balls[i]);
        }
    }
}

public class Gene
{
    int generation;
    int[] geneShape;
    public float score = 1;
    public float allScore = 0;
    float[] gene; // an array of all the weights


    public Gene(int[] geneShape, int generation) {
        // Create the gene and calculate the network shape
        this.geneShape = geneShape;
        this.generation = generation;
    }

    public void fromText(string fileText) {
        // Stores the input text string as an 1D array of floats inside the gene
        string[] geneText = fileText.Split(' ');
        gene = new float[geneText.Length];

        for (var i=0; i<geneText.Length; i++) {
            gene[i] = float.Parse(geneText[i]);
        }
    }

    public float sigmoid(float x) {
        // the exact natural constant e is hard to directly import and using a simpler number doesn't
        // cause any significant effect
        return (1 - Mathf.Pow(3.17f, -x)) / (1 + Mathf.Pow(3.17f, -x));
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

        int prevNodeStart = 0;
        for (var nextLayer=1; nextLayer<geneShape.Length; nextLayer++) {
            // Go through each next layer,
            // calculate the weighted sums as each node value
            /*
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
                // TODO: activation function
                if (filledNodeCount >= geneShape[0]) {
                    // if this node is not on the last output layer
                    nodeVal = sigmoid(nodeVal);
                }
                nodeVals[filledNodeCount] = nodeVal;
                filledNodeCount++;
            }
            */
            int prevLayerSize = geneShape[nextLayer - 1];
            for (int n=0; n<geneShape[nextLayer]; n++) {
                float nodeVal = 0;
                for (int i=0; i<prevLayerSize; i++) {
                    nodeVal += nodeVals[prevNodeStart + i] * gene[passedEdges];
                    passedEdges++;
                }
                nodeVal += gene[passedEdges];
                passedEdges++;
                nodeVal = sigmoid(nodeVal);
                nodeVals[filledNodeCount] = nodeVal;
                filledNodeCount++;
            }

            prevNodeStart += geneShape[nextLayer];
        }

        

        return nodeVals.Skip(nodeVals.Length - geneShape.Last()).ToArray();
    }

    public override string ToString() {
        string geneText = string.Join(" ", gene) + " " + score.ToString();
        return geneText;
    }
}