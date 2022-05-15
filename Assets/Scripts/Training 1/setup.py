import os
import random

topPercent = 0.25
maxMutatePercent = 0.4
# The size of each population
popSize = 75
geneSize = 2
# maximum and minimum score of a gene, used also to calculate performance
maxScore = 100
# geneShape is an array indicating the number of nodes in each layer including input and output
geneShape = [2, 4, 4, 3]
# netShape is a 2D array that shows the shape of the array of weights between neighboring layers
netShape = [[geneShape[i],geneShape[i+1]] for i in range(len(geneShape)-1)]

folderPath = "Data/"
startingFile = "starting.txt"
content = os.listdir(folderPath)

def randomGene():
    # creates a gene with random edge weights as a 1d list
    gene = [random.random() for i in range(sum([m*n for m,n in netShape]))]
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