using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JpegConverter.DCT;

namespace JpegConverterTest
{
    [TestClass]
    public class CosinusTransformaionTest
    {
        int[,] test1 = {
                { 255, 255, 255, 255, 255, 255, 255, 255 },
                { 255, 255, 255, 255, 255, 255, 255, 255 },
                { 255, 255, 255, 255, 255, 255, 255, 255 },
                { 255, 255, 255, 255, 255, 255, 255, 255 },
                { 255, 255, 255, 255, 255, 255, 255, 255 },
                { 255, 255, 255, 255, 255, 255, 255, 255 },
                { 255, 255, 255, 255, 255, 255, 255, 255 },
                { 255, 255, 255, 255, 255, 255, 255, 255 }
            };
        int[,] result1 = {
                { 2040, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 }
            };

        int[,] test2 = { 
            { 28, 34, 20, 18, 22, 17, 17, 17 }, 
            { 31, 36, 20, 31, 166, 184, 177, 140 }, 
            { 16, 17, 17, 95, 197, 185, 152, 161 }, 
            { 25, 20, 21, 42, 43, 41, 99, 150 }, 
            { 19, 18, 19, 71, 63, 99, 98, 63 }, 
            { 81, 103, 90, 118, 26, 31, 23, 22 }, 
            { 161, 160, 163, 148, 142, 146, 155, 96 }, 
            { 158, 139, 153, 148, 154, 155, 142, 35 }
        };
        int[,] result2 = { 
            { 680, -118, -44, 63, -8, 9, -24, 0 }, 
            { -181, -187, 27, 26, 33, -59, 2, 13 }, 
            { 102, 107, -26, 44, -41, 1, 2, 28 }, 
            { -180, 141, 24, -61, -14, 21, 0, -34 }, 
            { -143, 43, 28, -28, -1, 27, 1, -2 }, 
            { -12, 54, 43, -64, 22, 1, 1, 0 }, 
            { -77, 64, -40, -23, 26, 1, 1, -26 }, 
            { 59, -47, -50, 47, -2, -1, 2, 0 }
        };

        [TestMethod]
        public void TestDirectDCT()
        {
            CosinusTransformation ct = new CosinusTransformation(test1);
            int[,] calculated = ct.DirectDCT();
            for (int x = 0; x < test1.GetLength(0); x++)
            {
                for (int y = 0; y < test1.GetLength(1); y++)
                {
                    Assert.AreEqual(result1[x, y], calculated[x, y]);
                }
            }

            ct = new CosinusTransformation(test2);
            calculated = ct.DirectDCT();
            for (int x = 0; x < test2.GetLength(0); x++)
            {
                for (int y = 0; y < test2.GetLength(1); y++)
                {
                    Assert.AreEqual(result2[x, y], calculated[x, y]);
                }
            }
        }

        [TestMethod]
        public void TestSeperateDCT()
        {
            CosinusTransformation ct = new CosinusTransformation(test1);
            int[,] calculated = ct.SeperateDCT();

            for (int i = 0; i < test1.GetLength(0); i++)
            {
                for (int j = 0; j < test1.GetLength(1); j++)
                {
                    Assert.AreEqual(result1[i, j], calculated[i, j]);
                }
            }

            ct = new CosinusTransformation(test2);
            calculated = ct.SeperateDCT();
            for (int x = 0; x < test2.GetLength(0); x++)
            {
                for (int y = 0; y < test2.GetLength(1); y++)
                {
                    Assert.AreEqual(result2[x, y], calculated[x, y]);
                }
            }
        }

        [TestMethod]
        public void TestInvereDCT()
        {
            CosinusTransformation ct = new CosinusTransformation(result1);
            int[,] calculated = ct.InverseDirectDCT();

            for (int i = 0; i < result1.GetLength(0); i++)
            {
                for (int j = 0; j < result1.GetLength(1); j++)
                {
                    Assert.AreEqual(test1[i, j], calculated[i, j]);
                }
            }
        }
    }
}
