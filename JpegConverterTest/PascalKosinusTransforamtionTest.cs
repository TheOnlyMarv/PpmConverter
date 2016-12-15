using Microsoft.VisualStudio.TestTools.UnitTesting;
using JpegConverter.DCTPascal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegConverterTest
{
    [TestClass]
    public class PascalKosinusTransforamtionTest
    {
        [TestMethod]
        public void TestDirectDCT()
        {
            int[,] pixel = { { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 } };        
            int[,] result = { { 2040, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 } };
            int[,] dct = KosinusTransformation.direkteKosinusTransformation(pixel);

            for (int i = 0; i < dct.GetLength(0); i++)
            {
                for(int j = 0; j < dct.GetLength(1); j++)
                {
                    Assert.AreEqual(result[i, j], dct[i, j]);
                }
            }
        }

       [TestMethod]
       public void TestDirectDCT2()
        {
            int[,] pixel = { { 28, 34, 20, 18, 22, 17, 17, 17 }, { 31, 36, 20, 31, 166, 184, 177, 140 }, { 16, 17, 17, 95, 197, 185, 152, 161 }, { 25, 20, 21, 42, 43, 41, 99, 150 }, { 19, 18, 19, 71, 63, 99, 98, 63 }, { 81, 103, 90, 118, 26, 31, 23, 22 }, { 161, 160, 163, 148, 142, 146, 155, 96 }, { 158, 139, 153, 148, 154, 155, 142, 35 } };
            int[,] result = { { 680, -118, -44, 63, -8, 9, -24, 0 }, { -181, -187, 27, 26, 33, -59, 2, 13 }, { 102, 107, -26, 44, -41, 1, 2, 28 }, { -180, 141, 24, -61, -14, 21, 0, -34 }, { -143, 43, 28, -28, -1, 27, 1, -2 }, { -12, 54, 43, -64, 22, 1, 1, 0 }, { -77, 64, -40, -23, 26, 1, 1, -26 }, { 59, -47, -50, 47, -2, -1, 2, 0 } };


            int[,] dct = KosinusTransformation.direkteKosinusTransformation(pixel);

            for (int i = 0; i < dct.GetLength(0); i++)
            {
                for (int j = 0; j < dct.GetLength(1); j++)
                {
                    Assert.AreEqual(result[i, j], dct[i, j]);
                }
            }
        }

        [TestMethod]
        public void TestSeperiteDCT()
        {
            int[,] pixel = { { 2, 2 }, { 2, 2 } };
            int[,] result = { { 4, 0 }, { 0, 0 } };

            //int[,] pixel = { { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 } };
            //int[,] result = { { 2040, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 } };

            int[,] sdct = KosinusTransformation.seperierteKosinusTranformation(pixel);

            for (int i = 0; i < sdct.GetLength(0); i++)
            {
                for (int j = 0; j < sdct.GetLength(1); j++)
                {
                    Assert.AreEqual(result[i, j], sdct[i, j]);
                }
            }
        }


        [TestMethod]
        public void TestInverseDCT()
        {
            int[,] pixel = { { 2040, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 } };
            int[,] result = { { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 } };            
            int[,] idct = KosinusTransformation.inverseDiskreteKosinusTransformation(pixel);
            
            for (int i = 0; i < idct.GetLength(0); i++)
            {
                for (int j = 0; j < idct.GetLength(1); j++)
                {
                    Assert.AreEqual(result[i, j], idct[i, j]);
                }
            }
        }

    }
}
