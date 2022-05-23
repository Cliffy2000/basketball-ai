import random
import os
import statistics as st
import math

CROSSOVER_PROBABILITY = 0.6 # implicit selection, determines what proportion of the next gen comes from crossover
CROSSOVER_OPERATOR_PROBABILITY = 0.3
MUTATION_PROBABILITY = 0.2
MUTATION_OPERATOR_PROBABILITY = 0.3 # mutate each gene by how much
MUTATION_OPERATOR_MAGNITUDE = 0.2


# The size of each population
popSize = 500
# maximum and minimum score of a gene, used also to calculate performance
# geneShape is an array indicating the number of nodes in each layer including input and output
geneShape = [3, 3, 4, 4]
# netShape is a 2D array that shows the shape of the array of weights between neighboring layers
netSize = sum([geneShape[i]*geneShape[i+1] for i in range(len(geneShape)-1)]) + sum(geneShape[1:])
netShape = [geneShape[i]*geneShape[i+1]+geneShape[i+1] for i in range(len(geneShape)-1)]

# this path is for the code to run within unity
resultPath = 'Data/result.txt'
nextGenPath = 'Data/nextGen.txt'
result_comparePath = 'Data/result_compare.txt'
reportPath = 'Data/report.txt'


def randomGene():
    # creates a gene with random edge weights as a 1d list
    gene = [(random.random()-0.5)*2 for i in range(netSize)]
    return gene

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
            population.append([[float(g) for g in gene], float(score)])
    f.close()
    return population

def log(data, path=reportPath):
    with open(path, 'a+') as f:
        f.write(data + '\n')
    f.close()

def parentDist(parent1, parent2):
    '''
    Calculates the distance between two genes
    '''
    return math.sqrt(sum([(g1-g2)**2 for g1, g2 in zip(parent1, parent2)]))

def mutation_operator(gene):
    '''
    Mutates the gene by replacing a random value with a random value
    '''
    newGene = []
    for g in gene:
        if (random.random() < MUTATION_OPERATOR_PROBABILITY):
            newG = g + random.uniform(-MUTATION_OPERATOR_MAGNITUDE, MUTATION_OPERATOR_MAGNITUDE)
            newG = min(1, max(-1, newG))
            newGene.append(newG)
        else:
            newGene.append(g)
    return newGene

def crossover_operator(parent1, parent2):
    child1 = []
    child2 = []
    # randomly determine if the two parents exchange the layer
    for count in netShape:
        # randomly determine if the two parents exchange the layer
        swap = (random.random() < CROSSOVER_OPERATOR_PROBABILITY)
        if swap:
            child1 += parent1[len(child1):len(child1)+count]
            child2 += parent2[len(child2):len(child2)+count]
        else:
            child1 += parent2[len(child1):len(child1)+count]
            child2 += parent1[len(child2):len(child2)+count]
    return child1, child2

def nextGen_basic(data):
    '''
    Selection: gets rid of the bottom 1/2 of the population
    Crossover: randomly selects two parents and creates two children with a random crossover point, 
        this is repeated len(new_population) * crossoverProbability / 2 times
    Mutation: for each gene, mutate at the probability of mutationProbability, 
        if do mutate, randomly choose two values and replace them by a new random value
    '''
    data.sort(key=lambda x: float(x[1]), reverse=True)
    genes = [d[0] for d in data]
    scores = [d[1] for d in data]
    newGen = []

    for i in range(int(popSize * CROSSOVER_PROBABILITY / 2)):
        # Choose two parents based on their performance
        parent1, parent2 = random.choices(genes, k=2)
        child1, child2 = crossover_operator(parent1, parent2)
        newGen.append(child1)
        newGen.append(child2)
    
    for i in range(len(newGen)):
        if (random.random() < MUTATION_PROBABILITY):
            newGen[i] = mutation_operator(newGen[i])

    # Fill the rest of the population by creating random genes
    for i in range(popSize - len(newGen)):
        #log("popSize: " + popSize + ", len(newGen): " + len(newGen))
        newGen.append(randomGene())
    
    return newGen

def nextGen_adaptive_mutation(data):
    '''
    Selection: gets rid of the bottom 1/2 of the population
    Crossover: randomly selects two parents and creates two children with a random crossover point, 
        this is repeated len(new_population) * crossoverProbability / 2 times
    Mutation: for each gene, mutate at the probability of mutationProbability ACCORDING TO THE SCORE, 
        The mutation probability is evenly distributed across the population, between minMP and maxMP
        if do mutate, randomly choose two values and replace them by a new random value
    '''
    data.sort(key=lambda x: float(x[1]), reverse=True)
    genes = [d[0] for d in data]
    scores = [d[1] for d in data]
    newGen = []

    mutation_children_queue = []
    for i in range(int(popSize * CROSSOVER_PROBABILITY / 2)):
        # Choose two parents based on their performance
        parent1, parent2 = random.choices(genes, k=2)
        child1, child2 = crossover_operator(parent1, parent2)
        
        newGen.append(child1)
        mutation_children_queue.append([child2, parentDist(parent1, parent2)])
    
    mutation_children_queue.sort(key=lambda x: x[1])

    minMP = 0.2 # minimum mutation probability
    maxMP = 0.7 # maximum mutation probability
    mpInterval = (maxMP - minMP) / popSize
    for i in range(len(mutation_children_queue)):
        mutateProbability = minMP + i * mpInterval
        if (random.random() < mutateProbability):
            mutation_children_queue[i][0] = mutation_operator(mutation_children_queue[i][0])

    newGen += [gene[0] for gene in mutation_children_queue]
    
    # Fill the rest of the population by creating random genes
    for i in range(popSize - len(newGen)):
        newGen.append(randomGene())
    
    return newGen

def nextGen_weightedCrossOver(data):
    '''
    Crossover selector: Select each pair using the scores of the genes as weights.
    Crossover operator: Swap half of the parents to form "ABC456" and "123DEF" children.
    '''
    data.sort(key=lambda x: float(x[1]), reverse=True)
    genes = [d[0] for d in data]
    scores = [d[1] for d in data]
    newGen = []

    for i in range(int(popSize * CROSSOVER_PROBABILITY / 2)):
        # Choose two parents based on their performance
        parent1, parent2 = random.choices(genes, k=2, weights=scores)
        child1, child2 = crossover_operator(parent1, parent2)
        newGen.append(child1)
        newGen.append(child2)
    
    for i in range(len(newGen)):
        if (random.random() < MUTATION_PROBABILITY):
            newGen[i] = mutation_operator(newGen[i])

    # Fill the rest of the population by creating random genes
    for i in range(popSize - len(newGen)):
        newGen.append(randomGene())
    
    return newGen

def nextGen_dynamic_mutation(data):
    return nextGen_basic(data)

def nextGen_euclidian_distance(data):
    return nextGen_basic(data)

def createNextGen(population):
    # must take in processed population from readPopulation()
    
    # Sort the population by score
    population.sort(key=lambda x:float(x[-1]), reverse=True)
    scores = [float(g[-1]) for g in population]
    # Convert population to a list of genes(string -> float)
    genes =      [[float(g) for g in gene[0]] for gene in population]
    
    newPopulation = nextGen_adaptive_mutation(genes, scores)

    return newPopulation        

def evalDiversity(populationGenes):
    '''
    Calculates the diversity of each gene of the entire population by transposing 
    the entire population data array. Returns the sum of these values.
    Currently the diversity is meassured through st.dev and means
    '''
    # transposes the population gene 2d list
    populationT = list(zip(*populationGenes))
    report = [sum([st.stdev(position) for position in populationT])]
    report += [st.mean(position) for position in populationT]
    return [format(num, '.4f') for num in report]


def writeReport(data, path=reportPath):
    # Adds the generation number, score and diversity into a file 
    with open(path, 'a+') as f:
        genes = [[float(g) for g in gene[0]] for gene in data]
        totalScore = sum([float(gene[1]) for gene in data])
        report = evalDiversity(genes)
        f.write(format(totalScore, '.3f') + ' ' + ' '.join(report) + '\n')
    f.close()


'''
data = readData()
newGen = createNewGen(data)
newGenText = ['{} {:.4f}'.format(g[0], g[1]) for g in newGen]
writeData(newGenText)
'''
        
#if __name__ == 'unity' or __name__ == '__main__':
if __name__ == 'base':
    data = readPopulation()
    writeReport(data)
    newGeneration = nextGen_basic(data)
    writePopulation(newGeneration)
elif __name__ == 'adaptiveMutation':
    data = readPopulation()
    writeReport(data)
    newGeneration = nextGen_adaptive_mutation(data)
    writePopulation(newGeneration)
elif __name__ == 'dynamicMutation':
    data = readPopulation()
    writeReport(data)
    newGeneration = nextGen_dynamic_mutation(data)
    writePopulation(newGeneration)
elif __name__ == 'EuclideanDistance':
    data = readPopulation()
    writeReport(data)
    newGeneration = nextGen_euclidian_distance(data)
    writePopulation(newGeneration)
elif __name__ == 'WeightedCrossover':
    data = readPopulation()
    writeReport(data)
    newGeneration = nextGen_weightedCrossOver(data)
    writePopulation(newGeneration)