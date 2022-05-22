import os
import random
from datetime import datetime

pathPrefix = ''
dataFolder = 'Data/Training 2/'

folderPath = "Data/"
startingFile = "initial.txt"
topPercent = 0.25
maxMutatePercent = 0.4
# The size of each population
popSize = 30
geneSize = 2
# maximum and minimum score of a gene, used also to calculate performance
maxScore = 100
# geneShape is an array indicating the number of nodes in each layer including input and output
geneShape = [3, 4, 4, 4]
# netShape is a 2D array that shows the shape of the array of weights between neighboring layers
netShape = [[geneShape[i],geneShape[i+1]] for i in range(len(geneShape)-1)]


def randomGene():
    # creates a gene with random edge weights as a 1d list
    gene = [(random.random()-0.5)*2 for i in range(sum([m*n for m,n in netShape]))]
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


if __name__ == '__main__' or __name__ == 'unity':
    currentTime = datetime.now()
    folderName = currentTime.strftime("%Y-%m-%d-%H%M")
    # automatically changes the path if the python file is 
    # executed from the command line
    currentPath = os.getcwd()
    if os.getcwd().endswith("basketball-ai"):
        # the code is executed from unity
        pathPrefix = ''
    elif os.getcwd().endswith("scripts"):
        pathPrefix = '../../'

    if folderName not in pathPrefix+dataFolder:
        os.mkdir(pathPrefix + dataFolder + folderName)
    
    f = open(folderPath+startingFile, 'w')

    writePopulation(initializePopulation(), folderPath+startingFile)
    f.close()
