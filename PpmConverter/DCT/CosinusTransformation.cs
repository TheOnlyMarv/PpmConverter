using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegConverter.DCT
{
    public class CosinusTransformation
    {
        private const int BLOCK_SIZE = 8;
        private const double ONE_DIV_SQRT2 = 0.70710678118654746;
        private int[,] OriginalImage { get; set; }
        private int[,] TransformedImage { get; set; }

        public CosinusTransformation(int[,] image)
        {
            if (image.GetLength(0) % BLOCK_SIZE == 0 && image.GetLength(1) % BLOCK_SIZE == 0)
            {
                this.OriginalImage = image;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Dimensions not 8 divisible");
            }
        }

        #region Splitting and Merging
        private int[,] MergeBlockIntoImage(List<int[,]> blocks)
        {
            int sizeX = OriginalImage.GetLength(0);
            int sizeY = OriginalImage.GetLength(1);

            int[,] image = new int[sizeX, sizeY];

            int offsetX = 0;
            int offsetY = 0;
            foreach (int[,] block in blocks)
            {
                for (int x = 0; x < block.GetLength(0); x++)
                {
                    for (int y = 0; y < block.GetLength(1); y++)
                    {
                        image[x + offsetX, y + offsetY] = block[x, y];
                    }
                }
                offsetY += 8;
                if (offsetY >= sizeY)
                {
                    offsetX += 8;
                    offsetY = 0;
                }
            }
            return image;
        }

        private List<int[,]> SplitImageIntoBlocks()
        {
            List<int[,]> result = new List<int[,]>();
            for (int allX = 0; allX < OriginalImage.GetLength(0); allX += BLOCK_SIZE)
            {
                for (int allY = 0; allY < OriginalImage.GetLength(1); allY += BLOCK_SIZE)
                {
                    int[,] block = new int[BLOCK_SIZE, BLOCK_SIZE];
                    for (int x = 0; x < BLOCK_SIZE; x++)
                    {
                        for (int y = 0; y < BLOCK_SIZE; y++)
                        {
                            block[x, y] = OriginalImage[allX + x, allY + y];
                        }
                    }
                    result.Add(block);
                }
            }
            return result;
        }

        #endregion

        #region DirectDCT
        public int[,] DirectDCT()
        {
            List<int[,]> blocks = SplitImageIntoBlocks();

            for (int bId = 0; bId < blocks.Count; bId++)
            {
                int[,] block = blocks[bId];
                int[,] newBlock = new int[BLOCK_SIZE, BLOCK_SIZE];
                for (int i = 0; i < block.GetLength(0); i++)
                {
                    double ci = i == 0 ? ONE_DIV_SQRT2 : 1.0;
                    for (int j = 0; j < block.GetLength(1); j++)
                    {
                        double cj = j == 0 ? ONE_DIV_SQRT2 : 1.0;

                        double innerSum = 0.0;
                        for (int x = 0; x < BLOCK_SIZE; x++)
                        {
                            for (int y = 0; y < BLOCK_SIZE; y++)
                            {
                                innerSum += block[x, y] * Math.Cos(((2.0 * x + 1) * i * Math.PI) / (2.0 * BLOCK_SIZE)) * Math.Cos(((2 * y + 1) * j * Math.PI) / (2 * BLOCK_SIZE));
                            }
                        }
                        newBlock[i, j] = (int)(Math.Round(2.0 / BLOCK_SIZE * ci * cj * innerSum));
                    }
                }
                blocks[bId] = newBlock;
            }

            TransformedImage = MergeBlockIntoImage(blocks);
            return TransformedImage;
        }

        #endregion

        #region SeperateDCT
        public int[,] SeperateDCT()
        {
            List<int[,]> blocks = SplitImageIntoBlocks();

            for (int bId = 0; bId < blocks.Count; bId++)
            {
                int[,] block = blocks[bId];

                double[,] cMatrix = new double[BLOCK_SIZE, BLOCK_SIZE];
                double[,] cMatrixT = new double[BLOCK_SIZE, BLOCK_SIZE];

                for (int n = 0; n < BLOCK_SIZE; n++)
                {
                    for (int k = 0; k < BLOCK_SIZE; k++)
                    {
                        double c0 = k == 0 ? ONE_DIV_SQRT2 : 1.0;
                        cMatrix[k, n] = c0 * Math.Sqrt(2.0 / BLOCK_SIZE) * Math.Cos((2.0 * n + 1.0) * (k * Math.PI) / (2.0 * BLOCK_SIZE));
                        cMatrixT[n, k] = cMatrix[k, n];
                    }
                }
                double[,] temp = MatrixMultiplication(cMatrix, block);
                block = MatrixMultiplication(temp, cMatrixT);

                blocks[bId] = block;
            }


            TransformedImage = MergeBlockIntoImage(blocks);
            return TransformedImage;
        }

        #endregion

        #region Inverse DirectDCT
        public int[,] InverseDirectDCT()
        {
            List<int[,]> blocks = SplitImageIntoBlocks();

            for (int bId = 0; bId < blocks.Count; bId++)
            {
                int[,] block = blocks[bId];

                int[,] newBlock = new int[BLOCK_SIZE, BLOCK_SIZE];

                for (int x = 0; x < BLOCK_SIZE; x++)
                {
                    for (int y = 0; y < BLOCK_SIZE; y++)
                    {
                        double isumsum = 0;
                        for (int i = 0; i < BLOCK_SIZE; i++)
                        {
                            double ci = i == 0 ? ONE_DIV_SQRT2 : 1.0;
                            for (int j = 0; j < BLOCK_SIZE; j++)
                            {
                                double cj = j == 0 ? ONE_DIV_SQRT2 : 1.0;
                                isumsum += 2.0 / BLOCK_SIZE * ci * cj * block[i, j] * Math.Cos(((2.0 * x + 1.0) * i * Math.PI) / (2.0 * BLOCK_SIZE)) * Math.Cos(((2.0 * y + 1.0) * j * Math.PI) / (2.0 * BLOCK_SIZE));
                            }
                        }
                        newBlock[x, y] = (int)(Math.Round(isumsum));
                    }
                }
                blocks[bId] = newBlock;
            }

            TransformedImage = MergeBlockIntoImage(blocks);
            return TransformedImage;
        }

        #endregion

        #region Matrix Multiplications
        private int[,] MatrixMultiplication(double[,] m1, double[,] m2)
        {
            int[,] newMatrix = new int[BLOCK_SIZE, BLOCK_SIZE];
            for (int x = 0; x < BLOCK_SIZE; x++)
            {
                for (int y = 0; y < BLOCK_SIZE; y++)
                {
                    double product = 0;
                    for (int j = 0; j < BLOCK_SIZE; j++)
                    {
                        product = product + m1[x, j] * m2[j, y];
                    }
                    newMatrix[x, y] = (int)Math.Round(product);
                }
            }
            return newMatrix;
        }

        private double[,] MatrixMultiplication(double[,] m1, int[,] m2)
        {
            double[,] newMatrix = new double[BLOCK_SIZE, BLOCK_SIZE];
            for (int x = 0; x < BLOCK_SIZE; x++)
            {
                for (int y = 0; y < BLOCK_SIZE; y++)
                {
                    double product = 0;
                    for (int j = 0; j < BLOCK_SIZE; j++)
                    {
                        product = product + m1[x, j] * m2[j, y];
                    }
                    newMatrix[x, y] = product;
                }
            }
            return newMatrix;
        }

        #endregion
    }
}
