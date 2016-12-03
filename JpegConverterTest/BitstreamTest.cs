using System;
using JpegConverter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace JpegConverterTest
{
    [TestClass]
    public class BitstreamTest
    {
        [TestMethod]
        public void TestWriteReadSingleBits()
        {
            Bitstream bitstream = new Bitstream();
            int[] bits = new int[] { 1, 1, 1, 0, 0, 1, 1, 1 }; //231  -> 8 Bits
            bitstream.WriteBits(bits);
            bitstream.WriteBit(1);
            bitstream.WriteBit(1);
            bitstream.WriteBit(0);
            bitstream.WriteBit(1);
            bitstream.WriteBit(0);
            bitstream.WriteBit(0);
            bitstream.WriteBit(1);
            bitstream.WriteBit(0);
            //8 Bits -> 16 Bits

            bitstream.WriteBit(1);
            bitstream.WriteBit(1);
            bitstream.WriteBit(0);
            //3 Bits -> 19 Bits
            bitstream.Flush();
            bitstream.Seek(0, SeekOrigin.Begin);

            Assert.AreEqual(231, bitstream.ReadByte());
            Assert.AreEqual(210, bitstream.ReadByte());
            Assert.AreEqual(223, bitstream.ReadByte());
        }

        [TestMethod]
        public void TestReadWriteBytes()
        {
            Bitstream bitstream = new Bitstream();
            bitstream.WriteByte(0xf1);
            bitstream.WriteByte(0xf5);
            bitstream.Seek(0, SeekOrigin.Begin);

            Assert.AreEqual(0xf1, bitstream.ReadByte());
            Assert.AreEqual(0xf5, bitstream.ReadByte());
            Assert.AreEqual(-1, bitstream.ReadByte());
        }
    }
}
