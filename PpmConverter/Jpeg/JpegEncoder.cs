using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JpegConverter.Encoding;

namespace JpegConverter.Jpeg
{
    public class JpegEncoder
    {
        public static int[] LuminanceAC = { 0, 0, 0, 0 };
        public static int[] LuminanceDC = { 0, 0, 0, 0 };
        public static int[] ChrominaceAC = { 0, 0, 0, 1 };
        public static int[] ChrominaceDC = { 0, 0, 0, 1 };

        private Bitstream bitstream;

        private const byte SEGMENT_BEGIN = 0xff;

        public JpegEncoder()
        {
            bitstream = new Bitstream();
        }

        public void SaveIntoFile(string path)
        {
            bitstream.FlushIntoFile(path);
        }

        public void WriteMarker(PPMImage image, List<JpegConverter.Huffman.Huffman> huffmanTables, Encoding.ImageEncoder encoder)
        {
            WriteStartOfImage();
            WriteApp0();
            WriteDqt();
            WriteSof0(image);
            WriteDht(huffmanTables);
            WriteSos();
            WirteImageData(encoder, image);
            WirteEndOfImage();
        }

        private void WirteImageData(ImageEncoder encoder, PPMImage image)
        {
            List<int[]> yBlocks = new List<int[]>();
            foreach (var item in encoder.forChannelY)
            {
                int[] dcBits = item.Key.HuffmanCode.Concat(item.Key.BitPattern).ToArray();
                int[] acBits = new int[0];
                foreach (RunLengthAcPair rlap in item.Value)
                {
                    acBits = acBits.Concat(rlap.HuffmanCode.Concat(rlap.BitPattern ?? new int[]{ } ).ToArray()).ToArray();
                }
                int[] block = dcBits.Concat(acBits).ToArray();
                yBlocks.Add(block);
            }

            List<int[]> CbBlocks = new List<int[]>();
            foreach (var item in encoder.forChannelCb)
            {
                int[] dcBits = item.Key.HuffmanCode.Concat(item.Key.BitPattern).ToArray();
                int[] acBits = new int[0];
                foreach (RunLengthAcPair rlap in item.Value)
                {
                    acBits = acBits.Concat(rlap.HuffmanCode.Concat(rlap.BitPattern ?? new int[] { }).ToArray()).ToArray();
                }
                int[] block = dcBits.Concat(acBits).ToArray();
                CbBlocks.Add(block);
            }

            List<int[]> CrBlocks = new List<int[]>();
            foreach (var item in encoder.forChannelCr)
            {
                int[] dcBits = item.Key.HuffmanCode.Concat(item.Key.BitPattern).ToArray();
                int[] acBits = new int[0];
                foreach (RunLengthAcPair rlap in item.Value)
                {
                    acBits = acBits.Concat(rlap.HuffmanCode.Concat(rlap.BitPattern ?? new int[] { }).ToArray()).ToArray();
                }
                int[] block = dcBits.Concat(acBits).ToArray();
                CrBlocks.Add(block);
            }

            int Bb = image.Matrix.Channel0.GetLength(1) / 8;
            int offset = Bb - 1;
            int indexA = 0;
            for (int i = 0; i < CbBlocks.Count; i++)
            {
                if (i%(Bb/2)==0 && i!=0)
                {
                    indexA += Bb;
                }
                bitstream.WriteBits(yBlocks[indexA++]);
                bitstream.WriteBits(yBlocks[indexA]);
                bitstream.WriteBits(yBlocks[(indexA++) + offset]);
                bitstream.WriteBits(yBlocks[indexA + offset]);
                //bitstream.WriteBits(yBlocks[i]);
                bitstream.WriteBits(CbBlocks[i]);
                bitstream.WriteBits(CrBlocks[i]);
            }

        }

        private void WriteSos()
        {
            //Marker
            bitstream.WriteByte(SEGMENT_BEGIN);
            bitstream.WriteByte(0xda);

            //Länge
            int length = 6 + 2 * 3;
            bitstream.WriteByte((byte)(length >> 8));
            bitstream.WriteByte((byte)length);

            //Anz. Komponenten
            bitstream.WriteByte(0x03);

            //Y
            //  ID
            bitstream.WriteByte(0x01);
            //  HT ID 0-3=AC    4-7=DC
            bitstream.WriteBits(new int[] {
                LuminanceDC[0],
                LuminanceDC[1],
                LuminanceDC[2],
                LuminanceDC[3],
                LuminanceAC[0],
                LuminanceAC[1],
                LuminanceAC[2],
                LuminanceAC[3],
            });

            //Cb
            //  ID
            bitstream.WriteByte(0x02);
            //  HT ID 0-3=AC    4-7=DC
            bitstream.WriteBits(new int[] {
                ChrominaceDC[0],
                ChrominaceDC[1],
                ChrominaceDC[2],
                ChrominaceDC[3],
                ChrominaceAC[0],
                ChrominaceAC[1],
                ChrominaceAC[2],
                ChrominaceAC[3],
            });

            //Cr
            //  ID
            bitstream.WriteByte(0x03);
            //  HT ID 0-3=AC    4-7=DC
            bitstream.WriteBits(new int[] {
                ChrominaceDC[0],
                ChrominaceDC[1],
                ChrominaceDC[2],
                ChrominaceDC[3],
                ChrominaceAC[0],
                ChrominaceAC[1],
                ChrominaceAC[2],
                ChrominaceAC[3],
            });

            //3 Bytes ohne Relevanz
            bitstream.WriteByte(0x00);
            bitstream.WriteByte(0x3f);
            bitstream.WriteByte(0x00);

        }

        private void WriteDqt()
        {
            //Marker
            bitstream.WriteByte(SEGMENT_BEGIN);
            bitstream.WriteByte(0xdb);

            //Länge 
            int length = 132;
            bitstream.WriteByte((byte)(length >> 8));
            bitstream.WriteByte((byte)length);

            //Luminance QT
            // bit 0..3: Nummer der QT (0..3, sonst Fehler); bit 4..7: Genauigkeit der QT, 0 = 8 bit, sonst 16 bit
            int[] bits = { 0, 0, 0, 0, 0, 0, 0, 0 };
            bitstream.WriteBits(bits);

            // Schreiben der Koeffizienten in ZickZack-Sortierung
            foreach (int koeffizient in Quantisation.Quantisation.QuantisationTableForLuminanceZickZack)
            {
                bitstream.WriteByte((byte)koeffizient);
            }


            //Chrominance QT
            // bit 0..3: Nummer der QT (0..3, sonst Fehler); bit 4..7: Genauigkeit der QT, 0 = 8 bit, sonst 16 bit
            int[] bitss = { 0, 0, 0, 0, 0, 0, 0, 1 };
            bitstream.WriteBits(bitss);

            // Schreiben der Koeffizienten in ZickZack-Sortierung
            foreach (int koeffizient in Quantisation.Quantisation.QuantisationTableForChrominanceZickZack)
            {
                bitstream.WriteByte((byte)koeffizient);
            }
        }

        private void WirteEndOfImage()
        {
            bitstream.WriteByte(SEGMENT_BEGIN);
            bitstream.WriteByte(0xd9);
        }

        private void WriteStartOfImage()
        {
            bitstream.WriteByte(SEGMENT_BEGIN);
            bitstream.WriteByte(0xd8);
        }

        #region Segments
        private void WriteApp0()
        {
            //Marker
            bitstream.WriteByte(SEGMENT_BEGIN);
            bitstream.WriteByte(0xe0);

            //Laenge des Segments
            bitstream.WriteByte(0x00);
            bitstream.WriteByte(0x10);

            //JFIF 0x00
            bitstream.WriteByte(0x4a);
            bitstream.WriteByte(0x46);
            bitstream.WriteByte(0x49);
            bitstream.WriteByte(0x46);
            bitstream.WriteByte(0x00);

            //Major revision number 1
            bitstream.WriteByte(0x01);

            //Minor revision number 0..2 hier: 1
            bitstream.WriteByte(0x01);

            //Einheit der Pixelgroesse 0=Keine Einheit, sondern Seitenverhältnis
            bitstream.WriteByte(0x00);

            //x-Dichte != 0 hier: 0x0048
            bitstream.WriteByte(0x00);
            bitstream.WriteByte(0x48);

            //y-Dichte != 0 hier: 0x0048
            bitstream.WriteByte(0x00);
            bitstream.WriteByte(0x48);

            //groesse x y vorschaubild 0 = kein Vorschaubild
            bitstream.WriteByte(0x00);
            bitstream.WriteByte(0x00);

            //n-bytes fuer vorschaubild (x*y*3); Für keine Vorschau, kein byte
            
        }

        private void WriteSof0(PPMImage image)
        {
            //Marker
            bitstream.WriteByte(SEGMENT_BEGIN);
            bitstream.WriteByte(0xc0);

            //Laenge: 8 + anz. Komponenten * 3
            //JFIF benutzt entweder 1 Komponente (Y, Grauwertbilder) oder 3 Komponenten(YCbCr, Farbbilder)
            byte b = 8 + 3 * 3;
            bitstream.WriteByte(0x00);
            bitstream.WriteByte(b);

            //Genauigkeit der Daten in bits/sample: normal=8 (12 u. 16 meist nicht unterstützt)
            bitstream.WriteByte(0x08);

            //Bildgroesse y > 0
            
            bitstream.WriteByte((byte)(image.OrgY >> 8));
            bitstream.WriteByte((byte)image.OrgY);

            //Bildgroesse x > 0
            bitstream.WriteByte((byte)(image.OrgX >> 8));
            bitstream.WriteByte((byte)image.OrgX);

            //Anzahl Komponenten
            bitstream.WriteByte(0x03);

            //Komponente Y
            //  ID=1
            bitstream.WriteByte(0x01);
            //  Faktor unterabtastung (Bit 0-3 vertikal, 4-7 Horizontal);  Keine Unterabtastung: 0x22, Unterabtastung Faktor 2: 0x11
            bitstream.WriteByte(0x11);
            //  Nummer der Quantisierungstabelle [KEIN PLAN]
            bitstream.WriteByte(0x00);

            //Komponente Cb
            //  ID=2
            bitstream.WriteByte(0x02);
            //  Faktor unterabtastung (Bit 0-3 vertikal, 4-7 Horizontal);  Keine Unterabtastung: 0x22, Unterabtastung Faktor 2: 0x11
            bitstream.WriteByte(0x11);
            //  Nummer der Quantisierungstabelle [KEIN PLAN]
            bitstream.WriteByte(0x01);

            //Komponente Cr
            //  ID=2
            bitstream.WriteByte(0x03);
            //  Faktor unterabtastung (Bit 0-3 vertikal, 4-7 Horizontal);  Keine Unterabtastung: 0x22, Unterabtastung Faktor 2: 0x11
            bitstream.WriteByte(0x11);
            //  Nummer der Quantisierungstabelle [KEIN PLAN]
            bitstream.WriteByte(0x01);
        }
        
        private void WriteDht(List<JpegConverter.Huffman.Huffman> huffmanTables)
        {
            foreach (Huffman.Huffman huffman in huffmanTables)
            {
            //Marker
            bitstream.WriteByte(SEGMENT_BEGIN);
            bitstream.WriteByte(0xc4);

            //Laenge des Segments
            bitstream.WriteByte(0x00);
            int length = (19 + huffman.NumberOfSymbols()); //huffmanTables.Sum(x => x.NumberOfSymbols()) + 17 * huffmanTables.Count;//
                bitstream.WriteByte((byte)length);

                int[] id = null;
                switch (huffman.Type)
                {
                    case Huffman.HuffmanTyp.LuminanceAC:
                        id = LuminanceAC;
                        break;
                    case Huffman.HuffmanTyp.LuminanceDC:
                        id = LuminanceDC;
                        break;
                    case Huffman.HuffmanTyp.ChrominanceAC:
                        id = ChrominaceAC;
                        break;
                    case Huffman.HuffmanTyp.ChrominanceDC:
                        id = ChrominaceDC;
                        break;
                    default:
                        break;
                }

                // HT Informationen
                int[] htInfo = {
                    0,
                    0,
                    0,
                    huffman.Type == Huffman.HuffmanTyp.ChrominanceDC || huffman.Type == Huffman.HuffmanTyp.LuminanceDC ? 0 : 1,
                    id[0],
                    id[1],
                    id[2],
                    id[3]
                };
                bitstream.WriteBits(htInfo);

                // Anzahl von Symbolen mit Kodelängen von 1..16 (Summe dieser Anzahlen ist Gesamtzahl der Symbole, muss <= 256)
                foreach (int count in huffman.GetCountForEachLevel().Select(x=>x.Value))
                {
                    bitstream.WriteByte((byte)count);
                }

                // Tabelle mit den Symbolen in aufsteigender Folge der Kodelängen (n = total Gesamtzahl der Symbole)
                foreach (int symbol in huffman.GetSymbolsAscendingOnCodeLength())
                {
                    bitstream.WriteByte((byte)symbol);
                }

            }
        }
        #endregion
    }
}
