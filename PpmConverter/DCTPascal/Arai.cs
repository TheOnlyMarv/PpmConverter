using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegConverter.DCTPascal
{
    public class Arai
    {
        private const int BLOCK_SIZE = 8;
        private const double ONE_DIV_SQRT2 = 0.70710678118654746;
        private  int[,] OriginalImage { get; set; }
        private int[,] TransformedImage { get; set; }

        public Arai(int[,] image)
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

        private  int[,] MergeBlockIntoImage(List<int[,]> blocks)
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

        public int[,] araiDCT()
        {
            List<int[,]> blocks = SplitImageIntoBlocks();


            for (int bId = 0; bId < blocks.Count; bId++)
            {
                int[,] blockx = blocks[bId];

                double[,] araiBlock = new double[8, 8];
                int[,] result = new int[8, 8];
                double[] temp = new double[8];

                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        temp[j] = blockx[j, i];
                    }
                    temp = step1(temp);
                    temp = step2(temp);
                    temp = step3(temp);
                    temp = step4(temp);
                    temp = step5(temp);
                    temp = step6(temp);
                    temp = step7(temp);

                    for (int n = 0; n < 8; n++)
                    {
                        araiBlock[n, i] = temp[n];
                    }
                }

                for (int k = 0; k < 8; k++)
                {
                    for (int l = 0; l < 8; l++)
                    {
                        temp[l] = araiBlock[k, l];
                    }
                    temp = step1(temp);
                    temp = step2(temp);
                    temp = step3(temp);
                    temp = step4(temp);
                    temp = step5(temp);
                    temp = step6(temp);
                    temp = step7(temp);

                    for (int m = 0; m < 8; m++)
                    {
                        result[k, m] = (int)Math.Round(temp[m]);
                    }
                }

                blocks[bId] = result;
            }

            TransformedImage =  MergeBlockIntoImage(blocks);
            return TransformedImage;
        }

        private static double[] step1(double[] block)
        {
            double[] result = new double[8];

            result[0] = block[0] + block[7];
            result[1] = block[1] + block[6];
            result[2] = block[2] + block[5];
            result[3] = block[3] + block[4];
            result[4] = block[3] - block[4];
            result[5] = block[2] - block[5];
            result[6] = block[1] - block[6];
            result[7] = block[0] - block[7];

            return result;
        }

        private static double[] step2(double[] block)
        {
            double[] result = new double[8];

            result[0] = block[0] + block[3];
            result[1] = block[1] + block[2];
            result[2] = block[1] - block[2];
            result[3] = block[0] - block[3];
            result[4] = -1 * (block[4] + block[5]);
            result[5] = block[5] + block[6];
            result[6] = block[6] + block[7];
            result[7] = block[7];

            return result;
        }

        private static double[] step3(double[] block)
        {
            double[] result = new double[8];

            result[0] = block[0] + block[1];
            result[1] = block[0] - block[1];
            result[2] = block[2] + block[3];
            result[3] = block[3];
            result[4] = block[4];
            result[5] = block[5];
            result[6] = block[6];
            result[7] = block[7];

            return result;
        }

        private static double[] step4(double[] block)
        {
            double[] result = new double[8];
            double m1 = Math.Cos(4.0 * Math.PI / 16.0);
            double m2 = Math.Cos(6.0 * Math.PI / 16.0);
            double m3 = Math.Cos(2.0 * Math.PI / 16.0) - Math.Cos(6.0 * Math.PI / 16.0);
            double m4 = Math.Cos(2.0 * Math.PI / 16.0) + Math.Cos(6.0 * Math.PI / 16.0);

            result[0] = block[0];
            result[1] = block[1];
            result[2] = m1 * block[2];
            result[3] = block[3];
            result[4] = -1 * (m3 * block[4]) - ((block[4] + block[6]) * m2);
            result[5] = m1 * block[5];
            result[6] = (m4 * block[6]) - ((block[4] + block[6]) * m2);
            result[7] = block[7];

            return result;
        }

        private static double[] step5(double[] block)
        {
            double[] result = new double[8];

            result[0] = block[0];
            result[1] = block[1];
            result[2] = block[2] + block[3];
            result[3] = block[3] - block[2];
            result[4] = block[4];
            result[5] = block[5] + block[7];
            result[6] = block[6];
            result[7] = block[7] - block[5];

            return result;
        }

        private static double[] step6(double[] block)
        {
            double[] result = new double[8];

            result[0] = block[0];
            result[1] = block[1];
            result[2] = block[2];
            result[3] = block[3];
            result[4] = block[4] + block[7];
            result[5] = block[5] + block[6];
            result[6] = block[5] - block[6];
            result[7] = block[7] - block[4];

            return result;
        }

        private static double[] step7(double[] block)
        {
            double[] result = new double[8];

            double s0 = 1.0 / (2.0 * Math.Sqrt(2.0));
            double s1 = 1.0 / (4.0 * Math.Cos(1.0 * Math.PI / 16.0));
            double s2 = 1.0 / (4.0 * Math.Cos(2.0 * Math.PI / 16.0));
            double s3 = 1.0 / (4.0 * Math.Cos(3.0 * Math.PI / 16.0));
            double s4 = 1.0 / (4.0 * Math.Cos(4.0 * Math.PI / 16.0));
            double s5 = 1.0 / (4.0 * Math.Cos(5.0 * Math.PI / 16.0));
            double s6 = 1.0 / (4.0 * Math.Cos(6.0 * Math.PI / 16.0));
            double s7 = 1.0 / (4.0 * Math.Cos(7.0 * Math.PI / 16.0));

            result[0] = s0 * block[0];
            result[1] = s1 * block[5];
            result[2] = s2 * block[2];
            result[3] = s3 * block[7];
            result[4] = s4 * block[1];
            result[5] = s5 * block[4];
            result[6] = s6 * block[3];
            result[7] = s7 * block[6];

            return result;
        }
    }
}
