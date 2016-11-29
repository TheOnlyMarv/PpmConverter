using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PpmConverter
{
    public class Bitstream : Stream
    {
        private MemoryStream stream;
        private byte bufferByte;
        private byte bufferCounter;

        #region Properties
        public override bool CanRead
        {
            get
            {
                return stream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return stream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return stream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return stream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return stream.Position;
            }

            set
            {
                stream.Position = value;
            }
        }
        #endregion

        public Bitstream()
        {
            stream = new MemoryStream();
            bufferByte = 0;
            bufferCounter = 0;
        }
        public override void WriteByte(byte b)
        {
            stream.WriteByte(b);
        }

        public void WriteBit(int bit)
        {
            if (bit != 0 && bit != 1)
            {
                throw new IllegalFormatException("0 or 1 expected!");
            }
            bufferByte += (byte)(bit << (7 - bufferCounter++));
            if (bufferCounter == 8)
            {
                stream.WriteByte(bufferByte);
                bufferCounter = 0;
                bufferByte = 0;
            }
        }

        public void WriteBits(int[] bits)
        {
            for (int i = 0; i < bits.Length; i++)
            {
                WriteBit(bits[i]);
            }
            //byte counter = 0;
            //byte b = 0;
            //for (int i = 0; i < bits.Length; i++)
            //{
            //    b = (byte)((b << 1) + bits[i]);
            //    if (++counter == 8)
            //    {
            //        WriteByte(b);
            //        counter = 0;
            //        b = 0;
            //    }
            //}
            //if (counter != 0)
            //{
            //    byte temp = 0;
            //    for (int i = 0; i < 8 - counter; i++)
            //    {
            //        temp = (byte)((temp << 1) + 1);
            //    }
            //    WriteByte((byte)((b << 8 - counter) + temp));
            //}
        }

        public override int ReadByte()
        {
            return stream.ReadByte();
        }
        public int[] ReadBits()
        {
            int readed = ReadByte();
            if (readed == -1)
            {
                return null;
            }
            byte b = (byte)readed;
            int[] result = new int[8];
            for (int i = result.Length - 1; i >= 0; i--)
            {
                result[i] = (b & 0x01);
                b = (byte)(b >> 1);
            }
            return result;
        }

        public void FlushIntoFile(string file)
        {
            Flush();
            FileStream fs = new FileStream(file, FileMode.Create);
            long pos = Position;
            Position = 0;
            stream.CopyTo(fs);
            fs.Flush();
            fs.Close();
            Position = pos;
        }

        public override void Flush()
        {
            WriteIncompleteByte();
            stream.Flush();
        }

        private void WriteIncompleteByte()
        {
            while (bufferCounter != 8 && bufferCounter != 0)
            {
                bufferByte += (byte)(1 << (7 - bufferCounter++));
            }
            if (bufferCounter == 8)
            {
                WriteByte(bufferByte);
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            stream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return stream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            stream.Write(buffer, offset, count);
        }
    }
}
