using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PpmConverter
{
    public class PPMImage
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

        private RGBImage _matrix;

        public RGBImage Matrix
        {
            get { return _matrix; }
            set { _matrix = value; }
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
        public void SaveIntoFile(string path)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                           
                writer.WriteLine(Typ);
                writer.WriteLine(string.Format("{0} {1}", Matrix.B.GetLength(0).ToString(), Matrix.B.GetLength(1).ToString()));
                writer.WriteLine(MaxValue.ToString());

                for (int y = 0; y < Matrix.R.GetLength(1); y++)
                {
                    for (int x = 0; x < Matrix.R.GetLength(0); x++)
                    {
                        writer.Write(string.Format("{0} {1} {2}", Matrix.R.GetValue(x, y), Matrix.G.GetValue(x, y), Matrix.B.GetValue(x, y)));
                    }
                    writer.WriteLine("");
                }
            }
        }

        public static PPMImage LoadImageFromFile(string path)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path + " not found");
            }
            if (!path.EndsWith(".ppm", StringComparison.CurrentCultureIgnoreCase))
            {
                throw new WrongExtensionException("Wrong file extension!");
            }

            PPMImage image = new PPMImage();
            using (StreamReader reader = new StreamReader(path))
            {
                ReadingState state = ReadingState.LfTyp;
                int orgX = 0, orgY = 0;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine().Trim();
                    if (!line.StartsWith("#"))
                    {
                        line = RemoveInlineComments(line);
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
                                    throw new IllegalFormatException("Wrong image format");
                                }
                                break;
                            case ReadingState.LfMaxValue:
                                if (!byte.TryParse(line, out image._maxValue))
                                {
                                    throw new IllegalFormatException("Wrong image format");
                                }
                                state = ReadingState.LfImage;
                                break;

                            case ReadingState.LfImage:
                                ReadImage(line, reader, image, orgX, orgY);
                                FillEmptyPixel(image, orgX, orgY);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            return image;
        }

        private static string RemoveInlineComments(string line)
        {
            int index = line.IndexOf('#');
            if (index == -1)
            {
                return line;
            }
            return line.Substring(0, index).Trim();
        }

        private static void ReadImage(string start, StreamReader reader, PPMImage image, int orgX, int orgY)
        {
            List<string> listString = new List<string>();
            listString.AddRange(start.Split(new char[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries));

            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (!line.StartsWith("#"))
                {
                    listString.AddRange(line.Split(new char[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries));
                }
            }

            string[] value = listString.ToArray();
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
                        throw new IllegalFormatException("Wrong image format");
                    }
                }

            }
            catch (IndexOutOfRangeException)
            {
                throw new IllegalFormatException("Wrong image format");
            }
        }
        private static void FillEmptyPixel(PPMImage image, int orgX, int orgY)
        {
            for (int x = 0; x < orgX; x++)
            {
                for (int y = orgY; y < image.Matrix.B.GetLength(1); y++)
                {
                    image.Matrix.R[x, y] = image.Matrix.R[x, y - 1];
                    image.Matrix.G[x, y] = image.Matrix.G[x, y - 1];
                    image.Matrix.B[x, y] = image.Matrix.B[x, y - 1];
                }
            }

            for (int y = 0; y < image.Matrix.B.GetLength(1); y++)
            {
                for (int x = orgX; x < image.Matrix.B.GetLength(0); x++)
                {
                    image.Matrix.R[x, y] = image.Matrix.R[x - 1, y];
                    image.Matrix.G[x, y] = image.Matrix.G[x - 1, y];
                    image.Matrix.B[x, y] = image.Matrix.B[x - 1, y];
                }
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
