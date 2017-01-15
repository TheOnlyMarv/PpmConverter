using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegConverter.Quantisation
{
    public static class Quantisation
    {
        private static int BLOCK_SIZE = 8;
        private static int[,] quantisationTableForLuminance = {
            {16,11,10,16,24,40,51,61},
            {12,12,14,19,26,58,60,55},
            {14,15,16,24,40,57,69,56},
            {14,17,22,29,51,85,80,62},
            {18,22,37,56,68,109,103,77},
            {24,35,55,64,81,104,113,92},
            {49,64,78,87,103,121,120,101},
            {72,92,95,98,112,100,103,99},
        };
        private static int[,] quantisationTableForChrominance = {
            {17,18,24,47,99,99,99,99},
            {18,21,26,66,99,99,99,99},
            {24,26,56,99,99,99,99,99},
            {47,66,99,99,99,99,99,99},
            {99,99,99,99,99,99,99,99},
            {99,99,99,99,99,99,99,99},
            {99,99,99,99,99,99,99,99},
            {99,99,99,99,99,99,99,99},
        };

        public static int[] QuantisationTableForLuminanceZickZack 
        {
            get { return CreateZickZackSorting(quantisationTableForLuminance); }
        }
        public static int[] QuantisationTableForChrominanceZickZack
        {
            get { return CreateZickZackSorting(quantisationTableForChrominance); }
        }

        public static List<int[]> CreateZickZackSortingCompleteChannel(int[,] channel)
        {
            int[] result = new int[BLOCK_SIZE * BLOCK_SIZE];
            List <int[]> resultList = new List<int[]>();

            List<int[,]> splitted = SplitImageIntoBlocks(channel);
            foreach (int[,] block in splitted)
            {
                resultList.Add(CreateZickZackSorting(block));
            }
            return resultList;
        }

        private static List<int[,]> SplitImageIntoBlocks(int[,] image)
        {
            List<int[,]> result = new List<int[,]>();
            for (int allX = 0; allX < image.GetLength(0); allX += BLOCK_SIZE)
            {
                for (int allY = 0; allY < image.GetLength(1); allY += BLOCK_SIZE)
                {
                    int[,] block = new int[BLOCK_SIZE, BLOCK_SIZE];
                    for (int x = 0; x < BLOCK_SIZE; x++)
                    {
                        for (int y = 0; y < BLOCK_SIZE; y++)
                        {
                            block[x, y] = image[allX + x, allY + y];
                        }
                    }
                    result.Add(block);
                }
            }
            return result;
        }

        public static int[] CreateZickZackSorting(int[,] block)
        {
            int[] result = new int[BLOCK_SIZE * BLOCK_SIZE];
            int direction = 1;
            int i = 0;
            int j = 0;

            for(int k = 0; k <64; k++)
            {
                result[k] = block[j, i];
                i = i + direction;
                j = j - direction;
                if( j < 0)
                {
                    j = 0;
                    direction = -direction; //change direction
                }
                else if(i < 0)
                {
                    if(j > 7)   //outside left and bottom
                    {
                        j = 7;
                        i = i + 2;
                    }
                    else
                    {
                        i = 0;  //outside left
                    }
                    direction = -direction;
                }
                else if(i > 7) //outside right;
                {
                    i = 7;
                    j = j + 2;
                    direction = -direction;
                }
                else if(j > 7) //outside bottom
                {
                    j = 7;
                    i = i + 2;
                    direction = -direction;
                }
            }
            return result;
        }

        public static int[,] RunQuantisation(int[,] image, bool useChrominance = false)
        {
            int[,] newImage = new int[image.GetLength(0), image.GetLength(1)];

            int blocksEachRow = image.GetLength(1) / BLOCK_SIZE;
            for (int bId = 0; bId < (image.GetLength(0) * image.GetLength(1)) / BLOCK_SIZE / BLOCK_SIZE; bId++)
            {
                int offsetX = (bId % blocksEachRow) * 8;
                int offsetY = (bId / blocksEachRow) * 8;

                QuantisationForOneBlock(image, offsetX, offsetY, newImage, useChrominance ? quantisationTableForChrominance : quantisationTableForLuminance);
            }


            return newImage;
        }

        private static void QuantisationForOneBlock(int[,] image, int offsetX, int offsetY, int[,] newImage, int[,] quantisationTable)
        {
            for (int y = 0; y < BLOCK_SIZE; y++)
            {
                int realY = y + offsetY;
                for (int x = 0; x < BLOCK_SIZE; x++)
                {
                    int realX = x + offsetX;
                    newImage[realY, realX] = (int)Math.Round(image[realY, realX] / (double)quantisationTable[y, x]);
                }
            }
        }
    }
}
