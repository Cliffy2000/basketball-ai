#import time
import random

def calculateNextGen():
    nextGen = []
    for i in range(0, 50):
        direction = 315
        force = 0.2 + random.random() * 0.2
        gene = '{} {:.4f}'.format(direction, force)
        
        nextGen.append(gene)
    return nextGen


def main():
    nextGen = calculateNextGen()

    # Write next gen
    with (open("assets/nextGen.txt", "w")) as f:
        for gene in nextGen:
            f.write(gene + '\n')
        f.close()

main()
