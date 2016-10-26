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
                YCrCbImage ycrcbMatrix;
                try
                {
                    Console.Write("Load file... ");
                    image = PPMImage<RGBImage>.LoadImageFromFile(path);
                    ycrcbMatrix = RGBToYCbCr(image.Matrix);
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

        private static YCrCbImage RGBToYCbCr(RGBImage matrix)
        {
            byte[,] red = matrix.R;
            byte[,] green = matrix.G;
            byte[,] blue = matrix.B;

            int maxWidth = matrix.R.GetLength(0);
            int maxHight = matrix.R.GetLength(1);

            byte[,] y = new byte[maxWidth, maxHight];
            byte[,] cb = new byte[maxWidth, maxHight];
            byte[,] cr = new byte[maxWidth, maxHight];

            int width;
            int hight;

            for(hight = 0; hight < maxHight; hight++)
            {
                for (width = 0; width < maxWidth; width++)
                {
                    y[width, hight] = (byte)(((0.299 * red[width, hight]) + (0.587 * green[width, hight]) + (0.114 * blue[width, hight])));
                    cb[width, hight] = (byte)(((-0.1687 * red[width, hight]) + (-0.3312 * green[width, hight]) + (0.5 * blue[width, hight])) + 128);
                    cr[width, hight] = (byte)(((0.5 * red[width, hight]) + (-0.4186 * green[width, hight]) + (-0.0813 + blue[width, hight])) + 128);
                }
            }

            return new YCrCbImage(y, cb, cr);
        }
    }
}
