#import time
import random

def calculateNextGen():
    result = []
    data = []
    with open('assets/data.txt', 'r') as f:
        for line in f:
            data.append(eval(line))
    
    populationSize = len(data)
            
    
    data.sort(key=lambda x: x[-1])
    good = data[:int(len(data)*0.3)]
    bad = data[int(len(data)*0.3):]

    # top
    top = [g[0:2] for g in good]
    result += top

    # top combination
    top_comb = []
    for i in range(0, int(populationSize*0.4)):
        couple = random.sample(top, 2)
        direction = (couple[0][0] + couple[1][0]) / 2
        force = (couple[0][1] + couple[1][1]) / 2
        top_comb.append([direction, force])
    result += top_comb

    # bottom combination
    bottom_comb = []
    for i in range(0, int(populationSize*0.05)):
        couple = random.sample(bad, 2)
        direction = (couple[0][0] + couple[1][0]) / 2
        force = (couple[0][1] + couple[1][1]) / 2
        bottom_comb.append([direction, force])
    result += bottom_comb

    # random
    r = []
    for i in range(0, populationSize-len(result)):
        direction = random.randint(0, 360)
        force = 0.2 + random.random() * 0.6
        r.append([direction, force])
    result += r
    
    result = ['{} {:.4f}'.format(g[0], g[1]) for g in result]

    return result


def main():
    nextGen = calculateNextGen()
    # Write next gen
    with (open("assets/nextGen.txt", "w")) as f:
        for gene in nextGen:
            f.write(gene + '\n')
        f.close()

main()
