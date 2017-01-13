using JpegConverter.Encoding;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegConverterTest
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class RunLengthEncodingTest
    {
        int[] test = {
            12, 57,45,0,0,0,0,23,
            0,-30,-16,0,0,1,0,0,
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,
        };

        List<RunLengthAcPair> right = new List<RunLengthAcPair>()
        {
            new RunLengthAcPair() {Zeros = 0, Koeffizient = 57, Category = 6, BitPattern = new int[] { 1,1,1,0,0,1 }, PairAsByte=0x06},
            new RunLengthAcPair() {Zeros = 0, Koeffizient = 45, Category = 6, BitPattern = new int[] { 1,0,1,1,0,1 }, PairAsByte=0x06},
            new RunLengthAcPair() {Zeros = 4, Koeffizient = 23, Category = 5, BitPattern = new int[] {1,0,1,1,1 }, PairAsByte=0x45},
            new RunLengthAcPair() {Zeros = 1, Koeffizient = -30, Category = 5, BitPattern = new int[] {0,0,0,0,1 }, PairAsByte=0x15},
            new RunLengthAcPair() {Zeros = 0, Koeffizient = -16, Category = 5, BitPattern = new int[] {0,1,1,1,1 }, PairAsByte=0x05},
            new RunLengthAcPair() {Zeros = 2, Koeffizient = 1, Category = 1, BitPattern = new int[] {1 }, PairAsByte=0x21},
            new RunLengthAcPair() {Zeros = 0, Koeffizient = 0, Category = 0, BitPattern = null, PairAsByte=0x00}
        };



        int[,] test2 = {
            { 28, 34, 20, 18, 22, 17, 17, 17,          36, 34, 20, 18, 22, 17, 17, 17 },
            { 31, 36, 20, 31, 166, 184, 177, 140,           28, 34, 20, 18, 22, 17, 17, 17 },
            { 16, 17, 17, 95, 197, 185, 152, 161,         28, 34, 20, 18, 22, 17, 17, 17 },
            { 25, 20, 21, 42, 43, 41, 99, 150,            28, 34, 20, 18, 22, 17, 17, 17 },
            { 19, 18, 19, 71, 63, 99, 98, 63,            28, 34, 20, 18, 22, 17, 17, 17},
            { 81, 103, 90, 118, 26, 31, 23, 22,            28, 34, 20, 18, 22, 17, 17, 17 },
            { 161, 160, 163, 148, 142, 146, 155, 96,            28, 34, 20, 18, 22, 17, 17, 17 },
            { 158, 139, 153, 148, 154, 155, 142, 35,            28, 34, 20, 18, 22, 17, 17, 17 },

            { 142, 34, 20, 18, 22, 17, 17, 17,          48, 34, 20, 18, 22, 17, 17, 17 },
            { 31, 36, 20, 31, 166, 184, 177, 140,           28, 34, 20, 18, 22, 17, 17, 17 },
            { 16, 17, 17, 95, 197, 185, 152, 161,         28, 34, 20, 18, 22, 17, 17, 17 },
            { 25, 20, 21, 42, 43, 41, 99, 150,            28, 34, 20, 18, 22, 17, 17, 17 },
            { 19, 18, 19, 71, 63, 36, 98, 63,            28, 34, 20, 18, 22, 17, 17, 17},
            { 81, 103, 90, 118, 26, 31, 23, 22,            28, 34, 20, 18, 22, 17, 17, 17 },
            { 161, 160, 163, 148, 142, 146, 155, 96,            28, 34, 20, 18, 22, 17, 17, 17 },
            { 158, 139, 153, 148, 154, 155, 142, 35,            28, 34, 20, 18, 22, 17, 17, 17 }
        };

        List<RunLengthDcPair> right2 = new List<RunLengthDcPair>()
        {
            new RunLengthDcPair()
            {
                Difference = 28,
                Category = 5,
                BitPattern = new int[] { 1,1,1,0,0 }

            },

            new RunLengthDcPair()
            {
                Difference = 8,
                Category = 4,
                BitPattern = new int[] { 1,0,0,0 }

            },
            new RunLengthDcPair()
            {
                Difference = 106,
                Category = 7,
                BitPattern = new int[] { 1,1,0,1,0,1,0 }

            },
            new RunLengthDcPair()
            {
                Difference = -94,
                Category = 7,
                BitPattern = new int[] { 0,1,0,0,0,0,1 }

            }
        };


        [TestMethod]
        public void TestRunLengthEncodingForAC()
        {
            List<RunLengthAcPair> result = RunLengthEncoding.CreateAcRLE(test);
            Assert.IsTrue(result.Count == right.Count);

            for (int i = 0; i < right.Count; i++)
            {
                RunLengthAcPair rightPair = right[i];
                RunLengthAcPair resultPair = result[i];
                Assert.AreEqual(rightPair.Zeros, resultPair.Zeros, String.Format("Zeros Expected: {0} but was {1}", rightPair.Zeros, resultPair.Zeros));
                Assert.AreEqual(rightPair.Koeffizient, resultPair.Koeffizient, String.Format("Koeffizient Expected: {0} but was {1}", rightPair.Koeffizient, resultPair.Koeffizient));
                Assert.AreEqual(rightPair.Category, resultPair.Category, String.Format("Category Expected: {0} but was {1}", rightPair.Category, resultPair.Category));
                Assert.AreEqual(rightPair.PairAsByte, resultPair.PairAsByte, String.Format("PairAsByte Expected: {0} but was {1}", rightPair.PairAsByte, resultPair.PairAsByte));
                if (rightPair.BitPattern == null && resultPair.BitPattern == null)
                {
                    Assert.IsTrue(true);
                }
                else
                {
                    for (int j = 0; j < rightPair.BitPattern.Length; j++)
                    {
                        Assert.AreEqual(rightPair.BitPattern[j], resultPair.BitPattern[j], String.Format("Bit Expected: {0} but was {1}", rightPair.BitPattern[j], resultPair.BitPattern[j]));
                    }
                }
            }
        }

        [TestMethod]
        public void TestRunLengthEncodingForDC()
        {
            List<RunLengthDcPair> result = RunLengthEncoding.CreateDcRLE(test2);
            Assert.IsTrue(result.Count == right2.Count);


            for (int i = 0; i < result.Count; i++)
            {
                Assert.AreEqual(right2[i].Difference, result[i].Difference, String.Format("Difference Expected: {0} but was {1}", right2[i].Difference, result[i].Difference));
                Assert.AreEqual(right2[i].Category, result[i].Category, String.Format("Category Expected: {0} but was {1}", right2[i].Category, result[i].Category));
                for (int j = 0; j < right2[i].BitPattern.Length; j++)
                {
                    Assert.AreEqual(right2[i].BitPattern[j], result[i].BitPattern[j], String.Format("Bit Expected: {0} but was {1}", right2[i].BitPattern[j], result[i].BitPattern[j]));
                }
            }
        }
    }
}
