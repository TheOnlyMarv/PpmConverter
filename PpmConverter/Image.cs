using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegConverter
{
    public class Image
    {
        private byte[,] channel0; //R //Y
        private byte[,] channel1; //G //Cb
        private byte[,] channel2; //B //Cr

        public Image(byte[,] channel0, byte[,] channel1, byte[,] channel2)
        {
            this.channel0 = channel0;
            this.channel1 = channel1;
            this.channel2 = channel2;
        }

        public bool subsamplingChanne2()
        {
            int _countX = 0;
            int _countY = 0;
            byte[,] _newCb = new byte[channel2.GetLength(0) / 2, channel2.GetLength(1) / 2];
            for(int i = 0; i < channel2.GetLength(0); i++) 
            {
                for(int j = 0; j < channel2.GetLength(1); j++)
                {
                    if(j % 2 != 0 && i%2 != 0)
                    {
                        int avg = (int)channel2[i - 1, j - 1] + (int)channel2[i, j - 1] + (int)channel2[i - 1, j] + (int)channel2[i, j];
                        avg = (avg + 2) / 4;
                        _newCb[_countX, _countY++] = (byte)avg;
                    }
                }
                _countY = 0;
                if (i % 2 != 0) _countX++;
            }
            channel2 = _newCb;
            return true;
        }

        public bool subsamplingChannel1()
        {
            int _countX = 0;
            int _countY = 0;
            byte[,] _newCr = new byte[channel1.GetLength(0) / 2, channel1.GetLength(1) / 2];
            for (int i = 0; i < channel1.GetLength(0); i++)
            {
                for (int j = 0; j < channel1.GetLength(1); j++)
                {
                    if (j % 2 != 0 && i % 2 != 0)
                    {
                        int avg = (int)channel1[i - 1, j - 1] + (int)channel1[i, j - 1] + (int)channel1[i - 1, j] + (int)channel1[i, j];
                        avg = (avg + 2) / 4;
                        _newCr[_countX, _countY++] = (byte)avg;
                    }
                }
                _countY = 0;
                if(i % 2 != 0) _countX++;
            }
            channel1 = _newCr;
            return true;
        }

        public bool extendMatrix()
        {
            extendMatrixChannel2();
            extendMatrixChannel1();
            return true;
        }

        private bool extendMatrixChannel2() 
        {
            int indexX = -1;
            int indexY = -1;
            int offsetX = channel0.GetLength(0) / channel2.GetLength(0);
            int offsetY = channel0.GetLength(1) / channel2.GetLength(1);
            byte[,] _extendedMatrixCb = new byte[channel0.GetLength(0), channel0.GetLength(1)];
            for(int i = 0; i < channel0.GetLength(0); i++)
            {
                if (i % offsetY == 0) ++indexY;
                for(int j = 0; j < channel0.GetLength(1); j++)
                {
                    if (j % offsetX == 0) ++indexX;
                    _extendedMatrixCb[i, j] = channel2[indexY, indexX];
                }
                indexX = -1;
            }
            channel2 = _extendedMatrixCb;
            return true;
        }

        private bool extendMatrixChannel1()
        {
            int indexX = -1;
            int indexY = -1;
            int offsetX = channel0.GetLength(0) / channel1.GetLength(0);
            int offsetY = channel0.GetLength(1) / channel1.GetLength(1);
            byte[,] _extendedMatrixCr = new byte[channel0.GetLength(0), channel0.GetLength(1)];
            for (int i = 0; i < channel0.GetLength(0); i++)
            {
                if (i % offsetY == 0) ++indexY;
                for (int j = 0; j < channel0.GetLength(1); j++)
                {
                    if (j % offsetX == 0) ++indexX;
                    _extendedMatrixCr[i, j] = channel1[indexY, indexX];
                }
                indexX = -1;
            }
            channel1 = _extendedMatrixCr;
            return true;
        }
       
        public Image(int x, int y)
        {
            channel0 = new byte[x, y];
            channel1 = new byte[x, y];
            channel2 = new byte[x, y];
        }

        public byte[,] Channel0
        {
            get { return channel0; }
            set { channel0 = value; }
        }

        public byte[,] Channel1
        {
            get { return channel1; }
            set { channel1 = value; }
        }

        public byte[,] Channel2
        {
            get { return channel2; }
            set { channel2 = value; }
        }

        public static Image FromRGBtoYCbCr(Image image)
        {
            byte[,] red = image.Channel0;
            byte[,] green = image.Channel1;
            byte[,] blue = image.Channel2;

            int maxWidth = image.Channel0.GetLength(0);
            int maxHeight = image.Channel0.GetLength(1);

            byte[,] y = new byte[maxWidth, maxHeight];
            byte[,] cb = new byte[maxWidth, maxHeight];
            byte[,] cr = new byte[maxWidth, maxHeight];


            for (int height = 0; height < maxHeight; height++)
            {
                for (int width = 0; width < maxWidth; width++)
                {
                    y[width, height] = (byte)Math.Round(0.257 * red[width, height] + 0.504 * green[width, height] + 0.098 * blue[width, height] + 16);
                    cb[width, height] = (byte)Math.Round(-0.148 * red[width, height] - 0.291 * green[width, height] + 0.439 * blue[width, height] + 128);//(blue[width,height] - y[width, height]) * 0.564 + 128);
                    cr[width, height] = (byte)Math.Round(0.439 * red[width, height] - 0.368 * green[width, height] - 0.071 * blue[width, height] + 128);
                }
            }
            //Debug.WriteLine("r: {0}\tg: {1}\tb: {2}", rgbImage.R[30, 0], rgbImage.G[30, 0], rgbImage.B[30, 0]);
            //Debug.WriteLine("y: {0}\tcb: {1}\tcr: {2}", y[30,0] , cb[30, 0], cr[30, 0]);
            return new Image(y, cb, cr);
        }
    }
}