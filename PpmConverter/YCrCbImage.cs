using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PpmConverter
{
    class YCrCbImage : Image
    {
        private byte[,] _y;
        private byte[,] _cr;
        private byte[,] _cb;

        public YCrCbImage(byte[,] y, byte[,] cr, byte[,] cb)
        {
            _y = y;
            _cr = cr;
            _cb = cb;
        }

        public YCrCbImage(int x, int y)
        {
            _y = new byte[x, y];
            _cr = new byte[x, y];
            _cb = new byte[x, y];
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
    }
}