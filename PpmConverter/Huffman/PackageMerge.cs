using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegConverter.Huffman
{
    class PackageMerge
    {
        private static List<List<PackageNode>> PackageLists;
        public static List<KeyValuePair<int, int>> Generate(List<KeyValuePair<byte, int>> symbols, int limit)
        {
            InitializePackageLists(symbols, limit);
            MergeLists();

            return CreateLevelCounting(limit);
        }

        private static List<KeyValuePair<int, int>> CreateLevelCounting(int limit)
        {
            Dictionary<int, int> result = new Dictionary<int, int>();
            Dictionary<byte, int> symbolCount = CountSymbol(PackageLists[limit]);
            for (int i = 1; i <= limit; i++)
            {
                int counter = symbolCount.Count(x => x.Value == i);
                if (counter != 0)
                {
                    result[i] = counter;
                }
            }
            return result.OrderByDescending(x => x.Key).ToList();
        }

        private static Dictionary<byte, int> CountSymbol(List<PackageNode> list)
        {
            Dictionary<byte, int> result = new Dictionary<byte, int>();
            foreach (PackageNode packageNode in list)
            {
                foreach (byte symbol in packageNode.Symbols)
                {
                    try
                    {
                        result[symbol] += 1;
                    }
                    catch (Exception)
                    {
                        result[symbol] = 1;
                    }
                }
            }
            return result;
        }

        private static void MergeLists()
        {
            int counter = 1;
            while (counter < PackageLists.Count)
            {
                List<PackageNode> packageList = PackageLists[counter - 1];
                List<PackageNode> higherPackageList = PackageLists[counter];

                int symbolCounter = 0;

                while (symbolCounter < packageList.Count - 1)
                {
                    PackageNode packageNode = PackageNode.Compine(packageList[symbolCounter], packageList[symbolCounter + 1]); // packageList[symbolCounter].Compine(packageList[symbolCounter + 1]).Copy();
                    higherPackageList.Add(packageNode);
                    symbolCounter += 2;
                }
                PackageLists[counter] = higherPackageList.OrderBy(x => x.Frequency).ToList();
                counter++;
            }

        }

        private static void InitializePackageLists(List<KeyValuePair<byte, int>> symbols, int limit)
        {
            PackageLists = new List<List<PackageNode>>();
            for (int i = 0; i < limit; i++)
            {
                PackageLists.Add(CreateNewOriginalList(symbols));
            }
            PackageLists.Add(new List<PackageNode>());
        }

        private static List<PackageNode> CreateNewOriginalList(List<KeyValuePair<byte, int>> symbols)
        {
            List<PackageNode> result = new List<PackageNode>();
            foreach (var symbol in symbols)
            {
                result.Add(new PackageNode(symbol.Key, symbol.Value));
            }
            return result.OrderBy(x => x.Frequency).ToList();
        }
    }
}

struct PackageNode
{
    public int Frequency;
    public List<byte> Symbols;
    public PackageNode(byte symbol, int frequency)
    {
        this.Symbols = new List<byte>();
        Symbols.Add(symbol);
        this.Frequency = frequency;
    }
    public PackageNode Compine(PackageNode other)
    {
        Frequency += other.Frequency;
        foreach (byte symbol in other.Symbols)
        {
            Symbols.Add(symbol);
        }
        return this;
    }
    public static PackageNode Compine(PackageNode left, PackageNode right)
    {
        PackageNode newPn = new PackageNode();
        newPn.Frequency = left.Frequency + right.Frequency;
        newPn.Symbols = new List<byte>();
        foreach (byte item in left.Symbols)
        {
            newPn.Symbols.Add(item);
        }
        foreach (byte item in right.Symbols)
        {
            newPn.Symbols.Add(item);
        }
        return newPn;
    }

    public PackageNode Copy()
    {
        PackageNode newPn = new PackageNode(Symbols.First(), Frequency);
        newPn.Symbols = new List<byte>();
        foreach (byte symbol in Symbols)
        {
            newPn.Symbols.Add(symbol);
        }
        return newPn;
    }
}

