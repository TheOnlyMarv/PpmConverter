using JpegConverter.Huffman;
using JpegConverter.Jpeg;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using JpegConverter.DCT;
using System.Threading.Tasks;

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
            else if (args.Length == 1)
            {
                JpegEncoder encoder = new JpegEncoder();
                PPMImage image = LoadImageFromFile(args[0]);
                Dictionary<int, int> sl = GenerateTestHuffmanSymbols();
                Huffman.Huffman huffman = new Huffman.Huffman(sl);
                huffman.CreateLimitedHuffman(16, true);
                encoder.WriteMarker(image, huffman);
                encoder.SaveIntoFile("image.data");
            }

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
            jpegEncoder.WriteMarker(image, null); //Muss später noch gändert werden (NULL)
            Console.WriteLine(" - DONE");
        }


        private static void Subsampling(Image ycrcbMatrix)
        {
            Console.Write("Subsampling...");
            //ycrcbMatrix.SubsampleMatrix();
            //ycrcbMatrix.ExtendMatrix();
            ycrcbMatrix.subsamplingChanne2();
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
