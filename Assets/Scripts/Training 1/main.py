from curses import keyname
import random
from unittest import result

topPercent = 0.3
maxMutatePercent = 0.4
# The size of each population
popSize = 60
geneSize = 2
# maximum score of a gene, used also to calculate performance
maxScore = 100
# geneShape is an array indicating the number of nodes in each layer including input and output
geneShape = [1, 3, 2]
# netShape is a 2D array that shows the shape of the array of weights between neighboring layers
netShape = [[geneShape[i],geneShape[i+1]] for i in range(len(geneShape)-1)]

# this path is for the code to run within unity
resultPath = '../../../Data/result.txt'
nextGenPath = '../../../Data/nextGen.txt'


def readData():
    data = []
    with open(resultPath, 'r') as f:
        for line in f:
            data.append(eval(line))
        f.close()
    popSize = len(data)
    return data


def createNewGen(data):
    # split the data into two parts
    data.sort(key=lambda x: x[-1], reverse=True)
    splitIndex = int(topPercent*len(data))
    top, bottom = data[:splitIndex], data[splitIndex:]

    newGen = []
    newGen += [gene[:geneSize] for gene in top]

    # allows the mutation proportion to change according to the performance in a simple manner
    # potential change to more complex changing curve
    scores = [gene[-1] for gene in data]
    totalScorePercent = sum(scores) / (popSize * maxScore)
    mutatePercent = min(1 - totalScorePercent, maxMutatePercent)
    mutateCount = int(mutatePercent * popSize)

    for i in range(mutateCount):
        newGen.append([random.randint(0, 360), 0.2 + random.random() * 0.6])
    
    for i in range(popSize - mutateCount - splitIndex):
        # crosses the genes by selecting with the score as probabilities
        x1, x2 = random.choices(data, k = 2, weights = scores)
        newGen.append([(x1[0] + x2[0])/2, (x1[1] + x2[1])/2])
    
    return newGen


def writeData(newGenText):
    with (open(nextGenPath, "a+")) as f:
        for gene in newGenText:
            f.write(gene + '\n')
        f.close()


def initializePopulation():
    population = []
    for g in range(popSize):
        gene = []
        for dimFront,dimBack in netShape:
            gene.append([[format(random.random(),'.3f') for b in range(dimBack)] for f in range(dimFront)])
        population.append(gene)
    return population


def writePopulation(population, path=nextGenPath):
    with open(path, "w+") as f:
        for gene in population:
            geneText = ''
            for layer in gene:
                for node in layer:
                    geneText += ' '.join(node) + ' '
            f.write(geneText + '\n')
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
    newPopulation = []
    


'''
data = readData()
newGen = createNewGen(data)
newGenText = ['{} {:.4f}'.format(g[0], g[1]) for g in newGen]
writeData(newGenText)
'''
