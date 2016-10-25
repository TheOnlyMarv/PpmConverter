using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PpmConverter
{
    public class PPMImage<T> where T : Image
    {
        #region Declaration
        private PPMImage()
        {
        }

        private string _typ;

        public string Typ
        {
            get { return _typ; }
        }

        private T _matrix;

        public T Matrix
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

        #endregion

        #region Methods
        public static PPMImage<RGBImage> LoadImageFromFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path + " not found");
            }
            if (!path.EndsWith(".ppm", StringComparison.CurrentCultureIgnoreCase))
            {
                throw new IOException("Wrong file extension!");
            }

            PPMImage<RGBImage> image = new PPMImage<RGBImage>();
            using (StreamReader reader = new StreamReader(path))
            {
                ReadingState state = ReadingState.LfTyp;
                int orgX = 0, orgY = 0;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();
                    if (!line.StartsWith("#"))
                    {
                        switch (state)
                        {
                            case ReadingState.LfTyp:
                                image._typ = line;
                                state = ReadingState.LfResolution;
                                break;
                            case ReadingState.LfResolution:
                                string[] xy = line.Split(new char[] { ' ', '\t' });
                                int x, y;
                                if (xy.Length == 2 && int.TryParse(xy[0], out x) && int.TryParse(xy[1], out y))
                                {
                                    orgX = x;
                                    int temp = x % image._stepX;
                                    int newX = x - (temp == 0 ? 0 : temp - image._stepX);

                                    orgY = y;
                                    temp = y % image._stepY;
                                    int newY = y - (temp == 0 ? 0 : temp - image._stepY);

                                    image._matrix = new RGBImage(newX, newY);

                                    state = ReadingState.LfMaxValue;
                                }
                                else
                                {
                                    // IllegalFormatException
                                }
                                break;
                            case ReadingState.LfMaxValue:
                                if (!byte.TryParse(line, out image._maxValue))
                                {
                                    // IllegalFormatException
                                }
                                state = ReadingState.LfImage;
                                break;

                            case ReadingState.LfImage:
                                ReadImage(line, reader, image, orgX, orgY);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            return image;
        }

        private static void ReadImage(string start, StreamReader reader, PPMImage<RGBImage> image, int orgX, int orgY)
        {
            string allLines = start + " " + reader.ReadToEnd();
            string[] value = allLines.Replace("\n", "").Split(new char[] { ' ', '\t', '\n' });
            try
            {
                int currX = 0, currY = 0;
                for (int i = 0; i < value.Length; i = i + 3)
                {
                    byte r, g, b;
                    if (byte.TryParse(value[i], out r) && byte.TryParse(value[i + 1], out g) && byte.TryParse(value[i + 2], out b))
                    {
                        image.Matrix.R[currX, currY] = r;
                        image.Matrix.G[currX, currY] = g;
                        image.Matrix.B[currX, currY] = b;
                        if (++currX == orgX)
                        {
                            currX = 0;
                            currY++;
                        }
                    }
                    else
                    {
                        // IllegalFormatException
                    }
                }

            }
            catch (IndexOutOfRangeException)
            {
                // IllegalFormatException
            }
        }

        private enum ReadingState
        {
            LfTyp,
            LfResolution,
            LfMaxValue,
            LfImage
        }
        #endregion
    }
}
