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
            HuffmanTree ht = HuffmanTree.createRightTree(orgHt);

            //HuffmanTree ht = HuffmanTree.createRightTree(sl);

            Assert.AreEqual("111", ht.findCode(1, ""));
            Assert.AreEqual("110", ht.findCode(2, ""));
            Assert.AreEqual("101", ht.findCode(3, ""));
            Assert.AreEqual("100", ht.findCode(4, ""));
            Assert.AreEqual("01", ht.findCode(5, ""));
            Assert.AreEqual("00", ht.findCode(6, ""));
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
    }
}
