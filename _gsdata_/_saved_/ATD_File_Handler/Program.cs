using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ATD_File_Handler
{
    public class SpectrumFile
    {
        public string FileName { get; set; }
        public int[] Counts;
        public float[] ChannelEnergyFit;
        public int measurementTime = 3600;
        public DateTime measDateTime;
        public float CountRate = 0F;
        public float Temperature = 1;
        public float DoseRate = 0F;
        public string ActivityResult = "";
        public string EffectiveActivity = "";
        public string Geometry = "";

        public SpectrumFile(string name)
        {
            this.FileName = name;
        }
        public SpectrumFile(string name, int ChannelsCount)
        {
            this.FileName = name;
            Counts = new int[ChannelsCount];
            ChannelEnergyFit = new float[ChannelsCount];
        }
    }

    class Program
    {
        public static int startChannelIndex = 181;
        public static int endChannelIndex = 218;

        static void Main(string[] args)
        {
            string WORKDIR = ".";
            Console.WriteLine("Welcome to the 'spe' file handler!");
            if (args.Length > 0)
            {
                if (args[0].Substring(args[0].Length - 4, 4) == ".spe")
                    WORKDIR = Directory.GetParent(args[0]).FullName;
                else
                    WORKDIR = args[0];
                
            }
            Console.WriteLine("\tCurrent working folder is " + WORKDIR);
            Console.Write("\tSearching the '*.spe' files in subfolders...");
            List<string> speFilePaths = SearchSpectrumFiles(WORKDIR);
            Console.WriteLine("OK!");
            if (speFilePaths.Count > 0)
            {
                Console.Write("\tThere are " + speFilePaths.Count.ToString() + " files in working directory and subfolders. Do you want to list them? (Y/N): ");
                char inp = Console.ReadKey().KeyChar;
                Console.Write("\n");
                if (inp == 'Y' || inp == 'y')
                {
                    for (int i = 0; i < speFilePaths.Count; i++)
                    {
                        Console.WriteLine("\t\t\\" + Directory.GetParent(speFilePaths[i]).Name + "\\" + Path.GetFileName(speFilePaths[i]));
                    }
                }

                Console.WriteLine("\tReading files and extracting of data");
                Console.WriteLine("------------------------------ ------------------------------ ---------- ---------- ----------");
                Console.WriteLine(string.Format("{0,30}|{1,30}|{2,10}|{3,10}|{4,10}", "File name", "Total count rate (CR)", "CR K-40", "CR Ra-226", "CR Th-232"));
                Console.WriteLine("------------------------------ ------------------------------ ---------- ---------- ----------");
                List<SpectrumFile> speFilesList = new List<SpectrumFile>(0);
                for (int i = 0; i < speFilePaths.Count; i++)
                {
                    //Console.Write("\t\t" + Path.GetFileName(speFilePaths[i]) + "...");
                    //speFilesList.Add(ReadSpecFile(speFilePaths[i]));
                    var spData = ReadSpecFile(speFilePaths[i]);
                    //Console.Write("OK! -->  ");
                    float CR_Ra226 = GetIntegral(spData.Counts, 181, 218) / spData.measurementTime;
                    float CR_K40 = GetIntegral(spData.Counts, 446, 511) / spData.measurementTime;
                    float CR_Th232 = GetIntegral(spData.Counts, 825, 897) / spData.measurementTime;
                    //Console.WriteLine("Count rate (I_meas) is " + CR.ToString() + " s^-1");
                    Console.WriteLine(string.Format("{0,30}|{1,30}|{2,10}|{3,10}|{4,10}", 
                        Path.GetFileName(speFilePaths[i]), 
                        spData.CountRate, 
                        CR_K40, CR_Ra226, CR_Th232));
                }
            }
            else
                Console.WriteLine("\tThere are no spectum files in working directory.");

            Console.Write("Press any key to exit from application...");
            Console.ReadKey();
        }
        
        private static List<string> SearchSpectrumFiles(string rootFolder)
        {
            List<string> speFiles = new List<string>(0);
            foreach (string f in Directory.GetFiles(rootFolder))
            {
                if (f.Substring(f.Length-4, 4) == ".spe")
                {
                    speFiles.Add(f);
                }
            }
            foreach(string dir in Directory.GetDirectories(rootFolder))
            {
                speFiles.AddRange(SearchSpectrumFiles(dir));
            }
            return speFiles;
        }

        private static SpectrumFile ReadSpecFile(string fileName)
        {
            //Creating spectrum file class
            var file = new SpectrumFile(Path.GetFileNameWithoutExtension(fileName));
            //Start reading file line by line
            using (StreamReader rd = new StreamReader(fileName))
            {
                long lineCounter = 0;

                string line = "";
                while((line = rd.ReadLine())!= null)
                {
                    lineCounter++;
                    //Line with start date
                    if (line == "$DATE_MEA:")
                    {
                        //Extract start date from file
                        DateTime.TryParse(rd.ReadLine(), out file.measDateTime);
                        continue;
                    }
                    if (line == "$CPS:")
                    {
                        float.TryParse(rd.ReadLine(), out file.CountRate);
                    }
                    if (line == "$MEAS_TIM:")
                    {
                        int.TryParse(rd.ReadLine().Split(' ')[0], out file.measurementTime);
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
                        file.ChannelEnergyFit = new float[channelsCount];
                        for (int i = 0; i < channelsCount; i++)
                        {
                            line = rd.ReadLine();
                            float energy = 0;
                            float.TryParse(line.Split(' ')[1], out energy);
                            file.ChannelEnergyFit[i] = energy;
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
            }

            return file;
        }

        private static float GetIntegral(int[] counts, int startChannel, int endChannel)
        {
            if (counts.Length < 2) return 0F;
            float sum = 0f;
            for (int i = startChannel; i < endChannel; i++)
            {
                sum += (counts[i] + counts[i + 1]) * 1.0F;          //Step is 1 and equal to channel width
            }
            return sum / 2.0F;
        }
    }
}
