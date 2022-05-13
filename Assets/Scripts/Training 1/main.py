import random
import os

topPercent = 0.25
maxMutatePercent = 0.4
# The size of each population
popSize = 75
geneSize = 2
# maximum and minimum score of a gene, used also to calculate performance
maxScore = 100
# geneShape is an array indicating the number of nodes in each layer including input and output
geneShape = [2, 4, 3]
# netShape is a 2D array that shows the shape of the array of weights between neighboring layers
netShape = [[geneShape[i],geneShape[i+1]] for i in range(len(geneShape)-1)]

# this path is for the code to run within unity
resultPath = 'Data/result.txt'
nextGenPath = 'Data/nextGen.txt'


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


def writePopulation(population, path=nextGenPath, score=''):
    with open(path, "w+") as f:
        for gene in population:
            geneText = ' '.join([format(weight, '.3f') for weight in gene])
            f.write(geneText + score + '\n')
    f.close()


def readPopulation(path=resultPath):
    population = []
    with open(path, 'r') as f:
        result = f.readlines()
        for line in result:
            gene = line.split()
            score = gene.pop(-1)
            population.append([gene, score])
    f.close()
    return population


def createNextGen(population):
    # must take in processed population from readPopulation()
    population.sort(key=lambda x:float(x[-1]), reverse=True)
    scores = [float(g[-1]) for g in population]
    genes = [[float(g) for g in gene[0]] for gene in population]
    newPopulation = []

    newPopulation += genes[:int(popSize*topPercent)]
    mutationRate = (1 - sum(scores)/(maxScore*popSize)) * maxMutatePercent # controls the portion of mutation

    # for every mutation count, add a new individual
    for i in range(int(popSize * mutationRate)):
        newPopulation.append(randomGene())
    
    crossCount = popSize - len(newPopulation)
    for i in range(crossCount):
        parent1, parent2 = random.choices(genes[:int(popSize*topPercent)], k=2, weights=scores[:int(popSize*topPercent)])        
        newPopulation.append([(m+n)/2 for m,n in zip(parent1, parent2)])

    return newPopulation        


'''
data = readData()
newGen = createNewGen(data)
newGenText = ['{} {:.4f}'.format(g[0], g[1]) for g in newGen]
writeData(newGenText)
'''

if __name__ == 'unity':
    data = readPopulation()
    newGeneration = createNextGen(data)
    writePopulation(newGeneration)