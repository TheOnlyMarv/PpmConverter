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
        double[,] test1 = {
                { 255, 255, 255, 255, 255, 255, 255, 255 },
                { 255, 255, 255, 255, 255, 255, 255, 255 },
                { 255, 255, 255, 255, 255, 255, 255, 255 },
                { 255, 255, 255, 255, 255, 255, 255, 255 },
                { 255, 255, 255, 255, 255, 255, 255, 255 },
                { 255, 255, 255, 255, 255, 255, 255, 255 },
                { 255, 255, 255, 255, 255, 255, 255, 255 },
                { 255, 255, 255, 255, 255, 255, 255, 255 }
            };
        double[,] result1 = {
                { 2040, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 }
            };

        double[,] test2 = { 
            { 47,18,13,16,41,90,47,27 }, 
            { 62,42,35,39,66,90,41,26 }, 
            { 71,55,56,67,55,40,22,39 }, 
            { 53,60,63,50,48,25,37,87 }, 
            { 31,27,33,27,37,50,81,147}, 
            { 54,31,33,46,58,104,144,179 }, 
            { 76,70,71,91,118,151,176,184 }, 
            { 102,105,115,124,135,168,173,181 }
        };
        double[,] result2 = { 
            { 581,-144,56,17,15,-7,25,-9 }, 
            { -242,133,-48,42,-2,-7,13,-4 }, 
            { 108,-18,-40,71,-33,12,6,-10 }, 
            { -56,-93,48,19,-8,7,6,-2 }, 
            { -17,9,7,-23,-3,-10,5,3 }, 
            { 4,9,-4,-5,2,2,-7,3 }, 
            { -9,7,8,-6,5,12,2,-5 }, 
            { -9,-4,-2,-3,6,1,-1,-1 }
        };

        [TestMethod]
        public void TestDirectDCT()
        {
            double[,] calculated = CosinusTransformation.DirectDCT(test1);
            for (int i = 0; i < test1.GetLength(0); i++)
            {
                for (int j = 0; j < test1.GetLength(1); j++)
                {
                    Assert.IsTrue(result1[i, j] - 1 < calculated[i, j] && result1[i, j] + 1 > calculated[i, j], "Value must be greater {0} and smaller {1}. But was: {2}", result1[i, j] - 1, result1[i, j] + 1, calculated[i, j]);
                }
            }

            calculated = CosinusTransformation.DirectDCT(test2);
            for (int i = 0; i < test2.GetLength(0); i++)
            {
                for (int j = 0; j < test2.GetLength(1); j++)
                {
                    Assert.IsTrue(result2[i, j] - 1 < calculated[i, j] && result2[i, j] + 1 > calculated[i, j], "Value must be greater {0} and smaller {1}. But was: {2}", result2[i, j] - 1, result2[i, j] + 1, calculated[i, j]);
                }
            }
        }

        [TestMethod]
        public void TestSeperateDCT()
        {
            double[,] calculated = CosinusTransformation.SeperateDCT(test1);

            for (int i = 0; i < test1.GetLength(0); i++)
            {
                for (int j = 0; j < test1.GetLength(1); j++)
                {
                    Assert.IsTrue(result1[i, j] - 1 < calculated[i, j] && result1[i, j] + 1 > calculated[i, j], "Value must be greater {0} and smaller {1}. But was: {2}", result1[i, j] - 1, result1[i, j] + 1, calculated[i, j]);
                }
            }

            calculated = CosinusTransformation.SeperateDCT(test2);
            for (int i = 0; i < test2.GetLength(0); i++)
            {
                for (int j = 0; j < test2.GetLength(1); j++)
                {
                    Assert.IsTrue(result2[i, j] - 1 < calculated[i, j] && result2[i, j] + 1 > calculated[i, j], "Value must be greater {0} and smaller {1}. But was: {2}", result2[i, j] - 1, result2[i, j] + 1, calculated[i, j]);
                }
            }
        }

        [TestMethod]
        public void TestInvereDCT()
        {
            double[,] calculated = CosinusTransformation.InverseDirectDCT(result1);

            for (int i = 0; i < result1.GetLength(0); i++)
            {
                for (int j = 0; j < result1.GetLength(1); j++)
                {
                    Assert.IsTrue(test1[i, j] - 1 < calculated[i, j] && test1[i, j] + 1 > calculated[i, j], "Value must be greater {0} and smaller {1}. But was: {2}", test1[i, j] - 1, test1[i, j] + 1, calculated[i, j]);
                }
            }
        }

        [TestMethod]
        public void TestAraiDCT()
        {
            double[,] calculated = CosinusTransformation.AraiDCT(test2);

            for (int i = 0; i < test2.GetLength(0); i++)
            {
                for (int j = 0; j < test2.GetLength(1); j++)
                {
                    Assert.IsTrue(result2[i, j] - 1 < calculated[i, j] && result2[i, j] + 1 > calculated[i, j], "Value must be greater {0} and smaller {1}. But was: {2}", result2[i, j] - 1, result2[i, j] + 1, calculated[i, j]);
                }
            }
        }
    }
}
