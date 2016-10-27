using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PpmConverter
{
    public class YCbCrImage : Image
    {
        private byte[,] _y;
        private byte[,] _cr;
        private byte[,] _cb;

        public YCbCrImage(byte[,] y, byte[,] cb, byte[,] cr)
        {
            _y = y;
            _cb = cb;
            _cr = cr;
        }

        public YCbCrImage(int x, int y)
        {
            _y = new byte[x, y];
            _cb = new byte[x, y];
            _cr = new byte[x, y];
        }

        public byte[,] Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public byte[,] Cr
        {
            get { return _cr; }
            set { _cr = value; }
        }

        public byte[,] Cb
        {
            get { return _cb; }
            set { _cb = value; }
        }

        public static YCbCrImage FromRGB(RGBImage rgbImage)
        {
            byte[,] red = rgbImage.R;
            byte[,] green = rgbImage.G;
            byte[,] blue = rgbImage.B;

            int maxWidth = rgbImage.R.GetLength(0);
            int maxHeight = rgbImage.R.GetLength(1);

            byte[,] y = new byte[maxWidth, maxHeight];
            byte[,] cb = new byte[maxWidth, maxHeight];
            byte[,] cr = new byte[maxWidth, maxHeight];


            for (int height = 0; height < maxHeight; height++)
            {
                for (int width = 0; width < maxWidth; width++)
                {
                    y[width, height] = (byte)(((0.299 * red[width, height]) + (0.587 * green[width, height]) + (0.114 * blue[width, height])));
                    cr[width, height] = (byte)(((0.5 * red[width, height]) + (-0.4186 * green[width, height]) + (-0.0813 + blue[width, height])) + 128);
                    cb[width, height] = (byte)(((-0.1687 * red[width, height]) + (-0.3312 * green[width, height]) + (0.5 * blue[width, height])) + 128);
                }
            }

            return new YCbCrImage(y, cb, cr);
        }
    }
}