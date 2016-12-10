using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegConverter.Huffman
{
    public class Node
    {
        private int botLevel;
        private int topLevel;

        public byte Value { get; set; }
        public int Frequency { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }
        public bool Leaf { get { return Left == null && Right == null; } }
        public int LevelFromBootom { get { return botLevel; } }
        public int LevelFromTop { get { return topLevel; } set { topLevel = value; } }

        public Node(byte value, int frequency)
        {
            this.Value = value;
            this.Frequency = frequency;
            this.botLevel = 0;
            this.topLevel = 0;
        }
        public Node(Node left, Node right) : this(0, (left != null ? left.Frequency : 0) + (right != null ? right.Frequency : 0))
        {
            this.Left = left;
            this.Right = right;
            botLevel = Math.Max(left != null ? left.botLevel : 0, right != null ? right.botLevel : 0) + 1;
            topLevel = Math.Min(left != null ? left.topLevel : int.MaxValue, right != null ? right.topLevel : int.MaxValue) - 1;
        }
    }
}
