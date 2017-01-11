using JpegConverter.Encoding;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegConverterTest
{
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClass]
    public class RunLengthEncodingTest
    {
        int[] test = {
            12, 57,45,0,0,0,0,23,
            0,-30,-16,0,0,1,0,0,
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,
        };

        [TestMethod]
        public void TestRunLengthEncoding()
        {
            RunLengthEncoding.CreateAcRLE(test);
        }
    }
}
