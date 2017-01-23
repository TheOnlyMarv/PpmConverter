using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegConverter.Encoding
{
    using Jpeg;
    using Block = Int32;
    using RunLengthAcPairBlock = List<RunLengthAcPair>;

    public class ImageEncoder
    {
        private Image image;

        private List<Block[]> ChannelY;
        private List<Block[]> ChannelCb;
        private List<Block[]> ChannelCr;

        private List<RunLengthAcPairBlock> RunLengthAcChannelY = new List<RunLengthAcPairBlock>();
        private List<RunLengthAcPairBlock> RunLengthAcChannelCb = new List<RunLengthAcPairBlock>();
        private List<RunLengthAcPairBlock> RunLengthAcChannelCr = new List<RunLengthAcPairBlock>();

        private List<RunLengthDcPair> RunLengthDcChannelY;
        private List<RunLengthDcPair> RunLengthDcChannelCb;
        private List<RunLengthDcPair> RunLengthDcChannelCr;

        public List<KeyValuePair<RunLengthDcPair, RunLengthAcPairBlock>> forChannelY;
        public List<KeyValuePair<RunLengthDcPair, RunLengthAcPairBlock>> forChannelCb;
        public List<KeyValuePair<RunLengthDcPair, RunLengthAcPairBlock>> forChannelCr;

        private Huffman.Huffman huffmanYAc;
        private Huffman.Huffman huffmanCAc;
        private Huffman.Huffman huffmanYDc;
        private Huffman.Huffman huffmanCDc;

        private bool encoded;

        public ImageEncoder(Image image)
        {
            this.image = image;
            encoded = false;
        }

        public void StartCalculationAndEncoding()
        {
            ConvertingToYCbCr();
            SubSamplingImage();
            DCTCalculation();
            RunQuantisation();
            ZickZackSorting();
            CreateRunLengthEncoding();
            CreateHuffmann();
            PrepareForWritig();
            encoded = true;
        }

        private void PrepareForWritig()
        {
            forChannelY = new List<KeyValuePair<RunLengthDcPair, RunLengthAcPairBlock>>();
            for (int i = 0; i < RunLengthAcChannelY.Count; i++)
            {
                forChannelY.Add(new KeyValuePair<RunLengthDcPair, RunLengthAcPairBlock>(RunLengthDcChannelY[i], RunLengthAcChannelY[i]));
            }

            forChannelCb = new List<KeyValuePair<RunLengthDcPair, RunLengthAcPairBlock>>();
            for (int i = 0; i < RunLengthAcChannelCb.Count; i++)
            {
                forChannelCb.Add(new KeyValuePair<RunLengthDcPair, RunLengthAcPairBlock>(RunLengthDcChannelCb[i], RunLengthAcChannelCb[i]));
            }

            forChannelCr = new List<KeyValuePair<RunLengthDcPair, RunLengthAcPairBlock>>();
            for (int i = 0; i < RunLengthAcChannelCr.Count; i++)
            {
                forChannelCr.Add(new KeyValuePair<RunLengthDcPair, RunLengthAcPairBlock>(RunLengthDcChannelCr[i], RunLengthAcChannelCr[i]));
            }
        }

        public void WriteToJpegToFile(PPMImage ppmImage, string file)
        {
            if (!encoded)
            {
                StartCalculationAndEncoding();
            }

            JpegEncoder encoder = new JpegEncoder();
            encoder.WriteMarker(ppmImage, new List<Huffman.Huffman>() { huffmanYDc, huffmanYAc, huffmanCDc, huffmanCAc }, this);
            encoder.SaveIntoFile(file);
        }

        private void CreateHuffmann()
        {
            HuffmanForACY();
            HuffmanForACC();
            HuffmanForDCY();
            HuffmanForDCC();

            HuffmanApply();
        }

        private void HuffmanApply()
        {
            foreach (RunLengthAcPairBlock runLengthAcPairBlock in RunLengthAcChannelY)
            {
                foreach (var item in runLengthAcPairBlock)
                {
                    item.HuffmanCode = huffmanYAc.GetCode(item.PairAsByte);
                }
            }

            foreach (RunLengthAcPairBlock runLengthAcPairBlock in RunLengthAcChannelCb)
            {
                foreach (var item in runLengthAcPairBlock)
                {
                    item.HuffmanCode = huffmanCAc.GetCode(item.PairAsByte);
                }
            }

            foreach (RunLengthAcPairBlock runLengthAcPairBlock in RunLengthAcChannelCr)
            {
                foreach (var item in runLengthAcPairBlock)
                {
                    item.HuffmanCode = huffmanCAc.GetCode(item.PairAsByte);
                }
            }

            foreach (RunLengthDcPair runLengthDcPair in RunLengthDcChannelY)
            {
                runLengthDcPair.HuffmanCode = huffmanYDc.GetCode(runLengthDcPair.Category);
            }

            foreach (RunLengthDcPair runLengthDcPair in RunLengthDcChannelCb)
            {
                runLengthDcPair.HuffmanCode = huffmanCDc.GetCode(runLengthDcPair.Category);
            }

            foreach (RunLengthDcPair runLengthDcPair in RunLengthDcChannelCr)
            {
                runLengthDcPair.HuffmanCode = huffmanCDc.GetCode(runLengthDcPair.Category);
            }
        }

        private void HuffmanForDCC()
        {
            Dictionary<int, int> dict = new Dictionary<Block, Block>();
            foreach (var item in RunLengthDcChannelCb)
            {
                try
                {
                    dict[item.Category] += 1;
                }
                catch
                {
                    dict[item.Category] = 1;
                }
            }
            foreach (var item in RunLengthDcChannelCr)
            {
                try
                {
                    dict[item.Category] += 1;
                }
                catch
                {
                    dict[item.Category] = 1;
                }
            }
            huffmanCDc = new Huffman.Huffman(dict, Huffman.HuffmanTyp.ChrominanceDC);
            huffmanCDc.CreateLimitedHuffman(16, true);
        }

        private void HuffmanForDCY()
        {
            Dictionary<int, int> dict = new Dictionary<Block, Block>();
            foreach (var item in RunLengthDcChannelY)
            {
                try
                {
                    dict[item.Category] += 1;
                }
                catch
                {
                    dict[item.Category] = 1;
                }
            }
            huffmanYDc = new Huffman.Huffman(dict, Huffman.HuffmanTyp.LuminanceDC);
            huffmanYDc.CreateLimitedHuffman(16, true);
        }

        private void HuffmanForACC()
        {
            List<RunLengthAcPair> temp = new List<RunLengthAcPair>();
            foreach (var item in RunLengthAcChannelCb)
            {
                temp.AddRange(item);
            }
            foreach (var item in RunLengthAcChannelCr)
            {
                temp.AddRange(item);
            }
            Dictionary<int, int> dict = new Dictionary<Block, Block>();
            foreach (var item in temp)
            {
                try
                {
                    dict[item.PairAsByte] += 1;
                }
                catch
                {
                    dict[item.PairAsByte] = 1;
                }
            }
            huffmanCAc = new Huffman.Huffman(dict, Huffman.HuffmanTyp.ChrominanceAC);
            huffmanCAc.CreateLimitedHuffman(16, true);
        }

        private void HuffmanForACY()
        {
            List<RunLengthAcPair> temp = new List<RunLengthAcPair>();
            foreach (var item in RunLengthAcChannelY)
            {
                temp.AddRange(item);
            }
            Dictionary<int, int> dict = new Dictionary<Block, Block>();
            foreach (var item in temp)
            {
                try
                {
                    dict[item.PairAsByte] += 1;
                }
                catch
                {
                    dict[item.PairAsByte] = 1;
                }
            }
            huffmanYAc = new Huffman.Huffman(dict, Huffman.HuffmanTyp.LuminanceAC);
            huffmanYAc.CreateLimitedHuffman(16, true);
        }

        private void CreateRunLengthEncoding()
        {
            for (int i = 0; i < ChannelY.Count; i++)
            {
                RunLengthAcChannelY.Add(RunLengthEncoding.CreateAcRLE(ChannelY[i]));
            }
            for (int i = 0; i < ChannelCb.Count; i++)
            {
                RunLengthAcChannelCb.Add(RunLengthEncoding.CreateAcRLE(ChannelCb[i]));
                RunLengthAcChannelCr.Add(RunLengthEncoding.CreateAcRLE(ChannelCr[i]));
            }

            ChannelY = null;
            ChannelCb = null;
            ChannelCr = null;

            RunLengthDcChannelY = RunLengthEncoding.CreateDcRLE(image.Channel0);
            RunLengthDcChannelCb = RunLengthEncoding.CreateDcRLE(image.Channel1);
            RunLengthDcChannelCr = RunLengthEncoding.CreateDcRLE(image.Channel2);
        }

        private void ZickZackSorting()
        {
            ChannelY = Quantisation.Quantisation.CreateZickZackSortingCompleteChannel(image.Channel0);
            ChannelCb = Quantisation.Quantisation.CreateZickZackSortingCompleteChannel(image.Channel1);
            ChannelCr = Quantisation.Quantisation.CreateZickZackSortingCompleteChannel(image.Channel2);
        }

        private void RunQuantisation()
        {
            image.Channel0 = Quantisation.Quantisation.RunQuantisation(image.Channel0);
            image.Channel1 = Quantisation.Quantisation.RunQuantisation(image.Channel1, true);
            image.Channel2 = Quantisation.Quantisation.RunQuantisation(image.Channel2, true);
        }

        private void DCTCalculation()
        {
            DCT.CosinusTransformation.AraiDCT(image.Channel0);
            DCT.CosinusTransformation.AraiDCT(image.Channel1);
            DCT.CosinusTransformation.AraiDCT(image.Channel2);
        }

        private void SubSamplingImage()
        {
            image.extendMatrix();
            image.subsamplingChannel1();
            image.subsamplingChannel2();
        } // Bei Farben nochmal überprüfen

        private void ConvertingToYCbCr()
        {
            image = Image.FromRGBtoYCbCr(image);
        }
    }
}
