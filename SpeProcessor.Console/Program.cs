using SpeProcessor;
using SpeProcessor.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace SpeProcessorConsole
{
    class Program
    {
        static void Main(string[] args)
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
            List<PeakRangeData> peakRanges = Configs.GetDefaultSpectrometers()["IIE"].PeaksDefinition;

            Console.Write("Searching the '*.spe' files in subfolders. Please, wait ...");
            List<string> speFilePaths = FileSearcher.SearchSpectrumFilesAsync(WORKDIR);
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

        /// <summary>
        /// Calculates area under peak in predefined range
        /// </summary>
        /// <param name="counts">Array of pulses</param>
        /// <param name="start_channel_index">Index of start channel for integration</param>
        /// <param name="end_channel_index"> Index of the last channel for integration</param>
        /// <returns></returns>
        //private static void GetPeakParameters(SpectrumFile file, PeakRangeData p)
        //{
        //    //Смещение каналов при обработке спектра
        //    int channels_offset = (file.Detector.Name == "Alpha") ? +1 : 0;
        //    int start_channel_index = p.StartChannelIndex + channels_offset;
        //    int end_channel_index = p.EndChannelIndex + channels_offset;

        //    //Массив числа импульсов в спектре
        //    int[] counts = file.Counts;
        //    if (counts.Length < 2 || start_channel_index > end_channel_index)
        //    {
        //        return;
        //    }
        //    else
        //    {
        //        //Размер окна усреднения
        //        int window_of_averaging = p.AveragingWindowSize;
        //        //Среднее по 5 каналам, предшествующим началу пика
        //        float average_at_start = 0.0F;
        //        //Среднее по 5 каналам, последующим после конца пика
        //        float average_at_end = 0.0F;
        //        //Количество обрабатываемых каналов
        //        int handled_channels_count = end_channel_index - start_channel_index - 1;

        //        //Усредняем по началу пика
        //        if (start_channel_index > 0)
        //        {
        //            //Усредняем по отсчетам в каналах (идем в сторону уменьшения индекса канала)
        //            int counts_sum = 0;
        //            int channels_count = 0;
        //            for (int i = start_channel_index; i >= 0 && (start_channel_index - i) < window_of_averaging; i--)
        //            {
        //                counts_sum += counts[i];
        //                channels_count++;
        //            }
        //            average_at_start = (float)counts_sum / channels_count;
        //        }
        //        //Если до указанного канала нет значений, то принимаем первый канал за среднее
        //        else
        //            average_at_start = counts[0];

        //        //Усредняем по концу пика
        //        if (end_channel_index < counts.Length - 1)
        //        {
        //            int counts_sum = 0;
        //            int channels_count = 0;
        //            for (int i = end_channel_index; i < counts.Length && (i - end_channel_index) < window_of_averaging; i++)
        //            {
        //                counts_sum += counts[i];
        //                channels_count++;
        //            }
        //            average_at_end = (float)counts_sum / channels_count;
        //        }
        //        //Если после указанного канала нет значений, то принимаем последний канал за среднее
        //        else
        //            average_at_end = counts[counts.Length - 1];

        //        //Получаем коэффициенты уравнения прямой пъедестала
        //        float slope = (average_at_end - average_at_start) / handled_channels_count;
        //        //float intercept = counts[start_channel_index] - slope * start_channel_index;
        //        //Trace.WriteLine($"Наклон: {slope}, отрезок: {intercept}");

        //        //Integration with rectangle-based method
        //        //In this case we have taken into account the baseline of peak 
        //        float peak_sum = 0f;
        //        float total_counts_sum = 0f;
        //        float psum = average_at_start;
        //        for (int i = start_channel_index + 1; i < end_channel_index; i++)
        //        {
        //            //sum += counts[i] - (slope * i + intercept);          //Step is 1 and equal to channel width
        //            psum += slope;
        //            peak_sum += counts[i] - psum;
        //            total_counts_sum += counts[i];
        //        }
        //        //Записываем полученные значения
        //        var peak_data = new PeakData();
        //        peak_data.PeakArea = peak_sum;
        //        peak_data.FullPeakArea = total_counts_sum;
        //        peak_data.PeakName = p.RangeName;
        //        peak_data.CountRate = peak_sum / file.MeasurementDuration;
        //        file.PeakData.Add(p.RangeName, peak_data);
        //    }
        //}

    }
}
