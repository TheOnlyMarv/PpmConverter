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

            byte[,] y = new byte[matrix.R.GetLength(0), matrix.R.GetLength(1)];
            byte[,] cb = new byte[matrix.R.GetLength(0), matrix.R.GetLength(1)];
            byte[,] cr = new byte[matrix.R.GetLength(0), matrix.R.GetLength(1)];

            byte width;
            byte hight;

            for(hight = 0; hight < matrix.R.GetLength(1); hight++)
            {
                for (width = 0; width < matrix.R.GetLength(0); width++)
                {
                    y[hight, width] = (byte)(((0.299 * red[width, hight]) + (0.587 * green[width, hight]) + (0.114 * blue[width, hight])));
                    cb[hight, width] = (byte)(((-0.1687 * red[width, hight]) + (-0.3312 * green[width, hight]) + (0.5 * blue[width, hight])) + 128);
                    cr[hight, width] = (byte)(((0.5 * red[width, hight]) + (-0.4186 * green[width, hight]) + (-0.0813 + blue[width, hight])) + 128);
                }
            }

            return new YCrCbImage(y, cb, cr);
        }
    }
}
