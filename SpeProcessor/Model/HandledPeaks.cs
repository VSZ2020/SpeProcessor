using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeProcessor.Model
{
    public class HandledPeaks
    {
        public readonly SpectrumFile SpeFile;
        public IList<PeakData> Peaks { get; private set; } = new List<PeakData>();

        public PeakData this[int peakIndex]
        {
            get => Peaks[peakIndex];
            set
            {
                Peaks[peakIndex] = value;
            }
        }
        public int PeaksCount => Peaks.Count;
        public HandledPeaks(SpectrumFile speFile)
        {
            this.SpeFile = speFile;
        }

        public HandledPeaks SetHandledPeaksData(IList<PeakData> peaksData)
        {
            this.Peaks = peaksData;
            return this;
        }
    }
}
