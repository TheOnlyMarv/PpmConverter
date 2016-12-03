using JpegConverter.Huffman;
using JpegConverter.Jpeg;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegConverter
{
    class Program
    {
        public static void Main(string[] args)
        {
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

            SortedList<float, int> symbols = new SortedList<float, int>(new JpegConverter.Huffman.DuplicateKeyComparer<float>());

            //symbols.Add(0.3f, 72);
            //symbols.Add(0.2f, 97);
            //symbols.Add(0.15f, 108);
            //symbols.Add(0.1f, 32);
            //symbols.Add(0.25f, 111);

            symbols.Add(4.0f, 1);
            symbols.Add(4.0f, 2);
            symbols.Add(6.0f, 3);
            symbols.Add(6.0f, 4);
            symbols.Add(7.0f, 5);
            symbols.Add(9.0f, 6);

            JpegConverter.Huffman.HuffmanTree tree = JpegConverter.Huffman.HuffmanTree.createNormalTree(symbols);
            tree.print();

            Bitstream bitstream = new Bitstream();
            tree.encode("Hallo", bitstream);
            bitstream.Seek(0, System.IO.SeekOrigin.Begin);
            String result = tree.decode(bitstream);

            Console.WriteLine(result);

            Console.Read();

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
            jpegEncoder.WriteMarker(image);
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
