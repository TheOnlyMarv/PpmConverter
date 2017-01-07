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
        private static int[,] quantisationTable = {
            {50,50,50,50,50,50,50,50},
            {50,50,50,50,50,50,50,50},
            {50,50,50,50,50,50,50,50},
            {50,50,50,50,50,50,50,50},
            {50,50,50,50,50,50,50,50},
            {50,50,50,50,50,50,50,50},
            {50,50,50,50,50,50,50,50},
            {50,50,50,50,50,50,50,50},
        };

        public static int[] QuantisationTableZickZack 
        {
            get { return CreateZickZackSorting(quantisationTable); }
        }

        private static int[] CreateZickZackSorting(int[,] quantisationTable)
        {
            int[] result = new int[BLOCK_SIZE * BLOCK_SIZE];
            // TODO: Hier Zick Zack Sortierung einbringen :)
            return result;
        }

        public static int[,] RunQuantisation(double[,] image)
        {
            int[,] newImage = new int[image.GetLength(0), image.GetLength(1)];

            int blocksEachRow = image.GetLength(0) / BLOCK_SIZE;
            for (int bId = 0; bId < (image.GetLength(0) * image.GetLength(1)) / BLOCK_SIZE / BLOCK_SIZE; bId++)
            {
                int offsetX = (bId % blocksEachRow) * 8;
                int offsetY = (bId / blocksEachRow) * 8;

                QuantisationForOneBlock(image, offsetX, offsetY, newImage);
            }


            return newImage;
        }

        private static void QuantisationForOneBlock(double[,] image, int offsetX, int offsetY, int[,] newImage)
        {
            for (int y = 0; y < BLOCK_SIZE; y++)
            {
                int realY = y + offsetY;
                for (int x = 0; x < BLOCK_SIZE; x++)
                {
                    int realX = x + offsetX;
                    newImage[realX, realY] = (int)Math.Round(image[realX, realY] / quantisationTable[x, y]);
                }
            }
        }
    }
}
