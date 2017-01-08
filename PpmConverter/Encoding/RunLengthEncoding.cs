using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegConverter.Encoding
{
    public static class RunLengthEncoding
    {
        public static List<KeyValuePair<byte, int[]>> CreateAcRLE(int[] block)
        {
            List<KeyValuePair<byte, int>> runLengthPairs = CreateRunLengthPairs(block);
            List<int[]> bitPattern = CreateBitPatternAndCategory(runLengthPairs);
            return null;
        }

        private static List<int[]> CreateBitPatternAndCategory(List<KeyValuePair<byte, int>> runLengthPairs)
        {
            List<int[]> bitPattern = new List<int[]>();
            for (int i = 0; i < runLengthPairs.Count; i++)
            {
                int number = runLengthPairs[i].Value;
                int category = GetCategory(number);
                runLengthPairs[i] = new KeyValuePair<byte, int>(runLengthPairs[i].Key, category);

                //TODO BitMuster noch erstellen
            }
            return bitPattern;
        }

        private static int GetCategory(int number)
        {
            return (int)(Math.Log(Math.Abs(number), 2) + 1);
        }

        private static List<KeyValuePair<byte, int>> CreateRunLengthPairs(int[] block)
        {
            List<KeyValuePair<byte, int>> pairs = new List<KeyValuePair<byte, int>>();
            byte zeroCounter = 0;
            for (int i = 1; i < block.Length; i++)
            {
                if (block[i] == 0)
                {
                    zeroCounter++;
                }
                else
                {
                    pairs.Add(new KeyValuePair<byte, int>(zeroCounter, block[i]));
                    zeroCounter = 0;
                }

                if (zeroCounter > 15)
                {
                    pairs.Add(new KeyValuePair<byte, int>(zeroCounter, 0));
                    zeroCounter = 0;
                }
            }
            pairs.Add(new KeyValuePair<byte, int>(0, 0));

            return pairs;
        }
    }
}
