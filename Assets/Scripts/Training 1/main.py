import random
import os
import statistics as st

# The size of each population
popSize = 600
# maximum and minimum score of a gene, used also to calculate performance
maxScore = 200
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

def nextGen_basic(old_population, scores):
    '''
    Selection: gets rid of the bottom 1/2 of the population
    Crossover: randomly selects two parents and creates two children with a random crossover point, 
        this is repeated len(new_population) * crossoverProbability / 2 times
    Mutation: for each gene, mutate at the probability of mutationProbability, 
        if do mutate, randomly choose two values and replace them by a new random value
    '''
    mutationProbability = 0.25
    crossoverProbability = 0.65
    # Selection, only keeping the top 1/2 of the population
    new_population = old_population[:popSize//2]
    
    # Crossover: crossoverProbability means the proportion of the old_population
    # that was kept that will be mutated
    # # It is divided by 2 because each crossover takes 2 parents
    for i in range(int(len(new_population) * crossoverProbability / 2)):
        # Randomly select two parents
        parent1 = random.choice(old_population)
        parent2 = random.choice(old_population)
        # Crossover the two parents
        # Randomly select a crossover point
        crossPoint = random.randint(0, len(parent1)-1)
        # Create the child
        child1 = parent1[:crossPoint] + parent2[crossPoint:]
        child2 = parent2[:crossPoint] + parent1[crossPoint:]
        # Add the child to the new population
        new_population.append(child1)
        new_population.append(child2)
    print(int(len(new_population) * crossoverProbability / 2))
    # Mutation: mutationProbability means the proportion of the new_population
    # that will be mutated. This new_population contains the newly created crossover children
    for i in range(len(new_population)):
        rand_posibility = random.random()
        if rand_posibility < mutationProbability:
            # Mutate the gene by randomly choosing 8 values and replacing them with new random values
            replace_index = random.choices(range(len(new_population[i])), k=8)
            new_population[i][replace_index[0]] = (random.random()-0.5)*2
            new_population[i][replace_index[1]] = (random.random()-0.5)*2

    # Fill the rest of the population with random genes
    for i in range(len(new_population), popSize):
        new_population.append(randomGene())
    
    print(len(new_population))
    return new_population

def nextGen_adaptive_mutation(old_population, scores):
    '''
    Selection: gets rid of the bottom 1/2 of the population
    Crossover: randomly selects two parents and creates two children with a random crossover point, 
        this is repeated len(new_population) * crossoverProbability / 2 times
    Mutation: for each gene, mutate at the probability of mutationProbability ACCORDING TO THE SCORE, 
        The mutation probability is evenly distributed across the population, between minMP and maxMP
        if do mutate, randomly choose two values and replace them by a new random value
    '''
    minMP = 0.2 # minimum mutation probability
    maxMP = 0.7    # maximum mutation probability
    MP_interval = (maxMP - minMP) / popSize
    mutationProbability = [minMP + i*MP_interval for i in range(popSize)]
    crossoverProbability = 1
    # Selection, only keeping the top 1/3 of the population
    new_population = old_population[:popSize//1.5]
    
    # Crossover: crossoverProbability means the proportion of the old_population
    # that was kept that will be mutated
    # # It is divided by 2 because each crossover takes 2 parents
    for i in range(int(len(new_population) * crossoverProbability / 2)):
        # Randomly select two parents
        parent1 = random.choice(old_population)
        parent2 = random.choice(old_population)
        # Crossover the two parents
        # Randomly select a crossover point
        crossPoint = random.randint(0, len(parent1)-1)
        # Create the child
        child1 = parent1[:crossPoint] + parent2[crossPoint:]
        child2 = parent2[:crossPoint] + parent1[crossPoint:]
        # Add the child to the new population
        new_population.append(child1)
        new_population.append(child2)

    # Fill the rest of the population with random genes
    for i in range(len(new_population), popSize):
        new_population.append(randomGene())

    # Mutation: mutationProbability means the proportion of the new_population
    # that will be mutated. This new_population contains the newly created crossover children
    for i in range(len(new_population)):
        rand_posibility = random.random()
        if rand_posibility < mutationProbability[i]:
            # Mutate the gene by randomly choosing 10 values and replacing them with new random values
            replace_index = random.choices(range(len(new_population[i])), k=10)
            new_population[i][replace_index[0]] = (random.random()-0.5)*2
            new_population[i][replace_index[1]] = (random.random()-0.5)*2
    
    print(len(new_population))
    return new_population

def nextGen_weightedCrossOver(data):
    '''
    Crossover selector: Select each pair using the scores of the genes as weights.
    Crossover operator: Swap half of the parents to form "ABC456" and "123DEF" children.
    '''
    crossoverProbability = 0.35 # implicit selection
    data.sort(key=lambda x: float(x[1]), reverse=True)
    genes = [d[0] for d in data]
    scores = [d[1] for d in data]
    newGen = []

    for i in range(int(popSize * crossoverProbability / 2)):
        # Choose two parents based on their performance
        parent1, parent2 = random.choices(genes, k=2, weights=scores)
        child1, child2 = [], []
        # newGen.append(parent1)
        # newGen.append(parent2)
        for count in netShape:
            # randomly determine if the two parents exchange the layer
            swap = (random.random() > 0.8)
            if swap:
                child1 += parent1[len(child1):len(child1)+count]
                child2 += parent2[len(child2):len(child2)+count]
            else:
                child1 += parent2[len(child1):len(child1)+count]
                child2 += parent1[len(child2):len(child2)+count]
        newGen.append(child1)
        newGen.append(child2)
    
    for i in range(len(newGen)):
        if (random.random() > 0.9):
            n = random.choices(range(len(newGen[i])), k=1)
            for num in n:
                newGen[i][num] = (random.random()-0.5)*2

    # Mutate the rest of the population by creating random genes
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