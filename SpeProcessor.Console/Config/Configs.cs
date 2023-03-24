using SpeProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeProcessorConsole
{
    internal class Configs
    {
        //Global variables
        //public static string __channelsListFilename = "ChannelsList.txt";
        public const string DEFAULT_ALL_EXCEL_FILE = "All_spe.xlsx";
        public static Dictionary<string, Spectrometer> Spectrometers { get; set; }
        public static Dictionary<string, Spectrometer> GetDefaultSpectrometers()
        {
            if (Spectrometers != null)
                return Spectrometers;
            else
            {
                Spectrometer iie_spec = new Spectrometer()
                {
                    Name = "IIE",
                    PeaksDefinition = new List<PeakRangeData>()
                {
                    new PeakRangeData() { StartChannelIndex = 181, EndChannelIndex = 218, AveragingWindowSize = 5, RangeName = "Ra-226" },
                    new PeakRangeData() { StartChannelIndex = 446, EndChannelIndex = 511, AveragingWindowSize = 5, RangeName = "K-40" },
                    new PeakRangeData() { StartChannelIndex = 825, EndChannelIndex = 898, AveragingWindowSize = 5, RangeName = "Th-232" },
                    new PeakRangeData() { StartChannelIndex = 550, EndChannelIndex = 616, AveragingWindowSize = 5, RangeName = "Ra-226*" }
                }
                };
                Spectrometer alpha_spec = new Spectrometer()
                {
                    Name = "Alpha",
                    PeaksDefinition = new List<PeakRangeData>()
                {
                    new PeakRangeData() { StartChannelIndex = 185, EndChannelIndex = 223, AveragingWindowSize = 3, RangeName = "Ra-226" },
                    new PeakRangeData() { StartChannelIndex = 427, EndChannelIndex = 493, AveragingWindowSize = 5, RangeName = "K-40" },
                    new PeakRangeData() { StartChannelIndex = 782, EndChannelIndex = 870, AveragingWindowSize = 5, RangeName = "Th-232" },
                    new PeakRangeData() { StartChannelIndex = 515, EndChannelIndex = 578, AveragingWindowSize = 3, RangeName = "Ra-226*" }
                }
                };
                return new Dictionary<string, Spectrometer>() { { "IIE", iie_spec },{ "Alpha",alpha_spec} };
            }
            
        }
    }
}
