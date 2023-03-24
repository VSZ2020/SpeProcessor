using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeProcessor
{
    public struct PeakRangeData
    {
        public string RangeName { get; set; }
        public int StartChannelIndex { get; set; }
        public int EndChannelIndex { get; set; }
        public int AveragingWindowSize { get; set; }

        public int PeakChannelsCount => EndChannelIndex - StartChannelIndex - 1;
    }
}
