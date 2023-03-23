using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATD_File_Handler
{
    internal class Configs
    {
        //Global variables
        public static string __channelsListFilename = "ChannelsList.txt";
        public const string DEFAULT_ALL_EXCEL_FILE = "all_spe.xlsx";
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
                    PeaksDefinition = new List<PeaksToCalc>()
                {
                    new PeaksToCalc() { startChannel = 181, endChannel = 218, averagingWindow = 5, RangeName = "Ra-226" },
                    new PeaksToCalc() { startChannel = 446, endChannel = 511, averagingWindow = 5, RangeName = "K-40" },
                    new PeaksToCalc() { startChannel = 825, endChannel = 898, averagingWindow = 5, RangeName = "Th-232" },
                    new PeaksToCalc() { startChannel = 550, endChannel = 616, averagingWindow = 5, RangeName = "Ra-226*" }
                }
                };
                Spectrometer alpha_spec = new Spectrometer()
                {
                    Name = "Alpha",
                    PeaksDefinition = new List<PeaksToCalc>()
                {
                    new PeaksToCalc() { startChannel = 184, endChannel = 221, averagingWindow = 3, RangeName = "Ra-226" },
                    new PeaksToCalc() { startChannel = 426, endChannel = 491, averagingWindow = 5, RangeName = "K-40" },
                    new PeaksToCalc() { startChannel = 780, endChannel = 869, averagingWindow = 5, RangeName = "Th-232" },
                    new PeaksToCalc() { startChannel = 514, endChannel = 577, averagingWindow = 3, RangeName = "Ra-226*" }
                }
                };
                return new Dictionary<string, Spectrometer>() { { "IIE", iie_spec },{ "Alpha",alpha_spec} };
            }
            
        }
    }
}
