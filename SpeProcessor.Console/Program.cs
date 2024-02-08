using SpeProcessor;
using SpeProcessor.IO;
using SpeProcessor.Model;

namespace SpeProcessorConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                RunSPEHandler(args);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }

        
        private static void RunSPEHandler(string[] args)
        {
            string WORKDIR = Directory.GetCurrentDirectory();
            Console.WriteLine("Welcome to the '.spe' file handler!");
            if (args.Length > 0)
            {
                if (args[0].Substring(args[0].Length - 4, 4) == ".spe")
                    WORKDIR = Directory.GetParent(args[0]).FullName;
                else
                    WORKDIR = args[0];
            }
            else
            {
                Console.WriteLine("Enter path to folder of '.spe' file:");
                string path = Console.ReadLine();
                if (path.Substring(path.Length - 4, 4) == ".spe")
                    WORKDIR = Directory.GetParent(path).FullName;
                else
                    WORKDIR = path;
            }
            Console.WriteLine("Current working folder is " + WORKDIR);
            Console.WriteLine("Reading the list of calculation channels ranges...");

            //Берем набор пиков у первого спектрометра
            List<PeakRangeData> peakRanges = AvailableSpectrometers.Spectrometers[new Guid("5315AF3E-7184-44FF-94F5-523137D24ABE")].PeaksDefinition;

            Console.Write("Searching the '*.spe' files in subfolders. Please, wait ...");
            List<string> speFilePaths = FileSearcher.SearchSpectrumFiles(WORKDIR.Trim('\"'));
            Console.WriteLine("OK!");

            if (speFilePaths.Count > 0)
            {
                Console.Write("\tThere are " + speFilePaths.Count.ToString() + " files in working directory and subfolders. \n\tDo you want to list them? (Y/N): ");
                char inp = Console.ReadKey().KeyChar;
                Console.Write("\n");
                if (inp == 'Y' || inp == 'y')
                {
                    for (int i = 0; i < speFilePaths.Count; i++)
                    {
                        Console.WriteLine("\t\\" + Directory.GetParent(speFilePaths[i]).Name + "\\" + Path.GetFileName(speFilePaths[i]));
                    }
                }

                Console.WriteLine("Reading files and extracting of data is in progress...");
                //Decoration
                string decorationLine = "------------------------------ ------------------------- ";
                string tableHeader = string.Format("{0,30}|{1,25}|", "File name", "Total count rate (CR)");
                for (int i = 0; i < peakRanges.Count; i++)
                {
                    decorationLine += "------------------- ";
                    tableHeader += string.Format("{0,19}|", "CR (" + peakRanges[i].RangeName + ")");
                }

                Console.WriteLine(decorationLine);
                Console.WriteLine(tableHeader);
                Console.WriteLine(decorationLine);

                List<SpectrumFile> speFilesList = new List<SpectrumFile>(0);
                List<HandledPeaks> handledPeaksList = new List<HandledPeaks>();
                for (int i = 0; i < speFilePaths.Count; i++)
                {
                    var speFile = FileReaders.ReadSpecFile(speFilePaths[i]);
                    Console.Write(string.Format("{0,30}|{1,25}|",
                            Path.GetFileName(speFilePaths[i]),
                            speFile.TotalCountRate));
                    var handledPeaks = SpectrumHandler.ProcessPeaks(speFile);
                    for (int j = 0; j < handledPeaks.PeaksCount; j++)
                    {
                        Console.Write(
                            string.Format("{0,12:f2}({1:f3})|",
                            handledPeaks[j].CountRate,
                            handledPeaks[j].VariationCoefficient));
                    }
                    speFilesList.Add(speFile);
                    handledPeaksList.Add(handledPeaks);
                    Console.WriteLine();
                }
                Console.WriteLine(decorationLine);
                Console.Write("Do you want to save handled data to Excel? (Y/N): ");
                inp = Console.ReadKey().KeyChar;
                Console.Write("\n");
                if (inp == 'Y' || inp == 'y')
                {
                    Console.Write("Choose the saving type (enter corresponding number):\n\t" +
                        "0 - Cancel saving\n\t" +
                        "1 - Save each 'spe' file with calculation results to Excel\n\t" +
                        "2 - Save only calculation results from all 'spe' files to one Excel file\nType ID: ");
                    inp = Console.ReadKey().KeyChar;
                    Console.Write("\n");
                    switch (inp)
                    {
                        case '1':
                            {
                                Console.Write("Do you want to use file parent folder name as Excel file name? (Y/N): ");
                                inp = Console.ReadKey().KeyChar;
                                Console.Write("\n");
                                if (inp == 'Y' || inp == 'y')
                                {
                                    SaveConfig.IsUseParentFolderName = true;
                                }
                                else
                                    SaveConfig.IsUseParentFolderName = false;
                                //Записываем каждый обработанный спектр в Excel
                                for (int i = 0; i < speFilesList.Count; i++)
                                {
                                    ExcelWriters.WriteToExcel(speFilesList[i], handledPeaksList[i], WORKDIR, SaveConfig.IsUseParentFolderName);
                                }
                                Console.WriteLine("\tFiles were saved in " + WORKDIR);
                                break;
                            }
                        case '2':
                            {
                                ExcelWriters.WriteOneToExcelAllResults(speFilesList, handledPeaksList, WORKDIR);
                                Console.WriteLine("\tFiles were saved in " + WORKDIR);
                                break;
                            }
                        default: break;
                    }
                    Console.Write("\n");
                }
            }
            else
                Console.WriteLine("\tThere are no spectum files in working directory.");

            Console.Write("Press any key to exit from application...");
            Console.ReadKey();
        }
    }
}
