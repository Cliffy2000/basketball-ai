import random
from unittest import result
import copy

topPercent = 0.3
maxMutatePercent = 0.4
# The size of each population
popSize = 60
geneSize = 2
# maximum and minimum score of a gene, used also to calculate performance
minScore = 1
maxScore = 100
# geneShape is an array indicating the number of nodes in each layer including input and output
geneShape = [1, 3, 2]
# netShape is a 2D array that shows the shape of the array of weights between neighboring layers
netShape = [[geneShape[i],geneShape[i+1]] for i in range(len(geneShape)-1)]

# this path is for the code to run within unity
resultPath = '../../../Data/result.txt'
nextGenPath = '../../../Data/nextGen.txt'


def randomGene():
    # Creates a random gene according to netShape
    gene = []
    for dimFront,dimBack in netShape:
        gene.append([[random.random() for b in range(dimBack)] for f in range(dimFront)])
    return gene


def initializePopulation():
    population = []
    for g in range(popSize):
        population.append(randomGene())
    return population


def writePopulation(population, path=nextGenPath, score=''):
    with open(path, "w+") as f:
        for gene in population:
            geneText = ''
            for layer in gene:
                for node in layer:
                    geneText += ' '.join([format(item, '.3f') for item in node]) + ' '
            f.write(geneText + score + '\n')
    f.close()


def readPopulation(path=resultPath):
    population = []
    with open(path, 'r') as f:
        result = f.readlines()
        for line in result:
            line = line.split()
            score = line.pop(-1)
            gene = []
            for dimFront, dimBack in netShape:
                gene.append([[float(line.pop(0)) for b in range(dimBack)] for f in range(dimFront)])
            population.append([gene, score])
    return population


def createNextGen(population):
    # must take in processed population from readPopulation()
    population.sort(key=lambda x:x[-1], reverse=True)
    scores = [max(float(g[-1]), minScore) for g in population]
    genes = [g[0] for g in population]
    newPopulation = []

    newPopulation += genes[:int(popSize*topPercent)]
    mutationRate = (1 - sum(scores)/(maxScore*popSize)) * maxMutatePercent

    for i in range(int(popSize * mutationRate)):
        newPopulation.append(randomGene())
    
    crossCount = popSize - len(newPopulation)
    for i in range(crossCount):
        parent1, parent2 = random.choices(genes, k=2, weights=scores)
        child = copy.deepcopy(parent1)
        
        for i in range(len(parent1)):
            # parent1[i] is a layer
            for j in range(len(parent1[i])):
                # parent1[i][j] is the list of edges originating from the j'th node on the i'th layer
                for k in range(len(parent1[i][j])):
                    # parent[i][j][k] is the weight(number) of the k'th edge originating from the j'th node on the i'th layer
                    child[i][j][k] = (parent1[i][j][k] + parent2[i][j][k]) / 2
        
        newPopulation.append(child)

    return newPopulation        


'''
data = readData()
newGen = createNewGen(data)
newGenText = ['{} {:.4f}'.format(g[0], g[1]) for g in newGen]
writeData(newGenText)
'''
initial = initializePopulation()
writePopulation(initial, nextGenPath, score="50")
p = readPopulation(nextGenPath)
p1 = createNextGen(p)
writePopulation(p1, path="../../../Data/testfile.txt")