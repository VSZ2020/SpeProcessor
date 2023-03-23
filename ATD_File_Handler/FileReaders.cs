using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATD_File_Handler
{
    internal class FileReaders
    {
        public static SpectrumFile ReadSpecFile(string file_path)
        {
            //Creating spectrum file class
            var file = new SpectrumFile(file_path);
            //Start reading file line by line
            using (StreamReader rd = new StreamReader(file_path))
            {
                long lineCounter = 0;

                string line = "";
                while ((line = rd.ReadLine()) != null)
                {
                    lineCounter++;
                    //Line with start date
                    if (line == "$DATE_MEA:")
                    {
                        //Extract start date from file
                        file.measurementDateTime = rd.ReadLine();
                        continue;
                    }
                    if (line == "$CPS:")
                    {
                        float.TryParse(rd.ReadLine(), out file.TotalCountRate);
                    }
                    if (line == "$MEAS_TIM:")
                    {
                        int.TryParse(rd.ReadLine().Split(' ')[0], out file.measurementDuration);
                        continue;
                    }
                    if (line == "$DATA:")
                    {
                        line = rd.ReadLine();
                        int startChannel = 0;
                        int endChannel = 0;
                        string[] buf = line.Split(' ');
                        int.TryParse(buf[0], out startChannel);
                        int.TryParse(buf[1], out endChannel);
                        int channelsCount = endChannel - startChannel + 1;          //Gets the channels amount

                        //Create array for channels counts and arrays
                        file.Counts = new int[channelsCount];
                        //Read all channels
                        int bufNum = 0;
                        for (int i = 0; i < channelsCount; i++)
                        {
                            line = rd.ReadLine();
                            if (int.TryParse(line.Trim(), out bufNum))
                                file.Counts[i] = bufNum;
                            else
                                continue;
                        }
                        continue;
                    }
                    if (line == "$ENER_TABLE:")
                    {
                        line = rd.ReadLine();
                        int channelsCount = 0;
                        int.TryParse(line, out channelsCount);
                        file.ChannelToEnergyFit = new float[channelsCount];
                        for (int i = 0; i < channelsCount; i++)
                        {
                            line = rd.ReadLine();
                            float energy = 0;
                            float.TryParse(line.Split(' ')[1], out energy);
                            file.ChannelToEnergyFit[i] = energy;
                        }
                        continue;
                    }
                    if (line == "$TEMPERATURE:")
                    {
                        float.TryParse(rd.ReadLine(), out file.Temperature);
                        //TO-DO
                    }
                    if (line == "$DOSE_RATE:")
                    {
                        float.TryParse(rd.ReadLine(), out file.DoseRate);
                    }
                    if (line == "$DU_NAME:")
                    {
                        file.DU_NAME = rd.ReadLine();
                    }
                    if (line == "$RADIONUCLIDES:")
                    {
                        file.Radionuclides = rd.ReadLine();
                    }
                    if (line == "$ACTIVITYRESULT:")
                    {
                        file.ActivityResult = rd.ReadLine();
                    }
                    if (line == "$EFFECTIVEACTIVITYRESULT:")
                    {
                        file.EffectiveActivity = rd.ReadLine();
                    }
                    if (line == "$GEOMETRY:")
                    {
                        file.Geometry = rd.ReadLine();
                    }
                }
                //Detect spectrometer type
                if (file.Counts[0] == file.Counts[1] && file.Counts[1] == file.Counts[2] && file.Counts[2] == file.Counts[3])
                {
                    file.Detector = Configs.GetDefaultSpectrometers()["IIE"];
                }
                else
                    file.Detector = Configs.GetDefaultSpectrometers()["Alpha"];
            }
            return file;
        }
        
        public static List<string> SearchSpectrumFiles(string rootFolder)
        {
            List<string> speFiles = new List<string>(0);
            foreach (string f in Directory.GetFiles(rootFolder))
            {
                if (f.Substring(f.Length - 4, 4) == ".spe")
                {
                    speFiles.Add(Path.Combine(rootFolder,f));
                }
            }
            foreach (string dir in Directory.GetDirectories(rootFolder))
            {
                speFiles.AddRange(SearchSpectrumFiles(dir));
            }
            return speFiles;
        }

        public static List<string> SearchSpectrumFilesAsync(string rootFolder)
        {
            List<string> speFiles = new List<string>(0);
            Parallel.ForEach(Directory.GetFiles(rootFolder), (string f) => {
                if (f.Substring(f.Length - 4, 4) == ".spe")
                {
                    speFiles.Add(Path.Combine(rootFolder, f));
                }
            });

            Parallel.ForEach(Directory.GetDirectories(rootFolder), (string dir) =>
            {
                speFiles.AddRange(SearchSpectrumFiles(dir));
            });
            
            return speFiles;
        }
    }
}
