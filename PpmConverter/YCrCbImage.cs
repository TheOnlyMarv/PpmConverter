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

        public byte[,] Y
        {
            get { return _y; }
        }
        private byte[,] _Cr;

        public byte[,] Cr
        {
            get { return _Cr; }
        }
        private byte[,] _cb;
            
        public byte[,] _Cb
        {
            get { return _cb; }
        }

    }
}
