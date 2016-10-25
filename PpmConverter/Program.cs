using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PpmConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                string path = args[0];
                PPMImage<RGBImage> image;
                try
                {
                    Console.Write("Load file... ");
                    image = PPMImage<RGBImage>.LoadImageFromFile(path);
                    Console.WriteLine(" - DONE");
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine(ex.Message);
                }

                Console.ReadLine();
            }
        }
    }
}
