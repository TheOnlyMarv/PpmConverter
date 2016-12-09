using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegConverter.Huffman
{
    public class HuffmanTree
    {
        private int value;
        private bool leaf;
        private HuffmanTree leftNode, rightNode;

        private HuffmanTree(int value, bool leaf, HuffmanTree left, HuffmanTree right)
        {
            this.value = value;
            this.leaf = leaf;
            leftNode = left;
            rightNode = right;
        }

        public static HuffmanTree createNormalTree(SortedList<float, int> symbols)
        {
            SortedList<float, HuffmanTree> structure = new SortedList<float, HuffmanTree>(new DuplicateKeyComparer<float>());
            foreach (KeyValuePair<float, int> cur in symbols)
            {
                structure.Add(cur.Key, new HuffmanTree(cur.Value, true, null, null));
            }
            return runHuffmanAlgortihm(structure);
        }

        private static HuffmanTree runHuffmanAlgortihm(SortedList<float, HuffmanTree> structure)
        {
            if (structure.Count == 1) return structure.Values[0];
            HuffmanTree newItem = new HuffmanTree(-1, false, structure.Values[0], structure.Values[1]);
            float newKey = structure.Keys[0] + structure.Keys[1];

            structure.RemoveAt(0);
            structure.RemoveAt(0);

            structure.Add(newKey, newItem);

            return runHuffmanAlgortihm(structure);
        }

        public static HuffmanTree createRightTree(HuffmanTree tree)
        {
            SortedList<int, float> newStructur = new SortedList<int, float>();
            int tier = 0;
            HuffmanTree ht = tree;
            if(ht.leftNode == null && ht.rightNode == null)
            {
                return growRightAlgorithm(newStructur);
            }

            while (!ht.leaf)
            {
                if (ht.leftNode != null)
                {
                    ht = ht.leftNode;
                }
                else if (ht.rightNode != null)
                {
                    ht = ht.rightNode;
                }
                else
                {
                    ht.leaf = true;
                    createRightTree(ht);
                }
                tier++;
            }
            newStructur.Add(tier, ht.value);
            //entfernen des Blattes aus dem Baum

        
            tier = 0;

            return createRightTree(tree);;
        }

        public static HuffmanTree growRightAlgorithm(SortedList<int, float> structur)
        {
            //Neuer Code


            return null;
        }

        public static HuffmanTree AvoidOneStar(HuffmanTree huffmanTree)
        {
            HuffmanTree ht = huffmanTree;

            while (!ht.leaf)
            {
                ht = ht.rightNode;
            }

            ht.leaf = false;

            ht.leftNode = new HuffmanTree(ht.value, true, null, null);
            ht.rightNode = new HuffmanTree(-1, true, null, null);

            return huffmanTree;
        }

        public void encode(string word, Bitstream bitstream)
        {
            bitstream.WriteByte((byte)word.Length);
            char[] toEncode = word.ToCharArray();
            LinkedList<int> toWrite = new LinkedList<int>();
            for (int i = 0; i < toEncode.Length; ++i)
            {
                string code = findCode((int)toEncode[i], "");
                foreach (char symbol in code)
                {
                    if (symbol.Equals('0')) toWrite.AddLast(0);
                    else toWrite.AddLast(1);
                }
            }
            bitstream.WriteBits(toWrite.ToArray());
        }

        public string decode(Bitstream bitstream)
        {
            int[] toDecode = getAllBits(bitstream);
            int counter = 8; //first byte is the lenght of the word
            LinkedList<char> returnValues = new LinkedList<char>();

            byte length = 0;
            for (int i = 0; i < 8; ++i)
            {
                length = (byte)((length << 1) + toDecode[i]);
            }
            int maxLenght = (int)length;

            while (returnValues.Count < maxLenght)
            {
                HuffmanTree curTree = this;
                while (!curTree.leaf)
                {
                    if (toDecode[counter++] == 0) curTree = curTree.leftNode;
                    else curTree = curTree.rightNode;
                }

                returnValues.AddLast((char)curTree.value);
            }

            return new string(returnValues.ToArray());
        }

        private int[] getAllBits(Bitstream bitstream)
        {
            int[] retVal = new int[bitstream.Length * 8];
            int counter = 0;
            for (int i = 0; i < bitstream.Length; ++i)
            {
                int[] curByte = bitstream.ReadBits();
                for (int j = 0; j < 8; ++j)
                {
                    retVal[counter++] = curByte[j];
                }
            }
            return retVal;
        }

        public String findCode(int symbol, String code)
        {
            if (this.leaf)
            {
                if (value == symbol) return code;
                else return null;
            }
            string left = this.leftNode.findCode(symbol, code + 0);
            string right = this.rightNode.findCode(symbol, code + 1);

            if (left != null) return left;
            else return right;
        }

        public void print()
        {
            this.printRek("");
        }

        private void printRek(string code)
        {
            if (this.leaf)
            {
                Console.WriteLine("{0}  {1}", this.value, code);
                return;
            }
            if (this.leftNode != null) this.leftNode.printRek(code + "0");
            if (this.rightNode != null) this.rightNode.printRek(code + "1");
        }

    }

    public class DuplicateKeyComparer<TKey>
                :
             IComparer<TKey> where TKey : IComparable
    {
        #region IComparer<TKey> Members

        public int Compare(TKey x, TKey y)
        {
            int result = x.CompareTo(y);

            if (result == 0)
                return 1;
            else
                return result;
        }

        #endregion
    }
}
