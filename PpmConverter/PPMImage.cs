﻿using System;
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
                throw new WrongExtensionException("Wrong file extension!");
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
