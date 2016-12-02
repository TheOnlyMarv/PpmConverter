using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegConverter
{
    public class YCbCrImage
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

        #region Old
        public void SubsampleMatrix()
        {
            byte[,] newCr = new byte[_cr.GetLength(0) / PPMImage.StepX, _cr.GetLength(1) / PPMImage.StepY];
            byte[,] newCb = new byte[_cb.GetLength(0) / PPMImage.StepX, _cb.GetLength(1) / PPMImage.StepY];

            int smallY = 0;
            for (int y = 0; y < _cr.GetLength(1); y = y + PPMImage.StepY)
            {
                int smallX = 0;
                for (int x = 0; x < _cr.GetLength(0); x = x + PPMImage.StepX)
                {
                    int sumCr = 0;
                    int sumCb = 0;
                    for (int sY = 0; sY < PPMImage.StepY; sY++)
                    {
                        for (int sX = 0; sX < PPMImage.StepX; sX++)
                        {
                            sumCr += _cr[x + sY, y + sY];
                            sumCb += _cb[x + sY, y + sY];
                        }
                    }
                    newCr[smallX, smallY] = (byte)(sumCr / (PPMImage.StepX * PPMImage.StepY));
                    newCb[smallX, smallY] = (byte)(sumCb / (PPMImage.StepX * PPMImage.StepY));
                    smallX++;
                }
                smallY++;
            }
            _cr = newCr;
            _cb = newCb;
        }

        public void ExtendMatrix()
        {
            byte[,] newCr = new byte[_cr.GetLength(0) * PPMImage.StepX, _cr.GetLength(1) * PPMImage.StepY];
            byte[,] newCb = new byte[_cb.GetLength(0) * PPMImage.StepX, _cb.GetLength(1) * PPMImage.StepY];


            for (int sY = 0; sY < PPMImage.StepY; sY++)
            {
                for (int sX = 0; sX < PPMImage.StepX; sX++)
                {
                    for (int y = sY; y < newCr.GetLength(1); y = y + sY + 1)
                    {
                        for (int x = sX; x < newCr.GetLength(0); x = x + sX + 1)
                        {
                            newCr[x, y] = _cr[x / PPMImage.StepX, y / PPMImage.StepY];
                            newCb[x, y] = _cb[x / PPMImage.StepX, y / PPMImage.StepY];
                        }
                    }
                }
            }
            _cr = newCr;
            _cb = newCb;
        }
        #endregion

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
                        avg = (avg + 2) / 4;
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
                        avg = (avg + 2) / 4;
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
                if (i % offsetY == 0) ++indexY;
                for(int j = 0; j < _y.GetLength(1); j++)
                {
                    if (j % offsetX == 0) ++indexX;
                    _extendedMatrixCb[i, j] = _cb[indexY, indexX];
                }
                indexX = -1;
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
                if (i % offsetY == 0) ++indexY;
                for (int j = 0; j < _y.GetLength(1); j++)
                {
                    if (j % offsetX == 0) ++indexX;
                    _extendedMatrixCr[i, j] = _cr[indexY, indexX];
                }
                indexX = -1;
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
                    y[width, height] = (byte)Math.Round(0.257 * red[width, height] + 0.504 * green[width, height] + 0.098 * blue[width, height] + 16);
                    cb[width, height] = (byte)Math.Round(-0.148 * red[width, height] - 0.291 * green[width, height] + 0.439 * blue[width, height] + 128);//(blue[width,height] - y[width, height]) * 0.564 + 128);
                    cr[width, height] = (byte)Math.Round(0.439 * red[width, height] - 0.368 * green[width, height] - 0.071 * blue[width, height] + 128);
                }
            }
            //Debug.WriteLine("r: {0}\tg: {1}\tb: {2}", rgbImage.R[30, 0], rgbImage.G[30, 0], rgbImage.B[30, 0]);
            //Debug.WriteLine("y: {0}\tcb: {1}\tcr: {2}", y[30,0] , cb[30, 0], cr[30, 0]);
            return new YCbCrImage(y, cb, cr);
        }
    }
}