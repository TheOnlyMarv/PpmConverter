﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JpegConverter.DCT
{
    public class CosinusTransformation
    {
        private static int BLOCK_SIZE = 8;
        private static double ONE_DIV_SQRT2 = 0.70710678118654746;

        #region Splitting and Merging
        private static double[,] MergeBlockIntoImage(List<double[,]> blocks, int sizeX, int sizeY)
        {
            double[,] image = new double[sizeX, sizeY];

            int offsetX = 0;
            int offsetY = 0;
            foreach (double[,] block in blocks)
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

        private static List<double[,]> SplitImageIntoBlocks(double[,] image)
        {
            List<double[,]> result = new List<double[,]>();
            for (int allX = 0; allX < image.GetLength(0); allX += BLOCK_SIZE)
            {
                for (int allY = 0; allY < image.GetLength(1); allY += BLOCK_SIZE)
                {
                    double[,] block = new double[BLOCK_SIZE, BLOCK_SIZE];
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

        #endregion

        #region DirectDCT
        public static double[,] DirectDCT(double[,] image)
        {
            double[,] tempImage = image.Clone() as double[,];
            int blocksEachRow = image.GetLength(0) / BLOCK_SIZE;
            for (int bId = 0; bId < (image.GetLength(0) * image.GetLength(1)) / BLOCK_SIZE / BLOCK_SIZE; bId++)
            {
                int offsetX = (bId % blocksEachRow) * 8;
                int offsetY = (bId / blocksEachRow) * 8;

                DirectDCTforOneBlock(image, tempImage, offsetX, offsetY);
            }

            return image;
        }

        private static void DirectDCTforOneBlock(double[,] image, double[,] tempImage, int offsetX, int offsetY)
        {
            for (int i = offsetY; i < offsetY + 8; i++)
            {
                double ci = i == 0 ? ONE_DIV_SQRT2 : 1.0;
                for (int j = offsetX; j < offsetX + 8; j++)
                {
                    double cj = j == 0 ? ONE_DIV_SQRT2 : 1.0;

                    double innerSum = 0.0;
                    for (int x = 0; x < BLOCK_SIZE; x++)
                    {
                        for (int y = 0; y < BLOCK_SIZE; y++)
                        {
                            innerSum += tempImage[y, x] * Math.Cos(((2.0 * y + 1) * i * Math.PI) / (2.0 * BLOCK_SIZE)) * Math.Cos(((2 * x + 1) * j * Math.PI) / (2 * BLOCK_SIZE));
                        }
                    }
                    image[i, j] = (int)(Math.Round(2.0 / BLOCK_SIZE * ci * cj * innerSum));
                }
            }
        }



        #endregion

        #region SeperateDCT
        public static double[,] SeperateDCT(double[,] image)
        {
            List<double[,]> blocks = SplitImageIntoBlocks(image);

            for (int bId = 0; bId < blocks.Count; bId++)
            {
                double[,] block = blocks[bId];

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


            return MergeBlockIntoImage(blocks, image.GetLength(0), image.GetLength(1));
        }

        #endregion

        #region Inverse DirectDCT
        public static double[,] InverseDirectDCT(double[,] image)
        {
            List<double[,]> blocks = SplitImageIntoBlocks(image);

            for (int bId = 0; bId < blocks.Count; bId++)
            {
                double[,] block = blocks[bId];

                double[,] newBlock = new double[BLOCK_SIZE, BLOCK_SIZE];

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
                        newBlock[x, y] = Math.Round(isumsum);
                    }
                }
                blocks[bId] = newBlock;
            }

            image = MergeBlockIntoImage(blocks, image.GetLength(0), image.GetLength(1));
            return image;
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

        public static int[,] AraiDCT(int[,] image)
        {
            // Diese Methode bitte erstmal nicht anfassen und Ändern DANKE :) LG Marv
            double[,] temp = new double[image.GetLength(0), image.GetLength(1)];
            for (int i = 0; i < temp.GetLength(0); i++)
            {
                for (int j = 0; j < temp.GetLength(1); j++)
                {
                    temp[i, j] = (double)image[i, j];
                }
            }
            AraiDCT(temp);
            for (int i = 0; i < temp.GetLength(0); i++)
            {
                for (int j = 0; j < temp.GetLength(1); j++)
                {
                    image[i, j] = (int)Math.Round(temp[i, j]);
                }
            }
            return image;
        }

        public static double[,] AraiDCT(double[,] image)
        {
            int blocksEachRow = image.GetLength(0) / BLOCK_SIZE;
            for (int bId = 0; bId < (image.GetLength(0) * image.GetLength(1)) / BLOCK_SIZE / BLOCK_SIZE; bId++)
            {
                int offsetX = (bId % blocksEachRow) * 8;
                int offsetY = (bId / blocksEachRow) * 8;

                AraiForOneBlock(image, offsetX, offsetY);
            }

            return image;
        }

        private static void AraiForOneBlock(double[,] image, int offsetX, int offsetY)//double[,] image, int offsetX, int offsetY
        {

            for (int i = 0; i < BLOCK_SIZE; i++)
            {
                int realI = offsetX + i;
                //Arai(image[realI, offsetY + 0], image[realI, offsetY + 1], image[realI, offsetY + 2], image[realI, offsetY + 3], image[realI, offsetY + 4], image[realI, offsetY + 5], image[realI, offsetY + 6], image[realI, offsetY + 7],
                //    out image[realI, offsetY + 0], out image[realI, offsetY + 1], out image[realI, offsetY + 2], out image[realI, offsetY + 3], out image[realI, offsetY + 4], out image[realI, offsetY + 5], out image[realI, offsetY + 6], out image[realI, offsetY + 7]);

                double t0, t1, t2, t3, t4, t5, t6, t7;
                double d0 = image[realI, offsetY + 0], d1 = image[realI, offsetY + 1], d2 = image[realI, offsetY + 2], d3 = image[realI, offsetY + 3], d4 = image[realI, offsetY + 4], d5 = image[realI, offsetY + 5], d6 = image[realI, offsetY + 6], d7 = image[realI, offsetY + 7];

                //Step-1
                t0 = d0 + d7;
                t1 = d1 + d6;
                t2 = d2 + d5;
                t3 = d3 + d4;
                t4 = d3 - d4;
                t5 = d2 - d5;
                t6 = d1 - d6;
                t7 = d0 - d7;

                //Assign
                d0 = t0;
                d1 = t1;
                d2 = t2;
                d3 = t3;
                d4 = t4;
                d5 = t5;
                d6 = t6;
                d7 = t7;


                //Step-2
                t0 = d0 + d3;
                t1 = d1 + d2;
                t2 = d1 - d2;
                t3 = d0 - d3;
                t4 = -d4 - d5;
                t5 = d5 + d6;
                t6 = d6 + d7;
                //No 7

                //Assign
                d0 = t0;
                d1 = t1;
                d2 = t2;
                d3 = t3;
                d4 = t4;
                d5 = t5;
                d6 = t6;
                d7 = t7;


                //Step-3
                t0 = d0 + d1;
                t1 = d0 - d1;
                t2 = d2 + d3;
                //No 3-7

                //Assign
                d0 = t0;
                d1 = t1;
                d2 = t2;
                d3 = t3;
                d4 = t4;
                d5 = t5;
                d6 = t6;
                d7 = t7;


                //Step-4
                //No 0-1
                t2 = d2 * a1;
                //No 3
                double tempA5 = (d4 + d6) * a5;
                t4 = (-(d4 * a2)) - t5;
                t5 = d5 * a3;
                t6 = (d6 * a4) - tempA5;
                //No 7

                //Assign
                d0 = t0;
                d1 = t1;
                d2 = t2;
                d3 = t3;
                d4 = t4;
                d5 = t5;
                d6 = t6;
                d7 = t7;


                //Step-5
                //No 0-1
                t2 = d2 + d3;
                t3 = d3 - d2;
                //No 4
                t5 = d5 + d7;
                //No 6
                t7 = d7 - d5;

                //Assign
                d0 = t0;
                d1 = t1;
                d2 = t2;
                d3 = t3;
                d4 = t4;
                d5 = t5;
                d6 = t6;
                d7 = t7;


                //Step-6
                //No 0-3
                t4 = d4 + d7;
                t5 = d5 + d6;
                t6 = d5 - d6;
                t7 = d7 - d4;

                //Assign
                d0 = t0;
                d1 = t1;
                d2 = t2;
                d3 = t3;
                d4 = t4;
                d5 = t5;
                d6 = t6;
                d7 = t7;


                //Step-7
                t0 = d0 * s0;
                t1 = d1 * s4;
                t2 = d2 * s2;
                t3 = d3 * s6;
                t4 = d4 * s5;
                t5 = d5 * s1;
                t6 = d6 * s7;
                t7 = d7 * s3;

                //Reassign
                image[realI, offsetY + 0] = t0;
                image[realI, offsetY + 4] = t1;
                image[realI, offsetY + 2] = t2;
                image[realI, offsetY + 6] = t3;
                image[realI, offsetY + 5] = t4;
                image[realI, offsetY + 1] = t5;
                image[realI, offsetY + 7] = t6;
                image[realI, offsetY + 3] = t7;
            }
            for (int i = 0; i < BLOCK_SIZE; i++)
            {
                int realI = offsetY + i;

                double t0, t1, t2, t3, t4, t5, t6, t7;
                double d0 = image[offsetX + 0, realI], d1 = image[offsetX + 1, realI], d2 = image[offsetX + 2, realI], d3 = image[offsetX + 3, realI], d4 = image[offsetX + 4, realI], d5 = image[offsetX + 5, realI], d6 = image[offsetX + 6, realI], d7 = image[offsetX + 7, realI];

                //Step-1
                t0 = d0 + d7;
                t1 = d1 + d6;
                t2 = d2 + d5;
                t3 = d3 + d4;
                t4 = d3 - d4;
                t5 = d2 - d5;
                t6 = d1 - d6;
                t7 = d0 - d7;

                //Assign
                d0 = t0;
                d1 = t1;
                d2 = t2;
                d3 = t3;
                d4 = t4;
                d5 = t5;
                d6 = t6;
                d7 = t7;


                //Step-2
                t0 = d0 + d3;
                t1 = d1 + d2;
                t2 = d1 - d2;
                t3 = d0 - d3;
                t4 = -d4 - d5;
                t5 = d5 + d6;
                t6 = d6 + d7;
                //No 7

                //Assign
                d0 = t0;
                d1 = t1;
                d2 = t2;
                d3 = t3;
                d4 = t4;
                d5 = t5;
                d6 = t6;
                d7 = t7;


                //Step-3
                t0 = d0 + d1;
                t1 = d0 - d1;
                t2 = d2 + d3;
                //No 3-7

                //Assign
                d0 = t0;
                d1 = t1;
                d2 = t2;
                d3 = t3;
                d4 = t4;
                d5 = t5;
                d6 = t6;
                d7 = t7;


                //Step-4
                //No 0-1
                t2 = d2 * a1;
                //No 3
                double tempA5 = (d4 + d6) * a5;
                t4 = (-(d4 * a2)) - t5;
                t5 = d5 * a3;
                t6 = (d6 * a4) - tempA5;
                //No 7

                //Assign
                d0 = t0;
                d1 = t1;
                d2 = t2;
                d3 = t3;
                d4 = t4;
                d5 = t5;
                d6 = t6;
                d7 = t7;


                //Step-5
                //No 0-1
                t2 = d2 + d3;
                t3 = d3 - d2;
                //No 4
                t5 = d5 + d7;
                //No 6
                t7 = d7 - d5;

                //Assign
                d0 = t0;
                d1 = t1;
                d2 = t2;
                d3 = t3;
                d4 = t4;
                d5 = t5;
                d6 = t6;
                d7 = t7;


                //Step-6
                //No 0-3
                t4 = d4 + d7;
                t5 = d5 + d6;
                t6 = d5 - d6;
                t7 = d7 - d4;

                //Assign
                d0 = t0;
                d1 = t1;
                d2 = t2;
                d3 = t3;
                d4 = t4;
                d5 = t5;
                d6 = t6;
                d7 = t7;


                //Step-7
                t0 = d0 * s0;
                t1 = d1 * s4;
                t2 = d2 * s2;
                t3 = d3 * s6;
                t4 = d4 * s5;
                t5 = d5 * s1;
                t6 = d6 * s7;
                t7 = d7 * s3;

                //Reassign
                image[offsetX + 0, realI] = t0;
                image[offsetX + 4, realI] = t1;
                image[offsetX + 2, realI] = t2;
                image[offsetX + 6, realI] = t3;
                image[offsetX + 5, realI] = t4;
                image[offsetX + 1, realI] = t5;
                image[offsetX + 7, realI] = t6;
                image[offsetX + 3, realI] = t7;
            }
        }

        private static void Arai(
            double x0, double x1, double x2, double x3, double x4, double x5, double x6, double x7,
            out double xo0, out double xo1, out double xo2, out double xo3, out double xo4, out double xo5, out double xo6, out double xo7)
        {

            double t0, t1, t2, t3, t4, t5, t6, t7;
            double d0 = x0, d1 = x1, d2 = x2, d3 = x3, d4 = x4, d5 = x5, d6 = x6, d7 = x7;

            //Step-1
            t0 = d0 + d7;
            t1 = d1 + d6;
            t2 = d2 + d5;
            t3 = d3 + d4;
            t4 = d3 - d4;
            t5 = d2 - d5;
            t6 = d1 - d6;
            t7 = d0 - d7;

            //Assign
            d0 = t0;
            d1 = t1;
            d2 = t2;
            d3 = t3;
            d4 = t4;
            d5 = t5;
            d6 = t6;
            d7 = t7;
            

            //Step-2
            t0 = d0 + d3;
            t1 = d1 + d2;
            t2 = d1 - d2;
            t3 = d0 - d3;
            t4 = -d4 - d5;
            t5 = d5 + d6;
            t6 = d6 + d7;
            //No 7

            //Assign
            d0 = t0;
            d1 = t1;
            d2 = t2;
            d3 = t3;
            d4 = t4;
            d5 = t5;
            d6 = t6;
            d7 = t7;


            //Step-3
            t0 = d0 + d1;
            t1 = d0 - d1;
            t2 = d2 + d3;
            //No 3-7

            //Assign
            d0 = t0;
            d1 = t1;
            d2 = t2;
            d3 = t3;
            d4 = t4;
            d5 = t5;
            d6 = t6;
            d7 = t7;


            //Step-4
            //No 0-1
            t2 = d2 * a1;
            //No 3
            double tempA5 = (d4 + d6) * a5;
            t4 = (-(d4 * a2)) - t5;
            t5 = d5 * a3;
            t6 = (d6 * a4) - tempA5;
            //No 7

            //Assign
            d0 = t0;
            d1 = t1;
            d2 = t2;
            d3 = t3;
            d4 = t4;
            d5 = t5;
            d6 = t6;
            d7 = t7;


            //Step-5
            //No 0-1
            t2 = d2 + d3;
            t3 = d3 - d2;
            //No 4
            t5 = d5 + d7;
            //No 6
            t7 = d7 - d5;

            //Assign
            d0 = t0;
            d1 = t1;
            d2 = t2;
            d3 = t3;
            d4 = t4;
            d5 = t5;
            d6 = t6;
            d7 = t7;


            //Step-6
            //No 0-3
            t4 = d4 + d7;
            t5 = d5 + d6;
            t6 = d5 - d6;
            t7 = d7 - d4;

            //Assign
            d0 = t0;
            d1 = t1;
            d2 = t2;
            d3 = t3;
            d4 = t4;
            d5 = t5;
            d6 = t6;
            d7 = t7;


            //Step-7
            t0 = d0 * s0;
            t1 = d1 * s4;
            t2 = d2 * s2;
            t3 = d3 * s6;
            t4 = d4 * s5;
            t5 = d5 * s1;
            t6 = d6 * s7;
            t7 = d7 * s3;

            //Reassign
            xo0 = t0;
            xo4 = t1;
            xo2 = t2;
            xo6 = t3;
            xo5 = t4;
            xo1 = t5;
            xo7 = t6;
            xo3 = t7;
        }
        #endregion

        #region Matrix Multiplications

        private static double[,] MatrixMultiplication(double[,] m1, double[,] m2)
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
