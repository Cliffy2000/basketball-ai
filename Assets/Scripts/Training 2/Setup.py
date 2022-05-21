import os
import random
from datetime import datetime

pathPrefix = ''
dataFolder = 'Data/Training 2/'


if __name__ == '__main__' or __name__ == '__main__':
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
