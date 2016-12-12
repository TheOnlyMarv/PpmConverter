using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using JpegConverter.Huffman;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace JpegConverterTest
{
    using JpegConverter;
    using Symbol = Int32;
    [TestClass]
    public class HuffmanTest
    {
        [TestMethod]
        public void TestNewHuffman()
        {
            Dictionary<Symbol, int> sl = new Dictionary<Symbol, int>();
            sl.Add(1, 4);
            sl.Add(2, 4);
            sl.Add(3, 6);
            sl.Add(4, 6);
            sl.Add(5, 7);
            sl.Add(6, 9);
            Huffman huffman = new Huffman(sl);
            huffman.CreateNormalHuffman();

            Assert.AreEqual("010", huffman.GetCode(1));
            Assert.AreEqual("011", huffman.GetCode(2));
            Assert.AreEqual("110", huffman.GetCode(3));
            Assert.AreEqual("111", huffman.GetCode(4));
            Assert.AreEqual("00", huffman.GetCode(5));
            Assert.AreEqual("10", huffman.GetCode(6));


            Dictionary<Symbol, int> sl2 = new Dictionary<Symbol, int>();
            sl2.Add(1, 4);
            sl2.Add(2, 4);
            sl2.Add(3, 7);
            sl2.Add(4, 8);
            sl2.Add(5, 10);

            Huffman huffman2 = new Huffman(sl2);
            huffman2.CreateNormalHuffman();

            Assert.AreEqual("010", huffman2.GetCode(1));
            Assert.AreEqual("011", huffman2.GetCode(2));
            Assert.AreEqual("00", huffman2.GetCode(3));
            Assert.AreEqual("10", huffman2.GetCode(4));
            Assert.AreEqual("11", huffman2.GetCode(5));

        }

        [TestMethod]
        public void TestNewHuffmanAvoidingOneStar()
        {
            Dictionary<Symbol, int> sl = new Dictionary<Symbol, int>();
            sl.Add(1, 4);
            sl.Add(2, 4);
            sl.Add(3, 6);
            sl.Add(4, 6);
            sl.Add(5, 7);
            sl.Add(6, 9);
            Huffman huffman = new Huffman(sl);
            huffman.CreateNormalHuffman(true);

            Assert.AreEqual("010", huffman.GetCode(1));
            Assert.AreEqual("011", huffman.GetCode(2));
            Assert.AreEqual("110", huffman.GetCode(3));
            Assert.AreEqual("1110", huffman.GetCode(4));
            Assert.AreEqual("00", huffman.GetCode(5));
            Assert.AreEqual("10", huffman.GetCode(6));

            sl = new Dictionary<Symbol, int>();
            sl.Add(1, 4);
            sl.Add(2, 4);
            sl.Add(3, 7);
            sl.Add(4, 8);
            sl.Add(5, 10);

            huffman = new Huffman(sl);
            huffman.CreateNormalHuffman(true);

            Assert.AreEqual("010", huffman.GetCode(1));
            Assert.AreEqual("011", huffman.GetCode(2));
            Assert.AreEqual("00", huffman.GetCode(3));
            Assert.AreEqual("10", huffman.GetCode(4));
            Assert.AreEqual("110", huffman.GetCode(5));
        }

        [TestMethod]
        public void TestNewRightGrowingHuffman()
        {
            Dictionary<Symbol, int> sl = new Dictionary<Symbol, int>();
            sl.Add(1, 4);
            sl.Add(2, 4);
            sl.Add(3, 6);
            sl.Add(4, 6);
            sl.Add(5, 7);
            sl.Add(6, 9);
            Huffman huffman = new Huffman(sl);
            huffman.CreateRightGrowingHuffman();

            Assert.AreEqual("111", huffman.GetCode(1));
            Assert.AreEqual("110", huffman.GetCode(2));
            Assert.AreEqual("101", huffman.GetCode(3));
            Assert.AreEqual("100", huffman.GetCode(4));
            Assert.AreEqual("01", huffman.GetCode(5));
            Assert.AreEqual("00", huffman.GetCode(6));


            Dictionary<Symbol, int> sl2 = new Dictionary<Symbol, int>();
            sl2.Add(1, 4);
            sl2.Add(2, 4);
            sl2.Add(3, 7);
            sl2.Add(4, 8);
            sl2.Add(5, 10);

            Huffman huffman2 = new Huffman(sl2);
            huffman2.CreateRightGrowingHuffman();

            Assert.AreEqual("111", huffman2.GetCode(1));
            Assert.AreEqual("110", huffman2.GetCode(2));
            Assert.AreEqual("10", huffman2.GetCode(3));
            Assert.AreEqual("01", huffman2.GetCode(4));
            Assert.AreEqual("00", huffman2.GetCode(5));
        }

        [TestMethod]
        public void TestNewLimitedHuffman()
        {
            Dictionary<Symbol, int> sl = new Dictionary<Symbol, int>();
            sl.Add(1, 4);
            sl.Add(2, 4);
            sl.Add(3, 6);
            sl.Add(4, 6);
            sl.Add(5, 7);
            sl.Add(6, 9);
            Huffman huffman = new Huffman(sl);
            huffman.CreateLimitedHuffman(3);

            Assert.AreEqual("111", huffman.GetCode(1));
            Assert.AreEqual("110", huffman.GetCode(2));
            Assert.AreEqual("101", huffman.GetCode(3));
            Assert.AreEqual("100", huffman.GetCode(4));
            Assert.AreEqual("01", huffman.GetCode(5));
            Assert.AreEqual("00", huffman.GetCode(6));


            sl = new Dictionary<Symbol, int>();
            sl.Add(1, 1);
            sl.Add(2, 2);
            sl.Add(3, 4);
            sl.Add(4, 8);
            sl.Add(5, 16);
            sl.Add(6, 32);
            huffman = new Huffman(sl);
            huffman.CreateLimitedHuffman(3);

            Assert.AreEqual("111", huffman.GetCode(1));
            Assert.AreEqual("110", huffman.GetCode(2));
            Assert.AreEqual("101", huffman.GetCode(3));
            Assert.AreEqual("100", huffman.GetCode(4));
            Assert.AreEqual("01", huffman.GetCode(5));
            Assert.AreEqual("00", huffman.GetCode(6));
        }

        [TestMethod]
        public void TestEncodeAndDecode()
        {
            Dictionary<Symbol, int> sl = new Dictionary<Symbol, int>();
            sl.Add(1, 4);
            sl.Add(2, 4);
            sl.Add(3, 6);
            sl.Add(4, 6);
            sl.Add(5, 7);
            sl.Add(6, 9);
            Huffman huffman = new Huffman(sl);
            huffman.CreateLimitedHuffman(3);

            Assert.AreEqual("111", huffman.GetCode(1));
            Assert.AreEqual("110", huffman.GetCode(2));
            Assert.AreEqual("101", huffman.GetCode(3));
            Assert.AreEqual("100", huffman.GetCode(4));
            Assert.AreEqual("01", huffman.GetCode(5));
            Assert.AreEqual("00", huffman.GetCode(6));

            Bitstream bitstream = new Bitstream();
            Symbol[] toEncode = { 3, 5, 6, 2, 1, 4 };
            huffman.Encode(toEncode, bitstream);
            bitstream.Seek(0, System.IO.SeekOrigin.Begin);
            Symbol[] decoded = huffman.Decode(bitstream).ToArray();

            Assert.AreEqual(toEncode.Length, decoded.Length);

            for (int i = 0; i < toEncode.Length; i++)
            {
                Assert.AreEqual(toEncode[i], decoded[i]);
            }
        }
    }
}
