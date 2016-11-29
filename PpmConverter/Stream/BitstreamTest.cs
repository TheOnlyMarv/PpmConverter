using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PpmConverter
{
    public static class BitstreamTest
    {
        #region Benchmarking
        private static Bitstream bitstream;
        public static void StartBenchmarking(int numb)
        {
            Console.WriteLine("Benchmarking...");
            int[] bits = BenchGenerateRandomBits(numb);
            BenchInstantiateStream();
            BenchWriteStream(bits);
            BenchReadStream();
            BenchFlushIntoFile();
            Console.WriteLine("Benchmarking... DONE");
        }

        private static void BenchFlushIntoFile()
        {
            long start = DateTime.Now.Ticks;
            bitstream.FlushIntoFile("benchmark.stream");
            long end = DateTime.Now.Ticks;
            Console.WriteLine("Time for flush into file: {0}", new TimeSpan(DateTime.FromBinary(end - start).Ticks));
        }

        private static void BenchReadStream()
        {
            bitstream.Seek(0, SeekOrigin.Begin);
            long start = DateTime.Now.Ticks;
            int counter = 0;
            while (bitstream.ReadByte() != -1)
            {
                counter++;
            }
            long end = DateTime.Now.Ticks;
            Console.WriteLine("Time for read stream ({1} Bytes): {0}", new TimeSpan(DateTime.FromBinary(end - start).Ticks), counter);
        }

        private static void BenchWriteStream(int[] bits)
        {
            long start = DateTime.Now.Ticks;
            bitstream.WriteBits(bits);
            long end = DateTime.Now.Ticks;
            Console.WriteLine("Time for write stream: {0}", new TimeSpan(DateTime.FromBinary(end - start).Ticks));
        }

        private static void BenchInstantiateStream()
        {
            long start = DateTime.Now.Ticks;
            bitstream = new Bitstream();
            long end = DateTime.Now.Ticks;
            Console.WriteLine("Time for instantiate stream: {0}", new TimeSpan(DateTime.FromBinary(end - start).Ticks));
        }

        private static int[] BenchGenerateRandomBits(int numb)
        {
            long start = DateTime.Now.Ticks;
            Random random = new Random();
            int[] bits = new int[numb];
            for (int i = 0; i < bits.Length; i++)
            {
                bits[i] = random.Next(0, 2);
            }
            long end = DateTime.Now.Ticks;
            Console.WriteLine("Time for generate random BitArray[{1}]: {0}", new TimeSpan(DateTime.FromBinary(end - start).Ticks), numb);
            return bits;
        }
        #endregion

        #region Test
        private static string testFailed = "Test Failed: ";
        public static void StartTests()
        {
            testWriteReadByte();
            testWriteReadBits();
            testWriteReadSingleBits();
            Console.WriteLine("Tests Done");
        }

        private static void testWriteReadSingleBits()
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
            // Flush should add 5 Bits -> 24

            int exp1 = 231, exp2 = 210, exp3 = 223;
            bitstream.Seek(0, SeekOrigin.Begin);
            if (!(bitstream.ReadByte() == exp1 && bitstream.ReadByte() == exp2 && bitstream.ReadByte() == exp3))
            {
                Console.WriteLine(testFailed + "WriteReadSingleBits()");
            }
        }

        private static void testWriteReadBits()
        {
            Bitstream bitstream = new Bitstream();
            int[] bits = new int[] { 1, 1, 1, 0, 0, 1, 1, 1 }; //231
            bitstream.WriteBits(bits);
            bitstream.Seek(0, SeekOrigin.Begin);
            int[] readedBits = bitstream.ReadBits();
            for (int i = 0; i < readedBits.Length; i++)
            {
                if (bits[i] != readedBits[i])
                {
                    Console.WriteLine(testFailed + "WriteReadBits()");
                    break;
                }
            }
            bitstream.Seek(0, SeekOrigin.Begin);
            byte b = (byte)bitstream.ReadByte();
            if (b != 231)
            {
                Console.WriteLine(testFailed + "WriteReadBits()");
            }
        }

        private static void testWriteReadByte()
        {
            Bitstream bitstream = new Bitstream();
            bitstream.WriteByte(0xf1);
            bitstream.WriteByte(0xf5);
            bitstream.Seek(0, SeekOrigin.Begin);
            if (bitstream.ReadByte() != 0xf1 || bitstream.ReadByte() != 0xf5 || bitstream.ReadByte() != -1)
            {
                Console.WriteLine(testFailed + "WriteReadByte()");
            }
        }
        #endregion
    }
}
