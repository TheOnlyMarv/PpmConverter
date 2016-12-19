using JpegConverter.Huffman;
using JpegConverter.Jpeg;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using JpegConverter.DCT;
using JpegConverter.DCTPascal;
using System.Threading.Tasks;

namespace JpegConverter
{
    class Program
    {
        public static void Main(string[] args)
        {

            TestPerformanceDCT();

            //BitstreamTest.StartTests();
            //BitstreamTest.StartBenchmarking(10000000);
            //if (args.Length == 2)
            //{
            //    string sourcePath = args[0], destination = args[1];
            //    PPMImage image = LoadImageFromFile(sourcePath);
            //    YCbCrImage ycrcbMatrix = ConvertToYCbCr(image);
            //    Subsampling(ycrcbMatrix);
            //    RGBImage rgbImage = ConvertToRGB(ycrcbMatrix);
            //    JpegEncoder jpegEncoder = new JpegEncoder();
            //    ConvertToJpeg(jpegEncoder, image);
            //    SaveJpegIntoFile(jpegEncoder, destination);
            //}
            //Console.Read();

            //SortedList<float, Symbol> symbols = new SortedList<float, Symbol>(new JpegConverter.Huffman.DuplicateKeyComparer<float>());

            //symbols.Add(0.3f, 72);
            //symbols.Add(0.2f, 97);
            //symbols.Add(0.15f, 108);
            //symbols.Add(0.1f, 32);
            //symbols.Add(0.25f, 111);

            //symbols.Add(4.0f, 1);
            //symbols.Add(4.0f, 2);
            //symbols.Add(6.0f, 3);
            //symbols.Add(6.0f, 4);
            //symbols.Add(7.0f, 5);
            //symbols.Add(9.0f, 6);

            //JpegConverter.Huffman.HuffmanTree tree = JpegConverter.Huffman.HuffmanTree.createNormalTree(symbols);
            //JpegConverter.Huffman.HuffmanTree rightTree = JpegConverter.Huffman.HuffmanTree.createRightTree(tree);
            //tree.print();

            //Bitstream bitstream = new Bitstream();
            //tree.encode("Hallo", bitstream);
            //bitstream.Seek(0, System.IO.SeekOrigin.Begin);
            //String result = tree.decode(bitstream);

            //Console.WriteLine(result);

            //Console.Read();

        }

        private static void TestPerformanceDCT()
        {
            int[,] testImage = new int[256, 256];
            FillPixel(testImage);

            CosinusTransformation ct = new CosinusTransformation(testImage);
            Arai test = new Arai(testImage);

            int numOfTests = 30;

            TestDirectDCT(numOfTests, ct);
            TestSeperateDCT(numOfTests, ct);
            TestAraiDCT(numOfTests, ct);

            TestAraiDCTPascal(numOfTests, test);

            Console.ReadLine();
        }

        private static void TestAraiDCTPascal(int v, Arai test)
        {
            DateTime start = DateTime.Now;

            for (int i = 0; i < v; i++)
            {
                test.araiDCT();
            }

            DateTime end = DateTime.Now;

            TimeSpan time = TimeSpan.FromTicks((end.Ticks - start.Ticks) / v);

            Console.WriteLine("Avg time for pascal arai DCT over " + v + " times: " + time);
        }

        private static void TestAraiDCT(int v, CosinusTransformation ct)
        {
            DateTime start = DateTime.Now;

            for (int i = 0; i < v; i++)
            {
                ct.AraiDCT();
            }

            DateTime end = DateTime.Now;

            TimeSpan time = TimeSpan.FromTicks((end.Ticks - start.Ticks) / v);

            Console.WriteLine("Avg time for arai DCT over " + v + " times: " + time);
        }

        private static void TestSeperateDCT(int v, CosinusTransformation ct)
        {
            DateTime start = DateTime.Now;

            for (int i = 0; i < v; i++)
            {
                ct.SeperateDCT();
            }

            DateTime end = DateTime.Now;

            TimeSpan time = TimeSpan.FromTicks((end.Ticks - start.Ticks) / v);

            Console.WriteLine("Avg time for seperate DCT over " + v + " times: " + time);
        }

        private static void TestDirectDCT(int v, CosinusTransformation ct)
        {
            DateTime start = DateTime.Now;

            for (int i = 0; i < v; i++)
            {
                ct.DirectDCT();
            }
            
            DateTime end = DateTime.Now;

            TimeSpan  time = TimeSpan.FromTicks((end.Ticks - start.Ticks) / v);

            Console.WriteLine("Avg time for direct DCT over " + v + " times: " + time);
        }

        private static void FillPixel(int[,] testImage)
        {
            for (int x = 0; x < testImage.GetLength(0); x++)
            {
                for (int y = 0; y < testImage.GetLength(1); y++)
                {
                    testImage[x, y] = (x + y * 8) % 256;
                }
            }
        }

        private static void SaveJpegIntoFile(JpegEncoder jpegEncoder, string destination)
        {
            Console.Write("Save into file...");
            jpegEncoder.SaveIntoFile(destination);
            Console.WriteLine(" - DONE");
        }


        private static void ConvertToJpeg(JpegEncoder jpegEncoder, PPMImage image)
        {
            Console.Write("Convert to jpegStream...");
            jpegEncoder.WriteMarker(image, null); //Muss später noch gändert werden (NULL)
            Console.WriteLine(" - DONE");
        }


        private static void Subsampling(YCbCrImage ycrcbMatrix)
        {
            Console.Write("Subsampling...");
            //ycrcbMatrix.SubsampleMatrix();
            //ycrcbMatrix.ExtendMatrix();
            ycrcbMatrix.subsamplingCb();
            ycrcbMatrix.subsamplingCr();
            Console.WriteLine(" - DONE");
        }

        private static void SaveIntoFile(PPMImage image, string destinationPath)
        {
            Console.Write("Save new file...");
            image.SaveIntoFile(destinationPath);
            Console.WriteLine(" - DONE");
        }

        private static RGBImage ConvertToRGB(YCbCrImage ycrcbMatrix)
        {
            Console.Write("Converting back to RGB...");
            RGBImage rgbMatrix = RGBImage.FromYCbCr(ycrcbMatrix);
            Console.WriteLine(" - DONE");
            return rgbMatrix;
        }

        private static YCbCrImage ConvertToYCbCr(PPMImage image)
        {
            Console.Write("Converting to YCbCr...");
            YCbCrImage ycrcbMatrix = YCbCrImage.FromRGB(image.Matrix);
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
        #region DeadCode
        public static void Old_Main(string[] args)
        {
            string sourcePath, destinationPath;
            PPMImage image;
            YCbCrImage ycrcbMatrix;
            RGBImage rgbMatrix;
            try
            {
                switch (args.Length)
                {
                    case 0:
                        Console.WriteLine("Need a source parameter");
                        break;
                    case 1:
                        sourcePath = args[0];
                        image = LoadImageFromFile(sourcePath);
                        ycrcbMatrix = ConvertToYCbCr(image);
                        rgbMatrix = ConvertToRGB(ycrcbMatrix);
                        break;
                    case 2:
                        sourcePath = args[0];
                        destinationPath = args[1];
                        image = LoadImageFromFile(sourcePath);
                        ycrcbMatrix = ConvertToYCbCr(image);
                        Subsampling(ycrcbMatrix);
                        rgbMatrix = ConvertToRGB(ycrcbMatrix);
                        image.Matrix = rgbMatrix;
                        SaveIntoFile(image, destinationPath);
                        break;
                    default:
                        Console.WriteLine("Wrong number of arguments");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n{0}:\n{1}", ex.GetType().Name, ex.Message);
            }
#if DEBUG
            Console.ReadLine();
#endif
        }
        #endregion
    }
}
