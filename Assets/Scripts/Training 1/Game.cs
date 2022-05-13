using UnityEngine;
using System.IO;
using UnityEditor.Scripting.Python;
using UnityEditor;
using System.Linq;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    private float genStartTime = 0f;
    private float genTime = 7f;
    private int populationSize = 75;
    private int timeScale = 1;
    private Gene[] genes;
    private int generation = 1;
    private int[] geneShape = new int[] {2,4,3};
    /*  Task: Score while the movement of the player is constant speed
     *  Current gene shape ~ game:
     *  Input Layer: posX of the player
     *  Output Layer: shootAngle, shootForce, shootTrigger
     */
    private int[] newShape;
    private float topScore = 0;

    public GameObject player;
    public GameObject ball;
    private GameObject[] players;
    private GameObject[] balls;

    string starting_path = @"Data/starting.txt";
    string output_path = @"Data/result.txt";
    string input_path = @"Data/nextGen.txt";
    string bestGen_path = @"Data/best.txt";
    string score_path = @"Data/scores.txt";


    private bool isRunning = false;

    private void Start()
    {
        Time.timeScale = timeScale;
        genes = new Gene[populationSize];
        players = new GameObject[populationSize];
        balls = new GameObject[populationSize];

        string scriptPath = Path.Combine(Application.dataPath, "Scripts/Training 1/setup.py");
        PythonRunner.RunFile(scriptPath, "unity");

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
            players[i] = Instantiate(player, new Vector3(Random.Range(15, 25), 0, 0), Quaternion.identity);
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
            string t = "Generation: " + generation + " Score: " + totalScore + "\n";
            File.AppendAllText(score_path, t);
            if (totalScore > topScore) {
                File.WriteAllText(bestGen_path, t);
                File.AppendAllLines(bestGen_path, geneText);
                topScore = totalScore;
            }
            generation += 1;
            string scriptPath = Path.Combine(Application.dataPath, "Scripts/Training 1/main.py");
            PythonRunner.RunFile(scriptPath, "unity");

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

                players[i] = Instantiate(player, new Vector3(Random.Range(15, 25), 0, 0), Quaternion.identity);
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
    public float score = 1f;
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
                // TODO: hard coded index change to index -1
                if (filledNodeCount > geneShape[0] && filledNodeCount <= (geneShape[0]+geneShape[1])) {
                    // Activation function
                    if (nodeVal < 0) nodeVal = 0;
                }
                // TODO: activation function
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

