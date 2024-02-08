namespace SpeProcessor
{
    public class AvailableSpectrometers
    {
        private static Dictionary<Guid, Spectrometer>? spectrometers;
        public static Dictionary<Guid, Spectrometer> Spectrometers
        {
            get
            {
                return spectrometers == null ? Get() : spectrometers;
            }
        }

        /// <summary>
        /// Get spectrometer by its Name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Spectrometer? GetByName(string name)
        {
            return Spectrometers.Values.Where(sp => sp.Name == name).FirstOrDefault();
        }

        /// <summary>
        /// Returns the first spectrometer in Repository
        /// </summary>
        /// <returns></returns>
        public static Spectrometer? GetFirst() => Spectrometers.Values.First();

        /// <summary>
        /// Returns the Dictionary of available spectrometers
        /// </summary>
        /// <returns></returns>
        public static Dictionary<Guid, Spectrometer> Get()
        {
            var iie_GUID = new Guid("5315AF3E-7184-44FF-94F5-523137D24ABE");
            var alpha_GUID = new Guid("B3D7CA49-69D1-40B1-BB28-0CB065DC2C32");
            Spectrometer iie_spec = new Spectrometer()
            {
                Id = iie_GUID,
                Name = "IIE",
                PeaksDefinition = new List<PeakRangeData>()
                {
                    new PeakRangeData() { StartChannelIndex = 181, EndChannelIndex = 218, AveragingWindowSize = 5, RangeName = "Ra-226" },
                    new PeakRangeData() { StartChannelIndex = 446, EndChannelIndex = 511, AveragingWindowSize = 5, RangeName = "K-40" },
                    new PeakRangeData() { StartChannelIndex = 825, EndChannelIndex = 898, AveragingWindowSize = 5, RangeName = "Th-232" },
                    new PeakRangeData() { StartChannelIndex = 550, EndChannelIndex = 616, AveragingWindowSize = 5, RangeName = "Ra-226*" }
                },
                CalibrationCoefficients = new Dictionary<string, float>()
                {
                    {"Ra-226", 15.0398234060769f},
                    {"K-40", 3.98f },
                    {"Th-232", 1.75093106711342f},
                    {"Ra-226*", 4.75f}
                }
            };
            Spectrometer alpha_spec = new Spectrometer()
            {
                Id = alpha_GUID,
                Name = "Alpha",
                PeaksDefinition = new List<PeakRangeData>()
                {
                    new PeakRangeData() { StartChannelIndex = 185, EndChannelIndex = 223, AveragingWindowSize = 3, RangeName = "Ra-226" },
                    new PeakRangeData() { StartChannelIndex = 427, EndChannelIndex = 493, AveragingWindowSize = 5, RangeName = "K-40" },
                    new PeakRangeData() { StartChannelIndex = 782, EndChannelIndex = 870, AveragingWindowSize = 5, RangeName = "Th-232" },
                    new PeakRangeData() { StartChannelIndex = 515, EndChannelIndex = 578, AveragingWindowSize = 3, RangeName = "Ra-226*" }
                },
                CalibrationCoefficients = new Dictionary<string, float>()
                {
                    {"Ra-226", 13.4480820344375f},
                    {"K-40", 4.33f },
                    {"Th-232", 2.05880017465679f},
                    {"Ra-226*", 3.62f}
                }
            };
            return new Dictionary<Guid, Spectrometer>() { { iie_GUID, iie_spec }, { alpha_GUID, alpha_spec } };
        }
    }
}
