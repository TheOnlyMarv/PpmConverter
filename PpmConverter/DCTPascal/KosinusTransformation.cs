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

            for(int i = 0; i < farbkanal.GetLength(0); i++)
            {
                for(int j = 0; j < farbkanal.GetLength(1); j++)
                {
                    if(i % 8 == 0 && j % 8 == 0)
                    {
                        direkteKosinusTransformation(block);
                        block = new int[8,8];
                    }
                    block[i % 8, j % 8] = farbkanal[i,j];
                }
            }
        }

        public static int[,] direkteKosinusTransformation(int[,] block)
        {            
            int size = 0;
            if(block.GetLength(0) == block.GetLength(1))
            {
                size = block.GetLength(0);
            }
            else
            {
                return null;
            }
            double sumsum = 0;

            int[,] dctBlock = new int[8, 8];
            for(int i = 0; i < size; i++)
            {
                for(int j = 0; j < size; j++)
                {
                    for (int x = 0; x < block.GetLength(0); x++)
                    {
                        for (int y = 0; y < block.GetLength(1); y++)
                        {
                            sumsum = sumsum + block[x, y] * Math.Cos(((2 * x + 1) * i * Math.PI) / (2 * size)) * Math.Cos(((2 * y + 1) * j * Math.PI) / (2 * size));
                        }
                    }

                    if (i == 0 && j == 0)
                    {
                        dctBlock[i, j] = (int)(Math.Round(2.0 / size * 1.0 / Math.Sqrt(2) * 1.0 / Math.Sqrt(2) * sumsum));
                        sumsum = 0;
                    }
                    else if (i == 0 && j != 0)
                    {
                        dctBlock[i, j] = (int)(Math.Round(2.0 / size * 1.0 / Math.Sqrt(2) * 1.0 * sumsum));
                        sumsum = 0;
                    }
                    else if (i != 0 && j == 0)
                    {
                        dctBlock[i, j] =(int)(Math.Round(2.0 / size * 1.0 * 1.0 / Math.Sqrt(2) * sumsum));
                        sumsum = 0;
                    }
                    else
                    {
                        dctBlock[i, j] = (int)(Math.Round(2.0 / size * 1.0 * 1.0 * sumsum));
                        sumsum = 0;
                    }
                }
            }

            return dctBlock;
        }

        public static int[,] seperierteKosinusTranformation(int[,] block)
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

            int[,] matrix = new int[size, size];
            int[,] matrixT = new int[size, size];
            int[,] sDCT = new int[size, size];

            for(int k = 0; k < size; k++)
            {
                for(int n = 0; n < size; n++)
                {
                    if(k == 0)
                    {
                        matrix[k, n] = (int)(Math.Round(1.0 / Math.Sqrt(2) * Math.Sqrt(2 / size) * Math.Cos((2.0 * n + 1.0) * (k * Math.PI / (2.0 * size)))));
                        matrixT[n, k] = matrix[k, n];
                    }
                    else
                    {
                        matrix[k, n] = (int)(Math.Round(1.0 * Math.Sqrt(2 / size) * Math.Cos((2.0 * n + 1.0) * (k * Math.PI / (2.0 * size)))));
                        matrixT[n, k] = matrix[k, n];
                    }
                }
            }
            int produkt = 0;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            produkt = produkt + matrix[i, j] * matrixT[j, i];
                        }
                    }
                    sDCT[x, y] = produkt;
                    produkt = 0;
                }
            }
            return sDCT;
        } 

        public static int[,] inverseDiskreteKosinusTransformation(int[,] block)
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

            double isumsum = 0;

            int[,] idctBlock = new int[size, size];

            for(int x = 0; x < size; x++)
            {
                for(int y = 0; y < size; y++)
                {
                    for(int i = 0; i < size; i++)
                    {
                        for(int j = 0; j < size; j++)
                        {
                            if (i == 0 && j == 0)
                            {
                                isumsum = isumsum + 2.0 / size * (1.0 / Math.Sqrt(2)) * (1.0 / Math.Sqrt(2)) * block[i, j] * Math.Cos(((2.0 * x + 1.0) * i * Math.PI) / (2.0 * size)) * Math.Cos(((2.0 * y + 1.0) * j * Math.PI) / (2.0 * size));
                            }
                            else if (i == 0 && j != 0)
                            {
                                isumsum = isumsum + 2.0 / size * (1.0 / Math.Sqrt(2)) * 1.0 * block[i, j] * Math.Cos(((2.0 * x + 1.0) * i * Math.PI) / (2.0 * size)) * Math.Cos(((2.0 * y + 1.0) * j * Math.PI) / (2.0 * size));
                            }
                            else if (i != 0 && j == 0)
                            {
                                isumsum = isumsum + 2.0 / size * 1.0 * (1.0 / Math.Sqrt(2)) * block[i, j] * Math.Cos(((2.0 * x + 1.0) * i * Math.PI) / (2.0 * size)) * Math.Cos(((2.0 * y + 1.0) * j * Math.PI) / (2.0 * size));
                            }
                            else
                            {
                                isumsum = isumsum + 2.0 / size * 1.0 * 1.0 * block[i, j] * Math.Cos(((2.0 * x + 1.0) * i * Math.PI) / (2.0 * size)) * Math.Cos(((2.0 * y + 1.0) * j * Math.PI) / (2.0 * size));
                            }
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
