using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor.Scripting.Python;
using System.Linq;

public class GameTraining2 : MonoBehaviour
{
    private float genTime = 8f;
    private float genStartTime = 0f;
    private int populationSize = 300;
    private int iterationSize = 100;
    private int iterationNum = 0;

    private GeneV2[] genes;
    private int[] geneShape = new int[] { 3, 4, 4, 4 };
    private int generation = 0;

    public GameObject player;
    public GameObject ball;
    public GameObject[] players;
    public GameObject[] balls;

    private string initialPath = @"Data/initial.txt";
    private string outputPath = @"Data/result.txt"; // C# output for Python
    private string nextGenPath = @"Data/nextGen.txt"; // C# reads in

    // Used to make sure Unity waits for Python to finish
    private bool runUnity = true;

    // Start is called before the first frame update
    private void Start() {
        string scriptPath = Path.Combine(Application.dataPath, "Scripts/Training 1/Setup.py");
        PythonRunner.RunFile(scriptPath, "unity");

        newGeneration(initialPath);
        newIteration();

        genStartTime = Time.time;
    }

    // Update is called once per frame
    private void Update() {
        if (Time.time - genStartTime > genTime && runUnity) {
            iterationNum++;
            
            if (iterationNum * iterationSize >= populationSize) {
                // stops unity and completes Python process
                runUnity = false;
            }

            else {
                // continue to next iteration
            }
        }
    }

    private void newIteration() {
        players = new GameObject[iterationSize];
        balls = new GameObject[iterationSize];
        for (var i = 0; i < iterationSize; i++) {
            players[i + (iterationSize * iterationNum)] = Instantiate(player, new Vector3(Random.Range(20, 25), 0, 0), Quaternion.identity);
            players[i + (iterationSize * iterationNum)].GetComponent<PlayerTraining2>().gene = genes[i + (iterationSize * iterationNum)];
            balls[i + (iterationSize * iterationNum)] = Instantiate(ball, new Vector3(0, 0, 0), Quaternion.identity);
            players[i + (iterationSize * iterationNum)].GetComponent<PlayerTraining2>().setBall(balls[i + (iterationSize * iterationNum)]);
        }
    }

    private void newGeneration(string filePath) {
        // read in from file and creates entire population of genes
        genes = new GeneV2[populationSize];

        var data = File.ReadAllLines(filePath);
        for (var i = 0; i < populationSize; i++) {
            genes[i] = new GeneV2();
            genes[i].setGeneShape(geneShape);
            genes[i].fromText(data[i]);
        }
    }
}


public class GeneV2 {
    int[] geneShape;
    public float score = 1;
    float[] gene; // an array of all the weights

    public void setGeneShape(int[] geneShape) {
        this.geneShape = geneShape;
    }

    public void fromText(string fileText) {
        // Stores the input text string as an 1D array of floats inside the gene
        string[] geneText = fileText.Split(' ');
        gene = new float[geneText.Length];

        for (var i = 0; i < geneText.Length; i++) {
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
        for (var i = 0; i < geneShape[0]; i++) {
            // Insert the gameState into nodeVals as first layer
            nodeVals[i] = gameState[i];
            filledNodeCount++;
        }

        for (var nextLayer = 1; nextLayer < geneShape.Length; nextLayer++) {
            // Go through each next layer,
            // calculate the weighted sums as each node value
            int nodesInNext = geneShape[nextLayer];
            int nodesInPrev = geneShape[nextLayer - 1];
            // Calculate the value of each node in the next layer
            for (var n = 0; n < nodesInNext; n++) {
                float nodeVal = 0;
                // sum the weighted values from the previous layer
                for (var p = 0; p < nodesInPrev; p++) {
                    // Identifies the corresponding node value to get
                    // filledNodeCount - n is the first node in the next layer
                    int prevNodeIndex = filledNodeCount - n - nodesInPrev + p;
                    nodeVal += gene[passedEdges] * nodeVals[prevNodeIndex];
                    passedEdges++;
                }
                // TODO: activation function
                if ((nodeVals.Length - filledNodeCount > geneShape[geneShape.Length - 1]) &&
                    (filledNodeCount >= geneShape[0])) {
                    // if this node is not on the last output layer
                    nodeVal = sigmoid(nodeVal);
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
