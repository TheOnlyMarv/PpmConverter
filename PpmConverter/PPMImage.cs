using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegConverter
{
    public class PPMImage
    {
        #region Declaration
        private string _typ;
        private Image _matrix;
        private byte _maxValue;
        private static int _stepX = 16;
        private static int _stepY = 16;

        private PPMImage()
        {
        }

        public string Typ
        {
            get { return _typ; }
        }


        public Image Matrix
        {
            get { return _matrix; }
            set { _matrix = value; }
        }

        public byte MaxValue
        {
            get { return _maxValue; }
        }

        public static int StepX
        {
            get { return _stepX; }
        }

        public static int StepY
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
                writer.WriteLine(string.Format("{0} {1}", Matrix.Channel2.GetLength(0).ToString(), Matrix.Channel2.GetLength(1).ToString()));
                writer.WriteLine(MaxValue.ToString());

                for (int y = 0; y < Matrix.Channel0.GetLength(1); y++)
                {
                    for (int x = 0; x < Matrix.Channel0.GetLength(0); x++)
                    {
                        writer.Write(string.Format("{0} {1} {2} ", Matrix.Channel0.GetValue(x, y), Matrix.Channel1.GetValue(x, y), Matrix.Channel2.GetValue(x, y)));
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
                                if (line.Length == 2)
                                {
                                    image._typ = line;
                                    state = ReadingState.LfResolution;
                                    break;
                                }
                                image._typ = line.Substring(0, 2);
                                line = line.Substring(3);
                                state = ReadingState.LfResolution;
                                goto case ReadingState.LfResolution;
                            case ReadingState.LfResolution:
                                string[] xy = line.Split(new char[] { ' ', '\t', '\n' });
                                int x, y;
                                if (xy.Length >= 2)
                                {
                                    if (int.TryParse(xy[0], out x) && int.TryParse(xy[1], out y))
                                    {
                                        orgX = x;
                                        int temp = x % PPMImage._stepX;
                                        int newX = x - (temp == 0 ? 0 : temp - PPMImage._stepX);

                                        orgY = y;
                                        temp = y % PPMImage._stepY;
                                        int newY = y - (temp == 0 ? 0 : temp - PPMImage._stepY);

                                        image._matrix = new Image(newX, newY);

                                        state = ReadingState.LfMaxValue;

                                        if (xy.Length > 2)
                                        {
                                            line = line.Substring(xy[0].Length + xy[1].Length + 2);
                                            state = ReadingState.LfMaxValue;
                                            goto case ReadingState.LfMaxValue;
                                        }
                                    }
                                    else
                                    {
                                        throw new IllegalFormatException("Wrong image format");
                                    }
                                }
                                else
                                {
                                    throw new IllegalFormatException("Maybe X and Y in seperate lines");
                                }
                                break;
                            case ReadingState.LfMaxValue:
                                if (!byte.TryParse(line, out image._maxValue))
                                {
                                    if (byte.TryParse(line.Substring(0, line.IndexOf(' ')), out image._maxValue))
                                    {
                                        line = line.Substring(line.IndexOf(' ') + 1);
                                        state = ReadingState.LfImage;
                                        goto case ReadingState.LfImage;
                                    }
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
                        image.Matrix.Channel0[currY, currX] = r;
                        image.Matrix.Channel1[currY, currX] = g;
                        image.Matrix.Channel2[currY, currX] = b;
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
            for (int y = 0; y < orgY; y++)
            {
                for (int x = orgX; x < image.Matrix.Channel2.GetLength(1); x++)
                {
                    image.Matrix.Channel0[y, x] = image.Matrix.Channel0[y, x - 1];
                    image.Matrix.Channel1[y, x] = image.Matrix.Channel1[y, x - 1];
                    image.Matrix.Channel2[y, x] = image.Matrix.Channel2[y, x - 1];
                }
            }

            for (int x = 0; x < image.Matrix.Channel2.GetLength(1); x++)
            {
                for (int y = orgY; y < image.Matrix.Channel2.GetLength(0); y++)
                {
                    image.Matrix.Channel0[y, x] = image.Matrix.Channel0[y - 1, x];
                    image.Matrix.Channel1[y, x] = image.Matrix.Channel1[y - 1, x];
                    image.Matrix.Channel2[y, x] = image.Matrix.Channel2[y - 1, x];
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
