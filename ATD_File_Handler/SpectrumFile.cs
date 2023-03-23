using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATD_File_Handler
{
    public class SpectrumFile
    {
        public string FullPath { get; set; }
        public string FileName { get; set; }
        public int[] Counts;
        public float[] ChannelToEnergyFit;
        public int measurementDuration = 3600;
        public string measurementDateTime;
        public float TotalCountRate = 0F;
        public float Temperature = 1;
        public float DoseRate = 0F;
        public string DU_NAME = "";
        public string Radionuclides = "";
        public string ActivityResult = "";
        public string EffectiveActivity = "";
        public string Geometry = "";

        public Dictionary<string, PeakData> PeakData;
        public Spectrometer Detector { get; set; }

        public SpectrumFile(string full_path)
        {
            FullPath = full_path;
            this.FileName = Path.GetFileNameWithoutExtension(full_path);
        }
        public SpectrumFile(string full_path, int ChannelsCount)
        {
            FullPath = full_path;
            this.FileName = Path.GetFileNameWithoutExtension(full_path);
            Counts = new int[ChannelsCount];
            ChannelToEnergyFit = new float[ChannelsCount];
        }
    }

}
