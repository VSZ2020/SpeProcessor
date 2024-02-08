namespace SpeProcessor
{
    public static class PeakHandler
    {
        /// <summary>
        /// Perform averaging in defined range
        /// </summary>
        /// <param name="counts"></param>
        /// <param name="fromIndex"></param>
        /// <param name="toIndex"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static float AverageInRange(int[] counts, int fromIndex, int toIndex)
        {
            if (fromIndex > toIndex)
                throw new ArgumentException($"Start index {fromIndex} must be less than end index {toIndex}");
            if (fromIndex < 0)
                fromIndex = 0;
            if (toIndex >= counts.Length)
                toIndex = counts.Length - 1;
            int sum = 0;
            int counter = 0;
            for (int i = fromIndex; i <= toIndex; i++)
            {
                sum += counts[i];
                counter++;
            }
            return (float)sum / (toIndex - fromIndex + 1);
        }

        /// <summary>
        /// Returns value of pedestal slope coefficient
        /// </summary>
        /// <param name="spectrumCounts">Origin spectrum counts array</param>
        /// <param name="peakRange"></param>
        /// <returns></returns>
        public static float GetBaseSlope(int[] spectrumCounts, PeakRangeData peakRange)
        {
            int avgWindow = peakRange.AveragingWindowSize - 1;
            float averagedPeakLeftEdge = AverageInRange(spectrumCounts, peakRange.StartChannelIndex - avgWindow, peakRange.StartChannelIndex);
            float averagedPeakRightEdge = AverageInRange(spectrumCounts, peakRange.EndChannelIndex, peakRange.EndChannelIndex + avgWindow);
            int peakPointsCount = peakRange.EndChannelIndex - peakRange.StartChannelIndex - 1;
            //Pedestal equation slope
            return (averagedPeakRightEdge - averagedPeakLeftEdge) / peakPointsCount;
        }

        /// <summary>
        /// Substract peak pedestal (base) from counts
        /// </summary>
        /// <param name="spectrumCounts">Original counts array</param>
        /// <param name="baseSlope">Slope coefficient of pedestal</param>
        /// <returns>Cleared from pedestal array of counts</returns>
        public static float[] ClearFromBase(int[] spectrumCounts, PeakRangeData peakRange)
        {
            float startSum = AverageInRange(spectrumCounts, peakRange.StartChannelIndex - peakRange.AveragingWindowSize + 1, peakRange.StartChannelIndex);
            float baseSlope = GetBaseSlope(spectrumCounts, peakRange);
            float[] cleanPeak = new float[spectrumCounts.Length];
            for (int i = peakRange.StartChannelIndex + 1; i < peakRange.EndChannelIndex; i++)
            {
                startSum += baseSlope;
                cleanPeak[i] = spectrumCounts[i] - startSum;
            }
            return cleanPeak;
        }

        /// <summary>
        /// Returns count rate value (1/sec)
        /// </summary>
        /// <param name="peakArea">Peak area</param>
        /// <param name="measurementTime">Measurement time in seconds</param>
        /// <returns></returns>
        public static float GetCountRate(float peakArea, float measurementTime) => peakArea / measurementTime;

        public static float GetPeakArea(float[] peakCounts, PeakRangeData peakRange)
        {
            float sum = 0;
            for (int i = peakRange.StartChannelIndex + 1; i < peakRange.EndChannelIndex; i++)
                sum += peakCounts[i];
            return sum;
        }

        /// <summary>
        /// Returns area under Peak
        /// </summary>
        /// <param name="spectrumCounts">Spectrum counts array</param>
        /// <param name="peakRange">Parameters of handled peak</param>
        /// <returns></returns>
        public static float GetPeakArea(int[] spectrumCounts, PeakRangeData peakRange)
        {
            int sum = 0;
            for (int i = peakRange.StartChannelIndex + 1; i < peakRange.EndChannelIndex; i++)
                sum += spectrumCounts[i];
            return sum;
        }

        /// <summary>
        /// Returns peak object with calculated parameters
        /// </summary>
        /// <param name="counts"></param>
        /// <param name="peakRange"></param>
        /// <param name="measurementTime"></param>
        /// <returns></returns>
        public static PeakData HandleSinglePeak(int[] counts, PeakRangeData peakRange, float measurementTime)
        {
            var modifiedCounts = ClearFromBase(counts, peakRange);
            var peakArea = GetPeakArea(modifiedCounts, peakRange);

            return new PeakData()
            {
                PeakName = peakRange.RangeName,
                PeakArea = peakArea,
                FullPeakArea = GetPeakArea(counts, peakRange),
                CountRate = GetCountRate(peakArea, measurementTime)
            };
        }

        /// <summary>
        /// Returns handled peaks for determined ranges
        /// </summary>
        /// <param name="counts">Pulses spectrum</param>
        /// <param name="peakRanges">List of PeakRangeData with peak limits</param>
        /// <param name="measurementTime">Measurement duration in seconds</param>
        /// <returns></returns>
        public static IList<PeakData> HandlePeaks(int[] counts, IList<PeakRangeData> peakRanges, float measurementTime)
        {
            return peakRanges.Select(range => HandleSinglePeak(counts, range, measurementTime)).ToList();
        }
    }
}
