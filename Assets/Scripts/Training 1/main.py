from curses import newpad
from ntpath import join
import random
import os

# The size of each population
popSize = 75
# maximum and minimum score of a gene, used also to calculate performance
maxScore = 100
# geneShape is an array indicating the number of nodes in each layer including input and output
geneShape = [2, 4, 4, 3]
# netShape is a 2D array that shows the shape of the array of weights between neighboring layers
netShape = [[geneShape[i],geneShape[i+1]] for i in range(len(geneShape)-1)]

# this path is for the code to run within unity
resultPath = 'Data/result.txt'
nextGenPath = 'Data/nextGen.txt'
result_comparePath = 'Data/result_compare.txt'

'''
# Command line arguments:
resultPath = '../../../Data/result.txt'
nextGenPath = '../../../Data/nextGen.txt'
result_comparePath = '../../../Data/result_compare.txt'
'''


def randomGene():
    # creates a gene with random edge weights as a 1d list
    gene = [(random.random()-0.5)*2 for i in range(sum([m*n for m,n in netShape]))]
    return gene

'''
def initializePopulation():
    # returns a list of genes as a 2d list
    population = []
    for g in range(popSize):
        population.append(randomGene())
    return population
'''


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

def nextGen_basic(old_population, scores):
    '''
    Selection: gets rid of the bottom 1/2 of the population
    Crossover: randomly selects two parents and creates two children with a random crossover point, 
        this is repeated len(new_population) * crossoverProbability / 2 times
    Mutation: for each gene, mutate at the probability of mutationProbability, 
        if do mutate, randomly choose two values and replace them by a new random value
    '''
    mutationProbability = 0.1
    crossoverProbability = 0.8
    # Selection, only keeping the top 1/2 of the population
    new_population = old_population[:popSize//2]
    print(len(old_population))
    print(len(new_population))
    
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
            # Mutate the gene by randomly choosing 2 values and replacing them with new random values
            replace_index = random.choices(range(len(new_population[i])), k=2)
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
    minMP = 0.1 # minimum mutation probability
    maxMP = 0.5    # maximum mutation probability
    MP_interval = (maxMP - minMP) / popSize
    mutationProbability = [minMP + i*MP_interval for i in range(popSize)]
    print(mutationProbability)
    crossoverProbability = 0.8
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

    # Fill the rest of the population with random genes
    for i in range(len(new_population), popSize):
        new_population.append(randomGene())

    # Mutation: mutationProbability means the proportion of the new_population
    # that will be mutated. This new_population contains the newly created crossover children
    for i in range(len(new_population)):
        rand_posibility = random.random()
        if rand_posibility < mutationProbability[i]:
            # Mutate the gene by randomly choosing 2 values and replacing them with new random values
            replace_index = random.choices(range(len(new_population[i])), k=2)
            new_population[i][replace_index[0]] = (random.random()-0.5)*2
            new_population[i][replace_index[1]] = (random.random()-0.5)*2
    
    print(len(new_population))
    return new_population

def createNextGen(population):
    # must take in processed population from readPopulation()
    
    # Sort the population by score
    population.sort(key=lambda x:float(x[-1]), reverse=True)
    scores = [float(g[-1]) for g in population]
    # Convert population to a list of genes(string -> float)
    genes = [[float(g) for g in gene[0]] for gene in population]
    
    newPopulation = nextGen_adaptive_mutation(genes, scores)

    return newPopulation        


'''
data = readData()
newGen = createNewGen(data)
newGenText = ['{} {:.4f}'.format(g[0], g[1]) for g in newGen]
writeData(newGenText)
'''

if __name__ == 'unity' or __name__ == '__main__':
    data = readPopulation()

    genes = [[float(g) for g in gene[0]] for gene in data]
    writePopulation(genes, result_comparePath)
    newGeneration = createNextGen(data)
    writePopulation(newGeneration)