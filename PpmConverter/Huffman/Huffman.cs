using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegConverter.Huffman
{
    using Symbol = Int32;
    public class Huffman
    {
        private Dictionary<Symbol, int> Symbols { get; set; }
        private Node root { get; set; }
        public Dictionary<Symbol, string> CodeDictionary { get; set; }

        public Huffman(Dictionary<Symbol, int> symbols)
        {
            this.Symbols = symbols;
        }

        #region NormalHuffman
        public void CreateNormalHuffman(bool avoidOneStar = false)
        {
            CodeDictionary = null;
            List<KeyValuePair<int, Node>> nodes = CreateIntialNodes();
            RunNormalHuffman(nodes);
            if (avoidOneStar)
            {
                AvoidingOneStar(root);
            }
        }

        private List<KeyValuePair<int, Node>> CreateIntialNodes()
        {
            List<KeyValuePair<Symbol, int>> symbols = Symbols.OrderBy(x => x.Value).ToList();
            List<KeyValuePair<int, Node>> result = new List<KeyValuePair<int, Node>>();
            foreach (var symbol in symbols)
            {
                result.Add(new KeyValuePair<int, Node>(symbol.Value, new Node(symbol.Key, symbol.Value)));
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
        public string GetCode(Symbol symbol)
        {
            if (CodeDictionary == null)
            {
                CreateCodeDictionary(root);
            }
            return CodeDictionary[symbol];
        }

        private void CreateCodeDictionary(Node node)
        {
            CodeDictionary = new Dictionary<Symbol, string>();
            foreach (var symbol in Symbols)
            {
                CodeDictionary[symbol.Key] = FindCode(node, symbol.Key);
            }
        }
        private string FindCode(Node node, Symbol symbol, string code = "")
        {
            if (node == null)
            {
                return null;
            }
            if (node.Leaf)
            {
                if (node.Value == symbol)
                {
                    return code;
                }
                else
                {
                    return null;
                }
            }
            string left = FindCode(node.Left, symbol, code + "0");
            string right = FindCode(node.Right, symbol, code + "1");
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

            root = CreateRecursive(initialNodes, LevelNodes.OrderByDescending(x => x.Key).ToList());
            CodeDictionary = null;

            if (avoidOneStar)
            {
                AvoidingOneStar(root);
            }
        }

        private Node CreateRecursive(List<KeyValuePair<int, Node>> initialNodes, List<KeyValuePair<int, int>> levelNodes, int level = 0)
        {
            if (levelNodes.Count != 0 && levelNodes.First().Key != level)
            {
                Node right = CreateRecursive(initialNodes, levelNodes, level + 1);
                Node left = CreateRecursive(initialNodes, levelNodes, level + 1);
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
                return CreateRecursive(initialNodes, levelNodes, level);
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

        #region PackageMergedHuffman
        public void CreateLimitedHuffman(Symbol limit = 16, bool avoidOneStar = false)
        {
            List<KeyValuePair<int, int>> levelList = PackageMerge.Generate(Symbols.OrderBy(x => x.Value).ToList(), avoidOneStar ? limit - 1 : limit);
            root = CreateRecursive(CreateIntialNodes(), levelList);
            CodeDictionary = null;

            if (avoidOneStar)
            {
                AvoidingOneStar(root);
            }
        }
        #endregion
    }
}