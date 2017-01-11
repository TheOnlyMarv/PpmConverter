using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JpegConverter.Encoding
{
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

        RunLengthDcPair RunLengthDcChannelY;
        RunLengthDcPair RunLengthDcChannelCb;
        RunLengthDcPair RunLengthDcChannelCr;

        Huffman.Huffman huffmanColor;
        Huffman.Huffman huffmanAcDc;

        public ImageEncoder(Image image)
        {
            this.image = image;
        }

        public void StartCalculationAndEncoding()
        {
            ConvertingToYCbCr();
            SubSamplingImage();
            DCTCalculation();
            RunQuantisation();
            ZickZackSorting();
            CreateRunLengthEncoding();
        }

        private void CreateRunLengthEncoding()
        {
            for (int i = 0; i < ChannelY.Count; i++)
            {
                RunLengthAcChannelY.Add(RunLengthEncoding.CreateAcRLE(ChannelY[i]));
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
        }

        private void ConvertingToYCbCr()
        {
            image = Image.FromRGBtoYCbCr(image);
        }
    }
}
