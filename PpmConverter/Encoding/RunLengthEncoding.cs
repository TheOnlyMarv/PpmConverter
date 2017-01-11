using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegConverter.Encoding
{
    public static class RunLengthEncoding
    {
        private static int BLOCK_SIZE = 8;

        #region AC RLE
        public static List<RunLengthAcPair> CreateAcRLE(int[] block)
        {
            List<RunLengthAcPair> runLengthPairs = CreateRunLengthPairs(block);
            CreateBitPatternAndCategory(runLengthPairs);
            CreatePairAsByte(runLengthPairs);
            return runLengthPairs;
        }

        private static void CreatePairAsByte(List<RunLengthAcPair> runLengthPairs)
        {
            foreach (var item in runLengthPairs)
            {
                item.PairAsByte = (byte)((item.Zeros << 4) | item.Category);
            }
        }

        private static void CreateBitPatternAndCategory(List<RunLengthAcPair> runLengthPairs)
        {
            for (int i = 0; i < runLengthPairs.Count; i++)
            {
                int number = runLengthPairs[i].Koeffizient;
                byte category = GetCategory(number);
                runLengthPairs[i].Category = category;

                if (number == 0)
                {
                    runLengthPairs[i].BitPattern = null;
                }
                else
                {
                    runLengthPairs[i].BitPattern = CreateBitPattern(CreateBitNumber(category, number), category);
                }
            }
        }

        private static int CreateBitNumber(byte category, int number)
        {
            int lowerBound = -(int)(Math.Pow(2, category) - 1);
            int bitNumber = number < 0 ? Math.Abs(lowerBound - number) : number;

            return bitNumber;
        }

        private static int[] CreateBitPattern(int bitNumber, int numBits)
        {
            int[] result = new int[numBits];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (bitNumber >> numBits - 1 -i) & 1; 
            }
            return result;
        }

        private static byte GetCategory(int number)
        {
            return (byte)(Math.Log(Math.Abs(number), 2) + 1);
        }

        private static List<RunLengthAcPair> CreateRunLengthPairs(int[] block)
        {
            List<RunLengthAcPair> pairs = new List<RunLengthAcPair>();
            byte zeroCounter = 0;
            for (int i = 1; i < block.Length; i++)
            {
                if (block[i] == 0)
                {
                    zeroCounter++;
                }
                else if (zeroCounter > 15)
                {
                    int newPairs = zeroCounter / 16;
                    for (int j = 0; j < newPairs; j++)
                    {
                        pairs.Add(new RunLengthAcPair() { Zeros = zeroCounter, Koeffizient = 0 });
                        zeroCounter -= 16;
                    }
                }
                else
                {
                    pairs.Add(new RunLengthAcPair() { Zeros = zeroCounter, Koeffizient = block[i] });
                    zeroCounter = 0;
                }


            }
            pairs.Add(new RunLengthAcPair() { Zeros = 0, Koeffizient = 0 });

            return pairs;
        }
        #endregion

        #region DC RLE
        public static RunLengthDcPair CreateDcRLE(int[,] channel)
        {
            int difference = 0;
            int blocksEachRow = channel.GetLength(0) / BLOCK_SIZE;
            for (int bId = 0; bId < (channel.GetLength(0) * channel.GetLength(1)) / BLOCK_SIZE / BLOCK_SIZE; bId++)
            {
                int offsetX = (bId % blocksEachRow) * 8;
                int offsetY = (bId / blocksEachRow) * 8;

                difference -= channel[offsetX, offsetY];
            }

            byte category = GetCategory(difference);
            return new RunLengthDcPair() { Difference = difference, Category = GetCategory(difference), BitPattern = CreateBitPattern(CreateBitNumber(category, difference), category) };
        }
        #endregion
    }

    public class RunLengthAcPair
    {
        public int Zeros { get; set; }
        public int Koeffizient { get; set; }
        public bool EOB { get { return Zeros == 0 && Koeffizient == 0; } }
        public byte Category { get; set; }
        public int[] BitPattern { get; set; }
        public byte PairAsByte { get; set; }
    }

    public class RunLengthDcPair
    {
        public int Difference { get; set; }
        public byte Category { get; set; }
        public int[] BitPattern { get; set; }
    }
}
