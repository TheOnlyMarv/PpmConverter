using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegConverter.DCTPascal
{
    class KosinusTransformation
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
            int n = block.GetLength(0);
            double sumsum = 0;

            int[,] dctBlock = new int[8, 8];
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    for (int x = 0; x < block.GetLength(0); x++)
                    {
                        for (int y = 0; y < block.GetLength(1); y++)
                        {
                            sumsum = sumsum + block[x, y] * Math.Cos(((2 * x + 1) * i * Math.PI) / (2 * n)) * Math.Cos(((2 * y + 1) * j * Math.PI) / (2 * n));
                        }
                    }

                    if (i == 0 && j == 0)
                    {
                        dctBlock[i, j] = (int)(Math.Round(2.0 / n * 1.0 / Math.Sqrt(2) * 1.0 / Math.Sqrt(2) * sumsum));
                        sumsum = 0;
                    }
                    else if (i == 0 && j != 0)
                    {
                        dctBlock[i, j] = (int)(Math.Round(2.0 / n * 1.0 / Math.Sqrt(2) * 1.0 * sumsum));
                        sumsum = 0;
                    }
                    else if (i != 0 && j == 0)
                    {
                        dctBlock[i, j] =(int)(Math.Round(2.0 / n * 1.0 * 1.0 / Math.Sqrt(2) * sumsum));
                        sumsum = 0;
                    }
                    else
                    {
                        dctBlock[i, j] = (int)(Math.Round(2.0 / n * 1.0 * 1.0 * sumsum));
                        sumsum = 0;
                    }
                }
            }

            return dctBlock;
        }



        public static int[,] inverseDiskreteKosinusFormel(int[,] block)
        {
            int n = block.GetLength(0);
            double isumsum = 0;

            int[,] idctBlock = new int[8, 8];

            for(int x = 0; x < 8; x++)
            {
                for(int y = 0; y < 8; y++)
                {
                    for(int i = 0; i < n; i++)
                    {
                        for(int j = 0; j < n; j++)
                        {
                            if (i == 0 && j == 0)
                            {
                                isumsum = isumsum + 2.0 / n * (1.0 / Math.Sqrt(2)) * (1.0 / Math.Sqrt(2)) * block[i, j] * Math.Cos(((2.0 * x + 1.0) * i * Math.PI) / (2.0 * n)) * Math.Cos(((2.0 * y + 1.0) * j * Math.PI) / (2.0 * n));
                            }
                            else if (i == 0 && j != 0)
                            {
                                isumsum = isumsum + 2.0 / n * (1.0 / Math.Sqrt(2)) * 1.0 * block[i, j] * Math.Cos(((2.0 * x + 1.0) * i * Math.PI) / (2.0 * n)) * Math.Cos(((2.0 * y + 1.0) * j * Math.PI) / (2.0 * n));
                            }
                            else if (i != 0 && j == 0)
                            {
                                isumsum = isumsum + 2.0 / n * 1.0 * (1.0 / Math.Sqrt(2)) * block[i, j] * Math.Cos(((2.0 * x + 1.0) * i * Math.PI) / (2.0 * n)) * Math.Cos(((2.0 * y + 1.0) * j * Math.PI) / (2.0 * n));
                            }
                            else
                            {
                                isumsum = isumsum + 2.0 / n * 1.0 * 1.0 * block[i, j] * Math.Cos(((2.0 * x + 1.0) * i * Math.PI) / (2.0 * n)) * Math.Cos(((2.0 * y + 1.0) * j * Math.PI) / (2.0 * n));
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
