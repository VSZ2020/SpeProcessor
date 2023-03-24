using ClosedXML.Excel;
using SpeProcessor;
using SpeProcessor.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeProcessorConsole
{
    internal class ExcelWriters
    {
        /// <summary>
        /// Writes the spectrum file data to Excel file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="destinationFolder"></param>
        public static void WriteToExcel(SpectrumFile file, HandledPeaks peaks, string destinationFolder, bool IsUseParentFolderName)
        {
            int curCellRow = 1;
            int curCellCol = 1;
            using (var wb = new XLWorkbook())
            {
                var sheet = wb.Worksheets.Add("Data");
                //Записываем скорости счета и коэффициенты вариации
                curCellCol = 5;
                sheet.Cell(1, curCellCol).Value = "Скорости счета (имп/с)";
                foreach (var peak in peaks.Peaks)
                {
                    sheet.Cell(2, curCellCol).Value = $"{peak.PeakName}";
                    sheet.Cell(3, curCellCol).Value = $"{peak.CountRate}";
                    sheet.Cell(4, curCellCol++).Value = $"{peak.VariationCoefficient}";
                }
                curCellCol = 1;
                //Записываем остальную информацию
                sheet.Cell(curCellRow++, curCellCol).Value = "$DATE_MEA:";
                sheet.Cell(curCellRow++, curCellCol).Value = file.MeasurementDate;
                sheet.Cell(curCellRow++, curCellCol).Value = "$MEAS_TIM:";
                sheet.Cell(curCellRow++, curCellCol).Value = file.MeasurementDuration;
                sheet.Cell(curCellRow++, curCellCol).Value = "$CPS:";
                sheet.Cell(curCellRow++, curCellCol).Value = file.TotalCountRate;
                curCellRow++;
                for (int i = 0; i < file.Counts.Length; i++)
                {
                    sheet.Cell(curCellRow + i, 1).Value = i;                            //Channel index
                    sheet.Cell(curCellRow + i, 2).Value = file.ChannelToEnergyFit[i];   //Energy
                    sheet.Cell(curCellRow + i, 3).Value = file.Counts[i];               //Pulses
                }

                curCellRow += file.Counts.Length + 1;
                sheet.Cell(curCellRow++, curCellCol).Value = "$TEMPERATURE:";
                sheet.Cell(curCellRow++, curCellCol).Value = file.Temperature;
                sheet.Cell(curCellRow++, curCellCol).Value = "$SCALE_MODE:";
                sheet.Cell(curCellRow++, curCellCol).Value = "1";                       //FIXME
                sheet.Cell(curCellRow++, curCellCol).Value = "$DOSE_RATE:";
                sheet.Cell(curCellRow++, curCellCol).Value = file.DoseRate;
                sheet.Cell(curCellRow++, curCellCol).Value = "$DU_NAME:";
                sheet.Cell(curCellRow++, curCellCol).Value = file.DU_NAME;
                sheet.Cell(curCellRow++, curCellCol).Value = "$RADIONUCLIDES:";
                sheet.Cell(curCellRow++, curCellCol).Value = file.Radionuclides;
                sheet.Cell(curCellRow++, curCellCol).Value = "$ACTIVITYRESULT:";
                sheet.Cell(curCellRow++, curCellCol).Value = file.ActivityResult;
                sheet.Cell(curCellRow++, curCellCol).Value = "$EFFECTIVEACTIVITYRESULT:";
                sheet.Cell(curCellRow++, curCellCol).Value = file.EffectiveActivity;
                sheet.Cell(curCellRow++, curCellCol).Value = "$GEOMETRY:";
                sheet.Cell(curCellRow++, curCellCol).Value = file.Geometry;

                //Сохранение файла
                string filename = (IsUseParentFolderName ? (new FileInfo(file.FullPath)).Directory.Name : file.FileName) + ".xlsx";
                string filepath = Path.Combine(destinationFolder, filename);
                try
                {
                    wb.SaveAs(filepath);
                }
                catch (IOException ex)
                {
                    Console.WriteLine(">>Error during saving excel file " + file.FileName + ". This file won't be saved. Error message: " + ex.Message);
                }
            }
        }

        public static void WriteOneToExcelAllResults(List<SpectrumFile> files, IList<HandledPeaks> peaks,string destinationFolder)
        {
            int curCellRow = 1;
            int curCellCol = 1;
            var iie_peaks_definition = Configs.GetDefaultSpectrometers()["IIE"].PeaksDefinition;
            int peaksCount = iie_peaks_definition.Count;
            using (var wb = new XLWorkbook())
            {
                var sheet = wb.Worksheets.Add("Total");
                //Заголовки колонок
                sheet.Cell(curCellRow, 1).Value = "Название файла";
                sheet.Cell(curCellRow, 2).Value = "Родительская папка";
                sheet.Cell(curCellRow, 3).Value = "Полный путь к файлу";
                sheet.Cell(curCellRow, 4).Value = "Суммар. скор. счета (имп/с)";
                sheet.Cell(curCellRow, 5).Value = "Спектрометр";
                curCellCol = 6;
                for (int i = 0; i < peaksCount; i++)
                {
                    var range_name = iie_peaks_definition[i].RangeName;
                    sheet.Cell(curCellRow, curCellCol + i).Value = $"Count rate ({range_name})";
                    sheet.Cell(curCellRow, curCellCol + peaksCount + i).Value = $"Площадь пика ({range_name})";
                    sheet.Cell(curCellRow, curCellCol + peaksCount * 2 + i).Value = $"Сумма имп. в пике ({range_name})";
                    sheet.Cell(curCellRow, curCellCol + peaksCount * 3 + i).Value = $"Коэф. вариации ({range_name})";
                }

                curCellRow++;
                //Записываем численные значения
                for(int i = 0; i < files.Count; i++)
                {
                    //Запись заголовков файла
                    try
                    {
                        string parent_dir = (new FileInfo(files[i].FullPath)).Directory.Name;
                        //Trace.WriteLine(parent_dir);
                        sheet.Cell(curCellRow, 1).Value = files[i].FileName;
                        sheet.Cell(curCellRow, 2).Value = parent_dir;
                        sheet.Cell(curCellRow, 3).Value = files[i].FullPath;
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine($"Can't extract parent folder for file {files[i].FileName}\n{ex.Message}");
                        //sheet.Cell(curCellRow, 2).Value = file.FileName;
                    }
                    //Записываем скорости счета и название файла
                    sheet.Cell(curCellRow, 4).Value = files[i].TotalCountRate;
                    sheet.Cell(curCellRow, 5).Value = files[i].Detector?.Name ?? "Undefined";

                    curCellCol = 6;
                    for (int j = 0; j < peaks[i].PeaksCount; j++)
                    {
                        //Записываем скорость счета
                        sheet.Cell(curCellRow, curCellCol + j).Value = peaks[i][j].CountRate;
                        //Записываем площадь пика
                        sheet.Cell(curCellRow, curCellCol + peaksCount + j).Value = peaks[i][j].PeakArea;
                        //Записываем суммарное кол. импульсов в пике
                        sheet.Cell(curCellRow, curCellCol + peaksCount * 2 + j).Value = peaks[i][j].FullPeakArea;
                        //Записываем коэф вариации
                        sheet.Cell(curCellRow, curCellCol + peaksCount * 3 + j).Value = peaks[i][j].VariationCoefficient;

                    }
                    curCellRow++;
                }
                try
                {
                    wb.SaveAs(Path.Combine(destinationFolder, Configs.DEFAULT_ALL_EXCEL_FILE));
                }
                catch (IOException ex)
                {
                    Console.WriteLine(">>Error during saving excel file. This file won't be saved. Error message: " + ex.Message);
                }
            }
        }
    }
}
