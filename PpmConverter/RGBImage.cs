using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PpmConverter
{
    public class RGBImage : Image
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

    }
}
