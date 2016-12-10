using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegConverter.Huffman
{
    public class Huffman
    {
        private Dictionary<byte, int> Symboles { get; set; }
        private Node root { get; set; }
        public Dictionary<byte, string> CodeDictionary { get; set; }

        public Huffman(Dictionary<byte, int> symboles)
        {
            this.Symboles = symboles;
        }

        #region NormalHuffman
        public void CreateNormalHuffman(bool avoidOneStar = false)
        {
            CodeDictionary = null;
            List<KeyValuePair<int, Node>> nodes = CreateIntialNodes();
            RunNormalHuffman(nodes); //Sicher ist sicher
            if (avoidOneStar)
            {
                AvoidingOneStar(root);
            }
        }

        private List<KeyValuePair<int, Node>> CreateIntialNodes()
        {
            List<KeyValuePair<byte, int>> symboles = Symboles.OrderBy(x => x.Value).ToList();
            List<KeyValuePair<int, Node>> result = new List<KeyValuePair<int, Node>>();
            foreach (var symbole in symboles)
            {
                result.Add(new KeyValuePair<int, Node>(symbole.Value, new Node(symbole.Key, symbole.Value)));
            }
            return result.OrderBy(x => x.Key).ToList();
        }

        private void RunNormalHuffman(List<KeyValuePair<int, Node>> sorted)
        {
            if (sorted.Count == 1)
            {
                root = sorted[0].Value;
                return;
            }
            sorted.Add(new KeyValuePair<int, Node>(sorted[0].Key + sorted[1].Key, new Node(sorted[0].Value, sorted[1].Value)));
            sorted.RemoveAt(0);
            sorted.RemoveAt(0);
            RunNormalHuffman(sorted.OrderBy(x=>x.Value.Value).OrderBy(x => x.Key).ToList());
        }
        #endregion

        #region CodeFinding
        public string GetCode(byte symbole)
        {
            if (CodeDictionary == null)
            {
                CreateCodeDictionary(root);
            }
            return CodeDictionary[symbole];
        }

        private void CreateCodeDictionary(Node node)
        {
            CodeDictionary = new Dictionary<byte, string>();
            foreach (var symbole in Symboles)
            {
                CodeDictionary[symbole.Key] = FindCode(node, symbole.Key);
            }
        }
        private string FindCode(Node node, byte symbole, string code = "")
        {
            if (node == null)
            {
                return null;
            }
            if (node.Leaf)
            {
                if (node.Value == symbole)
                {
                    return code;
                }
                else
                {
                    return null;
                }
            }
            string left = FindCode(node.Left, symbole, code + "0");
            string right = FindCode(node.Right, symbole, code + "1");
            if (right != null)
            {
                return right;
            }
            else
            {
                return left;
            }
        }
        #endregion

        #region AvoidingOneStar
        private void AvoidingOneStar(Node node)
        {
            if (node.Right.Leaf)
            {
                node.Right = new Node(new Node(node.Right.Value, node.Right.Frequency), null);
                return;
            }
            AvoidingOneStar(node.Right);
        }
        #endregion

        #region RightGrowingHuffman
        public void CreateRightGrowingHuffman(bool avoidOneStar = false)
        {
            CreateNormalHuffman();

            Dictionary<int, int> LevelNodes = new Dictionary<int, int>();
            CountLevelEntries(LevelNodes, root);
            var initialNodes = CreateIntialNodes();

            root = CreateRekurcive(initialNodes, LevelNodes.OrderByDescending(x => x.Key).ToList());
            CodeDictionary = null;

            if (avoidOneStar)
            {
                AvoidingOneStar(root);
            }
        }

        private Node CreateRekurcive(List<KeyValuePair<int, Node>> initialNodes, List<KeyValuePair<int, int>> levelNodes, int level = 0)
        {
            if (levelNodes.Count != 0 && levelNodes.First().Key != level)
            {
                Node right = CreateRekurcive(initialNodes, levelNodes, level + 1);
                Node left = CreateRekurcive(initialNodes, levelNodes, level + 1);
                return new Node(left, right);
            }
            else if (levelNodes.First().Value != 0)
            {
                Node node = new Node(initialNodes.First().Value.Value, initialNodes.First().Value.Frequency);
                initialNodes.RemoveAt(0);
                int key = levelNodes.First().Key;
                int value = levelNodes.First().Value - 1;
                levelNodes.RemoveAt(0);
                if (value != 0)
                {
                    levelNodes.Insert(0, new KeyValuePair<int, int>(key, value));
                }
                return new Node(node.Value, node.Frequency);
            }
            else
            {
                levelNodes.RemoveAt(0);
                return CreateRekurcive(initialNodes, levelNodes, level);
            }
        }

        private void CountLevelEntries(Dictionary<int, int> dictionary, Node node, int level = 0)
        {
            if (node.Leaf)
            {
                try
                {
                    dictionary[level] += 1;
                }
                catch (Exception)
                {
                    dictionary[level] = 1;
                }
                return;
            }
            CountLevelEntries(dictionary, node.Right, level + 1);
            CountLevelEntries(dictionary, node.Left, level + 1);
        }
        #endregion
    }
}
