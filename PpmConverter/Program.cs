using JpegConverter.Huffman;
using JpegConverter.Jpeg;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using JpegConverter.DCT;
using System.Threading.Tasks;
using JpegConverter.Encoding;

namespace JpegConverter
{
    class Program
    {
        public static void Main(string[] args)
        {


            if (args.Length == 1 && string.Compare(args[0], "-test", true) == 0)
            {
                TestPerformanceDCT();
            }
            else if (args.Length == 2)
            {
                PPMImage image = LoadImageFromFile(args[0]);
                ImageEncoder encoder = new ImageEncoder(image.Matrix);
                encoder.WriteToJpegToFile(image, args[1]);
            }

        }

        private static Dictionary<int, int> GenerateTestHuffmanSymbols()
        {
            Dictionary<int, int> sl = new Dictionary<int, int>();
            sl.Add(1, 4);
            sl.Add(2, 4);
            sl.Add(3, 6);
            sl.Add(4, 6);
            sl.Add(5, 7);
            sl.Add(6, 9);
            return sl;
        }

        #region DCTPerformanceTests
        private static void TestPerformanceDCT()
        {
            int secondsForTesting = 10;

            TestDirectDCT(secondsForTesting);
            TestSeperateDCT(secondsForTesting);
            TestAraiDCT(secondsForTesting);

            Console.ReadLine();
        }


        private static void TestAraiDCT(int seconds)
        {
            long start = DateTime.Now.Ticks;
            int counter = 0;
            Stopwatch sp = new Stopwatch();

            double[,] testImage = new double[256, 256];
            while (DateTime.Now.Ticks - start < seconds * 10000000)
            {
                FillPixel(testImage);

                sp.Start();
                CosinusTransformation.AraiDCT(testImage);
                sp.Stop();
                counter++;
            }


            Console.WriteLine("Avg time for arai over " + counter + " times: " + sp.Elapsed.TotalMilliseconds / counter);
        }

        private static void TestSeperateDCT(int seconds)
        {
            long start = DateTime.Now.Ticks;
            int counter = 0;
            Stopwatch sp = new Stopwatch();

            double[,] testImage = new double[256, 256];
            while (DateTime.Now.Ticks - start < seconds * 10000000)
            {
                FillPixel(testImage);

                sp.Start();
                CosinusTransformation.SeperateDCT(testImage);
                sp.Stop();
                counter++;
            }


            Console.WriteLine("Avg time for seperate DCT over " + counter + " times: " + sp.Elapsed.TotalMilliseconds / counter);
        }

        private static void TestDirectDCT(int seconds)
        {
            long start = DateTime.Now.Ticks;
            int counter = 0;
            Stopwatch sp = new Stopwatch();

            double[,] testImage = new double[256, 256];
            while (DateTime.Now.Ticks - start < seconds * 10000000)
            {
                sp.Start();
                CosinusTransformation.DirectDCT(testImage);
                sp.Stop();
                counter++;
            }


            Console.WriteLine("Avg time for direct DCT over " + counter + " times: " + sp.Elapsed.TotalMilliseconds / counter);
        }

        private static void FillPixel(double[,] testImage)
        {
            for (int x = 0; x < testImage.GetLength(0); x++)
            {
                for (int y = 0; y < testImage.GetLength(1); y++)
                {
                    testImage[x, y] = (x + y * 8) % 256;
                }
            }
        }
        #endregion


        private static void SaveJpegIntoFile(JpegEncoder jpegEncoder, string destination)
        {
            Console.Write("Save into file...");
            jpegEncoder.SaveIntoFile(destination);
            Console.WriteLine(" - DONE");
        }


        private static void ConvertToJpeg(JpegEncoder jpegEncoder, PPMImage image)
        {
            Console.Write("Convert to jpegStream...");
            jpegEncoder.WriteMarker(image, null, null); //Muss später noch gändert werden (NULL)
            Console.WriteLine(" - DONE");
        }


        private static void Subsampling(Image ycrcbMatrix)
        {
            Console.Write("Subsampling...");
            //ycrcbMatrix.SubsampleMatrix();
            //ycrcbMatrix.ExtendMatrix();
            ycrcbMatrix.subsamplingChannel2();
            ycrcbMatrix.subsamplingChannel1();
            Console.WriteLine(" - DONE");
        }

        private static void SaveIntoFile(PPMImage image, string destinationPath)
        {
            Console.Write("Save new file...");
            image.SaveIntoFile(destinationPath);
            Console.WriteLine(" - DONE");
        }


        private static Image ConvertToYCbCr(PPMImage image)
        {
            Console.Write("Converting to YCbCr...");
            Image ycrcbMatrix = Image.FromRGBtoYCbCr(image.Matrix);
            Console.WriteLine(" - DONE");
            return ycrcbMatrix;
        }

        public static PPMImage LoadImageFromFile(string path)
        {
            Console.Write("Load file...");
            PPMImage image = PPMImage.LoadImageFromFile(path);
            Console.WriteLine(" - DONE");
            return image;
        }
    }
}
