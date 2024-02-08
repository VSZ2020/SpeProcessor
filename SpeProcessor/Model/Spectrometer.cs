using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SpeProcessor
{
    public class Spectrometer
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public List<PeakRangeData> PeaksDefinition { get; set; } = new();
        public Dictionary<string, float> CalibrationCoefficients = new();
    }
}
