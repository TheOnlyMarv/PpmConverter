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
    [TestClass]
    public class HuffmanTest
    {
        [TestMethod]
        public void TestNormalHuffman()
        {
            SortedList<float, int> sl = new SortedList<float, int>(new DuplicateKeyComparer<float>());
            sl.Add(4f, 1);
            sl.Add(4f, 2);
            sl.Add(6f, 3);
            sl.Add(6f, 4);
            sl.Add(7f, 5);
            sl.Add(9f, 6);
            HuffmanTree ht = HuffmanTree.createNormalTree(sl);

            Assert.AreEqual("010", ht.findCode(1, ""));
            Assert.AreEqual("011", ht.findCode(2, ""));
            Assert.AreEqual("110", ht.findCode(3, ""));
            Assert.AreEqual("111", ht.findCode(4, ""));
            Assert.AreEqual("00", ht.findCode(5, ""));
            Assert.AreEqual("10", ht.findCode(6, ""));
        }

        [TestMethod]
        public void TestRightHuffman()
        {
            SortedList<float, int> sl = new SortedList<float, int>(new DuplicateKeyComparer<float>());
            sl.Add(4f, 1);
            sl.Add(4f, 2);
            sl.Add(6f, 3);
            sl.Add(6f, 4);
            sl.Add(7f, 5);
            sl.Add(9f, 6);
            HuffmanTree orgHt = HuffmanTree.createNormalTree(sl);
            //HuffmanTree ht = HuffmanTree.createRightTree(orgHt);

            ////HuffmanTree ht = HuffmanTree.createRightTree(sl);

            //Assert.AreEqual("111", ht.findCode(1, ""));
            //Assert.AreEqual("110", ht.findCode(2, ""));
            //Assert.AreEqual("101", ht.findCode(3, ""));
            //Assert.AreEqual("100", ht.findCode(4, ""));
            //Assert.AreEqual("01", ht.findCode(5, ""));
            //Assert.AreEqual("00", ht.findCode(6, ""));
        }

        [TestMethod]
        public void TestAvoidOneStar()
        {
            SortedList<float, int> sl = new SortedList<float, int>(new DuplicateKeyComparer<float>());
            sl.Add(4f, 1);
            sl.Add(4f, 2);
            sl.Add(6f, 3);
            sl.Add(6f, 4);
            sl.Add(7f, 5);
            sl.Add(9f, 6);
            HuffmanTree orgHt = HuffmanTree.createNormalTree(sl);
            HuffmanTree newHt = HuffmanTree.AvoidOneStar(orgHt);

            Assert.AreEqual("010", newHt.findCode(1, ""));
            Assert.AreEqual("011", newHt.findCode(2, ""));
            Assert.AreEqual("110", newHt.findCode(3, ""));
            Assert.AreEqual("1110", newHt.findCode(4, ""));
            Assert.AreEqual("00", newHt.findCode(5, ""));
            Assert.AreEqual("10", newHt.findCode(6, ""));
        }

        [TestMethod]
        public void TestPackageMergeAlgorithm()
        {
            SortedList<float, int> sl = new SortedList<float, int>(new DuplicateKeyComparer<float>());
            sl.Add(1f, 1);
            sl.Add(2f, 2);
            sl.Add(5f, 3);
            sl.Add(10f, 4);
            sl.Add(21f, 5);

            HuffmanTree normalHt = HuffmanTree.createNormalTree(sl);
            Assert.AreEqual("0000", normalHt.findCode(1, ""));
            Assert.AreEqual("0001", normalHt.findCode(2, ""));
            Assert.AreEqual("001", normalHt.findCode(3, ""));
            Assert.AreEqual("01", normalHt.findCode(4, ""));
            Assert.AreEqual("1", normalHt.findCode(5, ""));

            //HuffmanTree newTree = HuffmanTree.CreateLengthLimitedHuffmanTree(sl, 16);
            //Assert.AreEqual("0000", normalHt.findCode(1, ""));
        }

        [TestMethod]
        public void TestNewHuffman()
        {
            Dictionary<byte, int> sl = new Dictionary<byte, int>();
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


            Dictionary<byte, int> sl2 = new Dictionary<byte, int>();
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
            Dictionary<byte, int> sl = new Dictionary<byte, int>();
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

            sl = new Dictionary<byte, int>();
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
            Dictionary<byte, int> sl = new Dictionary<byte, int>();
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


            Dictionary<byte, int> sl2 = new Dictionary<byte, int>();
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
    }
}
