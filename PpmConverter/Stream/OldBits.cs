using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PpmConverter
{
    public class OldBits
    {
        Stream stream;

        public object FastRandom { get; private set; }

        public OldBits()
        {
            long start = DateTime.Now.Ticks;
            stream = new MemoryStream();
            long end = DateTime.Now.Ticks;
            Console.WriteLine("Instanciate stream Time: {0}", new TimeSpan(DateTime.FromBinary(end - start).Ticks));
        }

        public void GenerateRandomBits(int numbBits)
        {
            Random random = new Random();
            long start = DateTime.Now.Ticks;
            byte b = 0;
            int bitcounter = 0;
            for (int i = 0; i < numbBits; i++)
            {
                
                b = (byte)((b << 1) + 1);
                bitcounter++;
                if (bitcounter == 8)
                {
                    stream.WriteByte(b);
                    bitcounter = 0;
                    b = 0;
                }
            }
            long end = DateTime.Now.Ticks;
            Console.WriteLine("Performace for {0} random Bits: {1}", numbBits, new TimeSpan(DateTime.FromBinary(end - start).Ticks));
        }

        public void TestRead()
        {
            long start = DateTime.Now.Ticks;
            stream.Seek(0, SeekOrigin.Begin);
            int i=0; 
            while (stream.ReadByte() != -1)
            {
                ++i;
            }
            long end = DateTime.Now.Ticks;
            Console.WriteLine("Read Bits: {0}", new TimeSpan(DateTime.FromBinary(end - start).Ticks));
            Console.WriteLine(i);
        }

        public void FlashIntoFile(string path)
        {
            long start = DateTime.Now.Ticks;
            FileStream fs = new FileStream(path, FileMode.Create);
            stream.Seek(0, SeekOrigin.Begin);
            stream.CopyTo(fs);
            fs.Flush();
            fs.Close();
            long end = DateTime.Now.Ticks;
            Console.WriteLine("Time for write the stream into file: {0}", new TimeSpan(DateTime.FromBinary(end - start).Ticks));
        }

        public void WriteJpegImageToStream(PPMImage ppmImage)
        {
            WriteStartOfImage();
            WriteApp0();
            WriteSof0();
            WriteEndOfImage();
        }

        public void WriteStartOfImage()
        {
            stream.WriteByte(0xff);
            stream.WriteByte(0xd8);
        }

        public void WriteEndOfImage()
        {
            stream.WriteByte(0xff);
            stream.WriteByte(0xd9);
        }

        public void WriteApp0()
        {
            //Marker
            stream.WriteByte(0xff);
            stream.WriteByte(0xe0);

            //Laenge des Segments
            stream.WriteByte(0x00);
            stream.WriteByte(0x10);

            //JFIF 0x00
            stream.WriteByte(0x4a);
            stream.WriteByte(0x46);
            stream.WriteByte(0x49);
            stream.WriteByte(0x46);
            stream.WriteByte(0x00);

            //Major revision number 1
            stream.WriteByte(0x01);

            //Minor revision number 0..2 hier: 1
            stream.WriteByte(0x01);

            //Einheit der Pixelgroesse 0=Keine Einheit, sondern Seitenverhältnis
            stream.WriteByte(0x00);

            //x-Dichte != 0 hier: 0x0048
            stream.WriteByte(0x00);
            stream.WriteByte(0x48);

            //y-Dichte != 0 hier: 0x0048
            stream.WriteByte(0x00);
            stream.WriteByte(0x48);

            //groesse x y vorschaubild 0 = kein Vorschaubild
            stream.WriteByte(0x00);
            stream.WriteByte(0x00);

            //n-bytes fuer vorschaubild (x*y*3) 0 = kein Vorschaubild
            stream.WriteByte(0x00);
        }

        public void WriteSof0()
        {
            //Marker
            stream.WriteByte(0xff);
            stream.WriteByte(0xc0);

            //Laenge: 8 + anz. Komponenten * 3
            //JFIF benutzt entweder 1 Komponente (Y, Grauwertbilder) oder 3 Komponenten(YCbCr, Farbbilder)
            byte b = 8 + 3 * 3;
            stream.WriteByte(0x00);
            stream.WriteByte(b);

            //Genauigkeit der Daten in bits/sample: normal=8 (12 u. 16 meist nicht unterstützt)
            stream.WriteByte(0x08);

            //Bildgroesse y > 0
            stream.WriteByte(0x00);
            stream.WriteByte(0x48);

            //Bildgroesse x > 0
            stream.WriteByte(0x00);
            stream.WriteByte(0x48);

            //Anzahl Komponenten
            stream.WriteByte(0x03);

            //Komponente Y
            //  ID=1
            stream.WriteByte(0x01);
            //  Faktor unterabtastung (Bit 0-3 vertikal, 4-7 Horizontal);  Keine Unterabtastung: 0x22, Unterabtastung Faktor 2: 0x11
            stream.WriteByte(0x11);
            //  Nummer der Quantisierungstabelle [KEIN PLAN]
            stream.WriteByte(0x01);

            //Komponente Cb
            //  ID=2
            stream.WriteByte(0x02);
            //  Faktor unterabtastung (Bit 0-3 vertikal, 4-7 Horizontal);  Keine Unterabtastung: 0x22, Unterabtastung Faktor 2: 0x11
            stream.WriteByte(0x11);
            //  Nummer der Quantisierungstabelle [KEIN PLAN]
            stream.WriteByte(0x01);

            //Komponente Cr
            //  ID=2
            stream.WriteByte(0x03);
            //  Faktor unterabtastung (Bit 0-3 vertikal, 4-7 Horizontal);  Keine Unterabtastung: 0x22, Unterabtastung Faktor 2: 0x11
            stream.WriteByte(0x11);
            //  Nummer der Quantisierungstabelle [KEIN PLAN]
            stream.WriteByte(0x01);
        }
    }
}
