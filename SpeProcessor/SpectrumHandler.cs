using SpeProcessor.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeProcessor
{
    public class SpectrumHandler
    {
        /// <summary>
        /// Handle spe-file peaks and save to new object
        /// </summary>
        /// <param name="speFile">Read spectrum file</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static HandledPeaks ProcessPeaks(SpectrumFile speFile)
        {
            if (speFile.Detector == null)
                throw new ArgumentNullException("Spectrometer field can't be NULL!");
            if (speFile.Detector.PeaksDefinition.Count < 1)
                throw new ArgumentException($"Peaks range data is empty for spectrum file {speFile.FileName}");

            var peaksData = PeakHandler.HandlePeaks(speFile.Counts, speFile.Detector.PeaksDefinition, speFile.MeasurementDuration);
           
            return new HandledPeaks(speFile)
                .SetHandledPeaksData(peaksData);
        }

        /// <summary>
        /// Handle list of Spectrum files and returns list of handled peaks for each file
        /// </summary>
        /// <param name="filesList"></param>
        /// <returns></returns>
        public static IList<HandledPeaks> ProcessSpeFiles(IList<SpectrumFile> filesList)
        {
            return filesList.Select(file => ProcessPeaks(file)).ToList();
        }
    }
}
