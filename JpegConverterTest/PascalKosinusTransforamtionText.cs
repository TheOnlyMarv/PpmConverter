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
    class PascalKosinusTransforamtionText
    {

        [TestMethod]
        public void directdctTest()
        {
            int[,] pixel = { { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 } };        
            int[,] result = { { 2040, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 } };
            int[,] dct = DCTPascal.KosinusTransformation.direkteKosinusTransformation(pixel);

            for (int i = 0; i < dct.GetLength(0); i++)
            {
                for(int j = 0; j < dct.GetLength(1); j++)
                {
                    Assert.AreEqual(result[i, j], dct[i, j]);
                }
            }
        }

        [TestMethod]
        public void inversedctTest()
        {
            int[,] pixel = { { 2040, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0 } };
            int[,] result = { { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 }, { 255, 255, 255, 255, 255, 255, 255, 255 } };            
            int[,] idct = DCTPascal.KosinusTransformation.direkteKosinusTransformation(pixel);
            
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
