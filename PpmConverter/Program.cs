using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PpmConverter
{
    class Program
    {
        public static void Main(string[] args)
        {
            string sourcePath, destinationPath;
            PPMImage<RGBImage> image;
            YCbCrImage ycrcbMatrix;
            RGBImage rgbMatrix;

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
                    rgbMatrix = ConvertToRGB(ycrcbMatrix);
                    SaveIntoFile(destinationPath);
                    break;
                default:
                    Console.WriteLine("Wrong number of arguments");
                    break;
            }
        }

        private static void SaveIntoFile(string destinationPath)
        {
            Console.Write("Converting back to RGB...");
            // Felix hier in file speichern
            Console.WriteLine(" - DONE");
        }

        private static RGBImage ConvertToRGB(YCbCrImage ycrcbMatrix)
        {
            Console.Write("Converting back to RGB...");
            RGBImage rgbMatrix = RGBImage.FromYCbCr(ycrcbMatrix);
            Console.WriteLine(" - DONE");
            return rgbMatrix;
        }

        private static YCbCrImage ConvertToYCbCr(PPMImage<RGBImage> image)
        {
            Console.Write("Converting to YCbCr...");
            YCbCrImage ycrcbMatrix = YCbCrImage.FromRGB(image.Matrix);
            Console.WriteLine(" - DONE");
            return ycrcbMatrix;
        }

        public static PPMImage<RGBImage> LoadImageFromFile(string path)
        {
            Console.Write("Load file...");
            PPMImage<RGBImage> image = PPMImage<RGBImage>.LoadImageFromFile(path);
            Console.WriteLine(" - DONE");
            return image;
        }
    }
}
