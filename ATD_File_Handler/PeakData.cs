using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATD_File_Handler
{
    public class PeakData
    {
        public string PeakName { get; set; }
        public float CountRate { get; set; }
        public float PeakArea { get; set; }
        public float TotalPeakSum { get; set; }
        public float VariationCoefficient
        {
            get
            {
                float coeff = 0.0F;
                if (PeakArea > 0)
                {
                    coeff = (float)Math.Sqrt(PeakArea + TotalPeakSum) / PeakArea;
                }
                return coeff;
            }
        }
    }
}
