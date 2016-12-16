using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegConverter.DCTPascal
{
    public class KosinusTransformation
    {
        private void get8x8Blocks(int[,] farbkanal)
        {
            int[,] block = new int[8, 8];

            for (int i = 0; i < farbkanal.GetLength(0); i++)
            {
                for (int j = 0; j < farbkanal.GetLength(1); j++)
                {
                    if (i % 8 == 0 && j % 8 == 0)
                    {
                        direkteKosinusTransformation(block);
                        block = new int[8, 8];
                    }
                    block[i % 8, j % 8] = farbkanal[i, j];
                }
            }
        }

        public static int[,] direkteKosinusTransformation(int[,] block)
        {
            int size = 0;
            if (block.GetLength(0) == block.GetLength(1))
            {
                size = block.GetLength(0);
            }
            else
            {
                return null;
            }

            double ci = 1;
            double cj = 1;
            double sumsum = 0;

            int[,] dctBlock = new int[size, size];
            for (int i = 0; i < size; i++)
            {
                if (i == 0)
                {
                    ci = 1.0 / Math.Sqrt(2);
                }
                else
                {
                    ci = 1;
                }
                for (int j = 0; j < size; j++)
                {
                    if (j == 0)
                    {
                        cj = 1.0 / Math.Sqrt(2);
                    }
                    else
                    {
                        cj = 1;
                    }
                    for (int x = 0; x < block.GetLength(0); x++)
                    {
                        for (int y = 0; y < block.GetLength(1); y++)
                        {
                            sumsum = sumsum + block[x, y] * Math.Cos(((2 * x + 1) * i * Math.PI) / (2 * size)) * Math.Cos(((2 * y + 1) * j * Math.PI) / (2 * size));
                        }
                    }

                    dctBlock[i, j] = (int)(Math.Round(2.0 / size * ci * cj * sumsum));
                    sumsum = 0;
                }
            }

            return dctBlock;
        }

        public static int[,] separierteKosinusTranformation(int[,] block)
        {
            int size = 0;
            if (block.GetLength(0) == block.GetLength(1))
            {
                size = block.GetLength(0);
            }
            else
            {
                return null;
            }

            double[,] input = new double[size, size];
            double[,] matrix = new double[size, size];
            double[,] matrixT = new double[size, size];
            //double[,] sDCT = new double[size, size];
            int[,] result = new int[size, size];

            for (int y = 0; y < block.GetLength(1); y++)
            {
                for (int x = 0; x < block.GetLength(0); x++)
                {
                    input[y, x] = block[y, x];
                }
            }

            for (int n = 0; n < size; n++)
            {
                for (int k = 0; k < size; k++)
                {
                    if (k == 0)
                    {
                        matrix[k, n] = ((1.0 / Math.Sqrt(2.0)) * (Math.Sqrt(2.0 / size) * Math.Cos(((2.0 * n + 1.0) * ((k * Math.PI) / (2.0 * size))))));
                        matrixT[n, k] = matrix[k, n];
                    }
                    else
                    {
                        matrix[k, n] = (1.0 * (Math.Sqrt(2.0 / size) * Math.Cos(((2.0 * n + 1.0) * ((k * Math.PI) / (2.0 * size))))));
                        matrixT[n, k] = matrix[k, n];
                    }
                }
            }

            input = matrizenMultiplikation(matrix, input);
            input = matrizenMultiplikation(input, matrixT);

            for (int y = 0; y < input.GetLength(1); y++)
            {
                for (int x = 0; x < input.GetLength(0); x++)
                {
                    result[y, x] = (int)(Math.Round(input[y, x]));
                }
            }

            return result;
        }

        private static double[,] matrizenMultiplikation(double[,] matrix, double[,] matrix2)
        {
            int size = matrix.GetLength(0);
            double[,] newMatrix = new double[size, size];
            double produkt = 0;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int j = 0; j < size; j++)
                    {
                        produkt = produkt + matrix[x, j] * matrix2[j, y];
                    }
                    newMatrix[x, y] = produkt;
                    produkt = 0;
                }
            }

            return newMatrix;
        }

        public static int[,] inverseDiskreteKosinusTransformation(int[,] block)
        {
            int size = 0;
            double ci = 1;
            double cj = 1;
            if (block.GetLength(0) == block.GetLength(1))
            {
                size = block.GetLength(0);
            }
            else
            {
                return null;
            }

            double isumsum = 0;

            int[,] idctBlock = new int[size, size];

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int i = 0; i < size; i++)
                    {
                        if (i == 0)
                        {
                            ci = (1.0 / Math.Sqrt(2));
                        }
                        else
                        {
                            ci = 1;
                        }
                        for (int j = 0; j < size; j++)
                        {
                            if (j == 0)
                            {
                                cj = (1.0 / Math.Sqrt(2));
                            }
                            else
                            {
                                cj = 1;
                            }
                            isumsum = isumsum + 2.0 / size * ci * cj * block[i, j] * Math.Cos(((2.0 * x + 1.0) * i * Math.PI) / (2.0 * size)) * Math.Cos(((2.0 * y + 1.0) * j * Math.PI) / (2.0 * size));
                        }
                    }
                    idctBlock[x, y] = (int)(Math.Round(isumsum));
                    isumsum = 0;
                }
            }

            return idctBlock;
        }

    }
}
