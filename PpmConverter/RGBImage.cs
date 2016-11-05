using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PpmConverter
{
    public class RGBImage
    {
        private byte[,] _r;
        private byte[,] _g;
        private byte[,] _b;
        public RGBImage(byte[,] r, byte[,] g, byte[,] b)
        {
            _r = r;
            _g = g;
            _b = b;
        }

        public RGBImage(int x, int y)
        {
            _r = new byte[x, y];
            _g = new byte[x, y];
            _b = new byte[x, y];
        }

        public byte[,] R
        {
            get { return _r; }
            set { _r = value; }
        }

        public byte[,] G
        {
            get { return _g; }
            set { _g = value; }
        }

        public byte[,] B
        {
            get { return _b; }
            set { _b = value; }
        }


        public static RGBImage FromYCbCr(YCbCrImage ycbcrimage)
        {
            ycbcrimage.extendMatrix();

            byte[,] y = ycbcrimage.Y;
            byte[,] cb = ycbcrimage.Cb;
            byte[,] cr = ycbcrimage.Cr;

            int maxWidth = ycbcrimage.Y.GetLength(0);
            int maxHeight = ycbcrimage.Y.GetLength(1);

            byte[,] r = new byte[maxWidth, maxHeight];
            byte[,] g = new byte[maxWidth, maxHeight];
            byte[,] b = new byte[maxWidth, maxHeight];

            for (int height = 0; height < maxHeight; height++)
            {
                for (int width = 0; width < maxWidth; width++)
                {
                    r[width, height] = (byte)(y[width, height] + 1.402 * (cr[width, height] - 128));
                    g[width, height] = (byte)(y[width, height] - 0.344136 *(cb[width, height] - 128) - 0.714136 * (cr[width, height] - 128));
                    b[width, height] = (byte)(y[width, height] + 1.772 * (cb[width, height] - 128) );

                    r[width, height] = Math.Max((byte)0, Math.Min((byte)255, r[width, height]));
                    g[width, height] = Math.Max((byte)0, Math.Min((byte)255, g[width, height]));
                    b[width, height] = Math.Max((byte)0, Math.Min((byte)255, b[width, height]));
                }
            }

            return new RGBImage(r, g, b);
        }
    }
}
