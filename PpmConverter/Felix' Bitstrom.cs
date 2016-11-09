using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PpmConverter
{
    class Felix__Bitstrom
    {
        public static void createBitFile()
        {            
            StreamWriter sw = new StreamWriter("C:/Users/Felix Hahmann/Desktop/Bits.txt");
            Random rnd = new Random();

            for (int i = 0; i < 10000000; i++)
            {
                sw.Write(rnd.Next(2));
            }

            sw.Close();
        }
    }
}
