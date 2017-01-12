using Microsoft.VisualStudio.TestTools.UnitTesting;
using JpegConverter.Quantisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegConverterTest
{
    [TestClass]
    public class ZickZackTest
    {
        private int[,] matrix =
        {
            {1 ,  2 ,  6 ,  7 , 15 , 16 , 28 , 29},
            {3 ,  5 ,  8 , 14 , 17 , 27 , 30 , 43},
            {4 ,  9 , 13 , 18 , 26 , 31 , 42 , 44},
            {10 , 12 , 19 , 25 , 32 , 41 , 45 , 54},
            {11 , 20 , 24 , 33 , 40 , 46 , 53 , 55},
            {21 , 23 , 34 , 39 , 47 , 52 , 56 , 61},
            {22 , 35 , 38 , 48 , 51 , 57 , 60 , 62},
            {36 , 37 , 49 , 50 , 58 , 59 , 63 , 64}
        };

        private int[] result = {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33,
                                   34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64};

        [TestMethod]
        public void testZickZack()
        {
            int[] zickZackArray = Quantisation.CreateZickZackSorting(matrix);

            for(int i = 0; i < 64; i++)
            {
                Assert.AreEqual(result[i], zickZackArray[i]);
            }
        }
    }
}
