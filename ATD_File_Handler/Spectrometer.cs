using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ATD_File_Handler
{
    public class Spectrometer
    {
        public string Name { get; set; }
        public List<PeaksToCalc> PeaksDefinition { get; set; }
    }
}
