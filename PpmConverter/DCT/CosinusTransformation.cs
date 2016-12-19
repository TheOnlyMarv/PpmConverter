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

        #region Arai DCT

        private const double s0 = 0.35355339059327373;
        private const double s1 = 0.25489778955207959;
        private const double s2 = 0.27059805007309851;
        private const double s3 = 0.30067244346752264;
        private const double s4 = 0.35355339059327373;
        private const double s5 = 0.44998811156820778;
        private const double s6 = 0.65328148243818818;
        private const double s7 = 1.2814577238707527;

        private const double c2 = 0.92387953251128674;
        private const double c4 = 0.70710678118654757;
        private const double c6 = 0.38268343236508984;

        private const double a1 = c4;
        private const double a2 = c2 - c6;
        private const double a3 = c4;
        private const double a4 = c6 + c2;
        private const double a5 = c6;

        public int[,] AraiDCT()
        {
            List<int[,]> blocks = SplitImageIntoBlocks();

            for (int bId = 0; bId < blocks.Count; bId++)
            {
                int[,] block = blocks[bId];
                for (int i = 0; i < BLOCK_SIZE; i++)
                {
                    Arai(block[i, 0], block[i, 1], block[i, 2], block[i, 3], block[i, 4], block[i, 5], block[i, 6], block[i, 7],
                        out block[i, 0], out block[i, 1], out block[i, 2], out block[i, 3], out block[i, 4], out block[i, 5], out block[i, 6], out block[i, 7]);
                }

                for (int i = 0; i < BLOCK_SIZE; i++)
                {
                    Arai(block[0, i], block[1, i], block[2, i], block[3, i], block[4, i], block[5, i], block[6, i], block[7, i],
                        out block[0, i], out block[1, i], out block[2, i], out block[3, i], out block[4, i], out block[5, i], out block[6, i], out block[7, i]);
                }
                blocks[bId] = block;
            }

            TransformedImage = MergeBlockIntoImage(blocks);
            return TransformedImage;
        }

        private void Arai(
            int x0, int x1, int x2, int x3, int x4, int x5, int x6, int x7,
            out int xo0, out int xo1, out int xo2, out int xo3, out int xo4, out int xo5, out int xo6, out int xo7)
        {

            Assign(x0, x1, x2, x3, x4, x5, x6, x7, out xo0, out xo1, out xo2, out xo3, out xo4, out xo5, out xo6, out xo7);


            //Step-1
            xo0 = x0 + x7;
            xo1 = x1 + x6;
            xo2 = x2 + x5;
            xo3 = x3 + x4;
            xo4 = x3 - x4;
            xo5 = x2 - x5;
            xo6 = x1 - x6;
            xo7 = x0 - x7;
            Assign(xo0, xo1, xo2, xo3, xo4, xo5, xo6, xo7, out x0, out x1, out x2, out x3, out x4, out x5, out x6, out x7);

            //Step-2
            xo0 = x0 + x3;
            xo1 = x1 + x2;
            xo2 = x1 - x2;
            xo3 = x0 - x3;
            xo4 = -x4 - x5;
            xo5 = x5 + x6;
            xo6 = x6 + x7;
            //No 7
            Assign(xo0, xo1, xo2, xo3, xo4, xo5, xo6, xo7, out x0, out x1, out x2, out x3, out x4, out x5, out x6, out x7);

            //Step-3
            xo0 = x0 + x1;
            xo1 = x0- x1;
            xo2 = x2+x3;
            //No 3-7
            Assign(xo0, xo1, xo2, xo3, xo4, xo5, xo6, xo7, out x0, out x1, out x2, out x3, out x4, out x5, out x6, out x7);

            //Step-4
            //No 0-1
            xo2 = (int)Math.Round((x2 * a1));
            //No 3
            double tempA5 = (x4 + x6) * a5;
            xo4 = (int)Math.Round((-(x4 * a2) - xo5));
            xo5 = (int)Math.Round((x5 * a3));
            xo6 = (int)Math.Round(((x6 * a4) - tempA5));
            //No 7
            Assign(xo0, xo1, xo2, xo3, xo4, xo5, xo6, xo7, out x0, out x1, out x2, out x3, out x4, out x5, out x6, out x7);

            //Step-5
            //No 0-1
            xo2 = x2 + x3;
            xo3 = x3 - x2;
            //No 4
            xo5 = x5 + x7;
            //No 6
            xo7 = x7 - x5;
            Assign(xo0, xo1, xo2, xo3, xo4, xo5, xo6, xo7, out x0, out x1, out x2, out x3, out x4, out x5, out x6, out x7);

            //Step-6
            //No 0-3
            xo4 = x4 + x7;
            xo5 = x5 + x6;
            xo6 = x5 - x6;
            xo7 = x7 - x4;
            Assign(xo0, xo1, xo2, xo3, xo4, xo5, xo6, xo7, out x0, out x1, out x2, out x3, out x4, out x5, out x6, out x7);

            //Step-7
            xo0 = (int)Math.Round(x0 * s0);
            xo1 = (int)Math.Round(x1 * s4);
            xo2 = (int)Math.Round(x2 * s2);
            xo3 = (int)Math.Round(x3 * s6);
            xo4 = (int)Math.Round(x4 * s5);
            xo5 = (int)Math.Round(x5 * s1);
            xo6 = (int)Math.Round(x6 * s7);
            xo7 = (int)Math.Round(x7 * s3);
            Reassign(xo0, xo1, xo2, xo3, xo4, xo5, xo6, xo7, out x0, out x1, out x2, out x3, out x4, out x5, out x6, out x7);

        }

        private void Assign(int xo0, int xo1, int xo2, int xo3, int xo4, int xo5, int xo6, int xo7, out int x0, out int x1, out int x2, out int x3, out int x4, out int x5, out int x6, out int x7)
        {
            x0 = xo0;
            x1 = xo1;
            x2 = xo2;
            x3 = xo3;
            x4 = xo4;
            x5 = xo5;
            x6 = xo6;
            x7 = xo7;
        }

        private void Reassign(int xo0, int xo1, int xo2, int xo3, int xo4, int xo5, int xo6, int xo7, out int x0, out int x1, out int x2, out int x3, out int x4, out int x5, out int x6, out int x7)
        {
            x0 = xo0;
            x4 = xo1;
            x2 = xo2;
            x6 = xo3;
            x5 = xo4;
            x1 = xo5;
            x7 = xo6;
            x3 = xo7;
        }

        private double Add(double a, double b)
        {
            return a + b;
        }
        private double Sub(double a, double b)
        {
            return a - b;
        }
        private double Multi(double a, double b)
        {
            return a * b;
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
