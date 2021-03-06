import os
import random

# The size of each population
popSize = 250
# maximum and minimum score of a gene, used also to calculate performance
maxScore = 100
# geneShape is an array indicating the number of nodes in each layer including input and output
geneShape = [3, 3, 4, 4]
# netShape is a 2D array that shows the shape of the array of weights between neighboring layers
netSize = sum([geneShape[i]*geneShape[i+1] for i in range(len(geneShape)-1)]) + sum(geneShape[1:])

folderPath = "Data/"
startingFile = "starting.txt"
content = os.listdir(folderPath)

def randomGene():
    # creates a gene with random edge weights as a 1d list
    gene = [(random.random()-0.5)*2 for i in range(netSize)]
    return gene


def initializePopulation():
    # returns a list of genes as a 2d list
    population = []
    for g in range(popSize):
        population.append(randomGene())
    return population


def writePopulation(population, path, score=''):
    with open(path, "w+") as f:
        for gene in population:
            geneText = ' '.join([format(weight, '.3f') for weight in gene])
            f.write(geneText + score + '\n')
    f.close()

f = open(folderPath+startingFile, 'w')

writePopulation(initializePopulation(), folderPath+startingFile)
f.close()