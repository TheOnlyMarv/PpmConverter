using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Collections;
using System.Diagnostics;

namespace PpmConverter
{
    class FelixBitstrom
    {
        public static void createBitFile()
        {
            FileStream fs = new FileStream("C:/Users/Felix Hahmann/Desktop/Bits", FileMode.Create);
            BitArray ba = new BitArray(10000000);
            BitArray tempArray = new BitArray(8);
            Random rnd = new Random();

            for (int i = 0; i < ba.Length; i++)
            {
                ba[i] = (rnd.Next(2) == 1 ? true : false);
            }

            for (int i = 0; i < ba.Length; i+=8)
            {
                for (int k = 0; k < 8; k++)
                {
                    tempArray[k] = ba[i + k];
                }

                fs.WriteByte(ConvertBitsToByte(tempArray));
            }

            fs.Flush();
            fs.Close();
        }

        public static byte ConvertBitsToByte(BitArray input)
        {
            int len = input.Length;

            if (len != 8)
            {
                throw new ArgumentException("bits");
            }
            
            int output = 0;
            for (int i = 0; i < len; i++)
                if (input.Get(i))
                    output += (1 << (len - 1 - i));
            return (byte)output;
        }
    }
}
