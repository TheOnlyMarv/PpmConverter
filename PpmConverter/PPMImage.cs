using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PpmConverter
{
    public class PPMImage <T> where T : Image
    {
        private PPMImage()
        {
        }

        private string _typ;

        public string Typ
        {
            get { return _typ; }
        }

        private T[,] _matrix;

        public T[,] Matrix
        {
            get { return _matrix; }
        }
        private byte _maxValue;

        public byte MaxValue
        {
            get { return _maxValue; }
        }
        private int _stepX = 2;

        public int StepX
        {
            get { return _stepX; }
        }

        private int _stepY = 2;

        public int StepY
        {
            get { return _stepY; }
        }

        public static PPMImage<T> LoadImageFromFile(string path)
        {
            return new PPMImage<T>();
        }
    }
}
