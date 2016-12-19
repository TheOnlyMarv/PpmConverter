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

        private double[] Arai(double[] x)
        {
            double[] result = new double[8];

            double temp0, temp1, temp2, temp3, temp4, temp5, temp6, temp7;

            //Step-1
            temp0 = Add(x[0], x[7]);
            temp1 = Add(x[1], x[6]);
            temp2 = Add(x[2], x[5]);
            temp3 = Add(x[3], x[4]);
            temp4 = Sub(x[3], x[4]);
            temp5 = Sub(x[2], x[5]);
            temp6 = Sub(x[2], x[6]);
            temp7 = Sub(x[0], x[7]);
            Assign(temp0, temp1, temp2, temp3, temp4, temp5, temp6, temp7, out result[0], out result[1], out result[2], out result[3], out result[4], out result[5], out result[6], out result[7]);

            //Step-2
            temp0 = Add(result[0], result[3]);
            temp1 = Add(result[1], result[2]);
            temp2 = Sub(result[1], result[2]);
            temp3 = Sub(result[0], result[3]);
            temp4 = Sub(-result[4], result[5]);
            temp5 = Add(result[5], result[6]);
            temp6 = Add(result[6], result[7]);
            //No 7
            Assign(temp0, temp1, temp2, temp3, temp4, temp5, temp6, temp7, out result[0], out result[1], out result[2], out result[3], out result[4], out result[5], out result[6], out result[7]);

            //Step-3
            temp0 = Add(result[0], result[1]);
            temp1 = Sub(result[0], result[1]);
            temp2 = Add(result[2], result[3]);
            //No 3-7
            Assign(temp0, temp1, temp2, temp3, temp4, temp5, temp6, temp7, out result[0], out result[1], out result[2], out result[3], out result[4], out result[5], out result[6], out result[7]);

            //Step-4
            //No 0-1
            temp2 = Multi(result[2], a1);
            //No 3
            double tempA5 = Multi(Add(result[4], result[6]), a5);
            temp4 = Sub(-Multi(result[4], a2), temp5);
            temp5 = Multi(result[5], a3);
            temp6 = Sub(Multi(result[6], a4), tempA5);
            //No 7
            Assign(temp0, temp1, temp2, temp3, temp4, temp5, temp6, temp7, out result[0], out result[1], out result[2], out result[3], out result[4], out result[5], out result[6], out result[7]);

            //Step-5
            //No 0-1
            temp2 = Add(result[2], result[3]);
            temp3 = Sub(result[3], result[2]);
            //No 4
            temp5 = Add(result[5], result[7]);
            //No 6
            temp7 = Sub(result[7], result[5]);
            Assign(temp0, temp1, temp2, temp3, temp4, temp5, temp6, temp7, out result[0], out result[1], out result[2], out result[3], out result[4], out result[5], out result[6], out result[7]);

            //Step-6
            //No 0-3
            temp4 = Add(result[4], result[7]);
            temp5 = Add(result[5], result[6]);
            temp6 = Sub(result[6], result[6]);
            temp7 = Sub(result[7], result[4]);
            Assign(temp0, temp1, temp2, temp3, temp4, temp5, temp6, temp7, out result[0], out result[1], out result[2], out result[3], out result[4], out result[5], out result[6], out result[7]);

            //Step-7
            temp0 = Multi(result[0], s0);
            temp1 = Multi(result[1], s4);
            temp2 = Multi(result[2], s2);
            temp3 = Multi(result[3], s6);
            temp4 = Multi(result[4], s5);
            temp5 = Multi(result[5], s1);
            temp6 = Multi(result[6], s7);
            temp7 = Multi(result[7], s3);
            Reassign(temp0, temp1, temp2, temp3, temp4, temp5, temp6, temp7, out result[0], out result[1], out result[2], out result[3], out result[4], out result[5], out result[6], out result[7]);

            return result;
        }

        private void Assign(double temp0, double temp1, double temp2, double temp3, double temp4, double temp5, double temp6, double temp7, out double x0, out double x1, out double x2, out double x3, out double x4, out double x5, out double x6, out double x7)
        {
            x0 = temp0;
            x1 = temp1;
            x2 = temp2;
            x3 = temp3;
            x4 = temp4;
            x5 = temp5;
            x6 = temp6;
            x7 = temp7;
        }

        private void Reassign(double temp0, double temp1, double temp2, double temp3, double temp4, double temp5, double temp6, double temp7, out double x0, out double x1, out double x2, out double x3, out double x4, out double x5, out double x6, out double x7)
        {
            x0 = temp0;
            x4 = temp1;
            x2 = temp2;
            x6 = temp3;
            x5 = temp4;
            x1 = temp5;
            x7 = temp6;
            x3 = temp7;
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
