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
        private int _StepX = 2;
        private int _stepY = 2;

        public YCbCrImage(byte[,] y, byte[,] cb, byte[,] cr)
        {
            _y = y;
            _cb = cb;
            _cr = cr;
        }

        public bool subsamplingCb()
        {
            int _countX = 0;
            int _countY = 0;
            byte[,] _newCb = new byte[_cb.GetLength(0) / 2, _cb.GetLength(1) / 2];
            for(int i = 0; i < _cb.GetLength(0); i++) 
            {
                for(int j = 0; j < _cb.GetLength(1); j++)
                {
                    if(j % 2 != 0 && i%2 != 0)
                    {
                        int avg = (int)_cb[i - 1, j - 1] + (int)_cb[i, j - 1] + (int)_cb[i - 1, j] + (int)_cb[i, j];
                        avg = avg / 4;
                        _newCb[_countX, _countY++] = (byte)avg;
                    }
                }
                _countY = 0;
                if (i % 2 != 0) _countX++;
            }
            _cb = _newCb;
            return true;
        }

        public bool subsamplingCr()
        {
            int _countX = 0;
            int _countY = 0;
            byte[,] _newCr = new byte[_cr.GetLength(0) / 2, _cr.GetLength(1) / 2];
            for (int i = 0; i < _cr.GetLength(0); i++)
            {
                for (int j = 0; j < _cr.GetLength(1); j++)
                {
                    if (j % 2 != 0 && i % 2 != 0)
                    {
                        int avg = (int)_cr[i - 1, j - 1] + (int)_cr[i, j - 1] + (int)_cr[i - 1, j] + (int)_cr[i, j];
                        avg = avg / 4;
                        _newCr[_countX, _countY++] = (byte)avg;
                    }
                }
                _countY = 0;
                if(i % 2 != 0) _countX++;
            }
            _cr = _newCr;
            return true;
        }

        public bool extendMatrix()
        {
            extendMatrixCb();
            extendMatrixCr();
            return true;
        }

        private bool extendMatrixCb() 
        {
            int indexX = -1;
            int indexY = -1;
            int offsetX = _y.GetLength(0) / _cb.GetLength(0);
            int offsetY = _y.GetLength(1) / _cb.GetLength(1);
            byte[,] _extendedMatrixCb = new byte[_y.GetLength(0), _y.GetLength(1)];
            for(int i = 0; i < _y.GetLength(0); i++)
            {
                if (i % offsetY == 0) indexY++;
                for(int j = 0; j < _y.GetLength(1); j++)
                {
                    if (j % offsetX == 0) indexX++;
                    _extendedMatrixCb[j, i] = _cb[indexX, indexY];
                }
            }
            _cb = _extendedMatrixCb;
            return true;
        }

        private bool extendMatrixCr()
        {
            int indexX = -1;
            int indexY = -1;
            int offsetX = _y.GetLength(0) / _cr.GetLength(0);
            int offsetY = _y.GetLength(1) / _cr.GetLength(1);
            byte[,] _extendedMatrixCr = new byte[_y.GetLength(0), _y.GetLength(1)];
            for (int i = 0; i < _y.GetLength(0); i++)
            {
                if (i % offsetY == 0) indexY++;
                for (int j = 0; j < _y.GetLength(1); j++)
                {
                    if (j % offsetX == 0) indexX++;
                    _extendedMatrixCr[j, i] = _cr[indexX, indexY];
                }
            }
            _cr = _extendedMatrixCr;
            return true;
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