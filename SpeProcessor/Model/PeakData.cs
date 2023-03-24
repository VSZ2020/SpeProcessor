using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeProcessor
{
    public class PeakData
    {
        public string PeakName { get; set; }
        public float CountRate { get; set; }
        public float PeakArea { get; set; }
        public float FullPeakArea { get; set; }
        public float VariationCoefficient
        {
            get
            {
                float coeff = 0.0F;
                if (PeakArea > 0)
                {
                    coeff = (float)Math.Sqrt(PeakArea + FullPeakArea) / PeakArea;
                }
                return coeff;
            }
        }
    }
}
