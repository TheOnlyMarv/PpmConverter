using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PpmConverter
{
    public class RGBImage : Image
    {
        public RGBImage(byte[,] r, byte[,] g, byte[,] b)
        {
            _r = r;
            _g = g;
            _b = b;
        }

        private byte[,] _r;

        public byte[,] R
        {
            get { return _r; }
        }

        private byte[,] _g;

        public byte[,] G
        {
            get { return _g; }
        }
        private byte[,] _b;

        public byte[,] B
        {
            get { return _b; }
        }

    }
}
