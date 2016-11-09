using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PpmConverter
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Create Bits in File...");
            Felix__Bitstrom.createBitFile();
            Console.WriteLine(" - Done");

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }

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
    }
}
