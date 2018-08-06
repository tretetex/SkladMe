using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using SkladMe.ViewModel;
using Excel = Microsoft.Office.Interop.Excel;


namespace SkladMe.Infrastructure
{
    internal class Export
    {
        public static async Task SaveCollection(IList<ProductVM> coll,
            CancellationToken token = default(CancellationToken))
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "CSV-документ|*.csv|Excel-документ|*.xlsx",
                Title = "Сохранение результатов",
                InitialDirectory = Directory.GetCurrentDirectory()
            };
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                switch (saveFileDialog.FilterIndex)
                {
                    case 1:
                        await Task.Run(() => WriteToCsv(coll, saveFileDialog.FileName), token).ConfigureAwait(false);
                        break;
                    case 2:
                        await Task.Run(() => WriteToXls(coll, saveFileDialog.FileName), token).ConfigureAwait(false);
                        break;
                    default: return;
                }
            }
        }

        public static void WriteToXls(IList<ProductVM> products, string path)
        {
            if (products.Count == 0)
            {
                return;
            }
            int rowPerSheet = 20000;
            Excel.Application oXL = new Excel.Application {DisplayAlerts = false};
            Excel.Workbook oWB = oXL.Workbooks.Add(Missing.Value);
            Excel.Worksheet oWS = AddWorksheet(oWB, $"0-{rowPerSheet}");

            oWB.Sheets[1].Delete();

            var list = new List<object[]>();

            for (int i = 0; i < products.Count; i++)
            {
                var data = new object[24];
                
                if (i > 0 && i % rowPerSheet == 0)
                {
                    WriteDataToExcel(oWS, list);
                    list.Clear();
                    oWS = AddWorksheet(oWB, $"{i}-{i + rowPerSheet}");
                }

                var p = products.ElementAt(i).Model;

                data[0] = p.Id;
                data[1] = ChangeStringIfFirstSymbolAsEqualSign(p.Prefix.Title);
                data[2] = p.Chapter.Title;
                data[3] = p.Subсhapter.Title;
                data[4] = p.Title;
                data[5] = p.Fee;
                data[6] = p.Price;

                var feeToPrice = products.ElementAt(i).FeeToPrice ?? 0;
                data[7] = Math.Round(feeToPrice, 3);

                data[8] = ChangeStringIfFirstSymbolAsEqualSign(p.Organizer?.Nickname);

                data[9] = p.DateOfCreation.ToShortDateString();
                data[10] = products.ElementAt(i).Involvement;
                data[11] = p.Popularity;
                data[12] = p.OrganizerRating;
                data[13] = p.ClubMemberRating;
                data[14] = p.MembersAsMainCount + p.MembersAsReserveCount;
                data[15] = p.MembersAsMainCount;
                data[16] = p.MembersAsReserveCount;
                data[17] = p.PeopleForMin;
                data[18] = ChangeStringIfFirstSymbolAsEqualSign(p.Creator.Nickname);

                string feeDate = null;
                if (p.DateFee.HasValue)
                {
                    feeDate = p.DateFee.Value.ToShortDateString();
                }
                data[19] = feeDate;
                data[20] = p.ReviewCount;
                data[21] = p.Rating;
                data[22] = p.ViewCount;
                data[23] = ChangeStringIfFirstSymbolAsEqualSign(p.Note);
                
                list.Add(data);
            }

            WriteDataToExcel(oWS, list);

            oWB.SaveAs(path, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                Excel.XlSaveAsAccessMode.xlNoChange, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                Missing.Value);
            oWB.Close(true, Missing.Value, Missing.Value);
        }

        private static void WriteDataToExcel(Excel.Worksheet ws, List<object[]> data)
        {
            var cellsCount = 24;
            var array = new object[data.Count, cellsCount];
            for (int i = 0; i < data.Count; i++)
            {
                for (int j = 0; j < cellsCount; j++)
                {
                    array[i, j] = data[i][j];
                }
            }

            var startCell = (Excel.Range) ws.Cells[2, 1];
            var endCell = (Excel.Range) ws.Cells[data.Count + 1, cellsCount]; 
            var writeRange = ws.Range[startCell, endCell];

            writeRange.Value2 = array;
        }

        private static Excel.Worksheet AddWorksheet(Excel.Workbook owb, string sheetName)
        {
            owb.Sheets.Add(After: owb.Sheets[owb.Sheets.Count]);
            Excel.Worksheet oWS = owb.Worksheets[owb.Worksheets.Count] as Excel.Worksheet;

            oWS.Name = sheetName;
            oWS.Cells[1, 1] = "Id";
            oWS.Cells[1, 2] = "Префикс";
            oWS.Cells[1, 3] = "Раздел";
            oWS.Cells[1, 4] = "Подраздел";
            oWS.Cells[1, 5] = "Название";
            oWS.Cells[1, 6] = "Взнос";
            oWS.Cells[1, 7] = "Цена";
            oWS.Cells[1, 8] = "Взнос к цене";
            oWS.Cells[1, 9] = "Организатор";
            oWS.Cells[1, 10] = "Дата создания";
            oWS.Cells[1, 11] = "Вовлеченность";
            oWS.Cells[1, 12] = "Популярность";
            oWS.Cells[1, 13] = "Рейтинг для Орга";
            oWS.Cells[1, 14] = "Рейтинг для ЧК";
            oWS.Cells[1, 15] = "Пользователи";
            oWS.Cells[1, 16] = "В основном списке";
            oWS.Cells[1, 17] = "В резервном списке";
            oWS.Cells[1, 18] = "Человек до минималки";
            oWS.Cells[1, 19] = "Топикстартер";
            oWS.Cells[1, 20] = "Сбор взносов";
            oWS.Cells[1, 21] = "Отзывы";
            oWS.Cells[1, 22] = "Оценка";
            oWS.Cells[1, 23] = "Просмотры";
            oWS.Cells[1, 24] = "Заметка";

            oWS.Cells[1, 1].EntireRow.Font.Bold = true;

            return oWS;
        }

        private static string ChangeStringIfFirstSymbolAsEqualSign(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return null;
            return str[0] == '=' ? "\"" + str + "\"" : str;
        }

        private static void WriteToCsv(IEnumerable<ProductVM> products, string filepath)
        {
            string delimiter = ";";
            string headers =
                $"Id{delimiter}Префикс{delimiter}Раздел{delimiter}Подраздел{delimiter}Название{delimiter}Взнос{delimiter}Цена{delimiter}" +
                $"Взнос к цене{delimiter}Организатор{delimiter}Дата создания{delimiter}Вовлеченность{delimiter}Популярность{delimiter}Рейтинг для Орга{delimiter}" +
                $"Рейтинг для ЧК{delimiter}Пользователи{delimiter}В основном списке{delimiter}В резервном списке{delimiter}Человек до минималки{delimiter}" +
                $"Топикстартер{delimiter}Сбор взносов{delimiter}Отзывы{delimiter}Оценка{delimiter}Просмотры{delimiter}Заметка";

            using (var writer = new StreamWriter(filepath))
            {
                writer.WriteLine(headers);

                foreach (var product in products)
                {
                    var productCsv = GetProductCsv(product, delimiter);
                    writer.WriteLine(productCsv);
                }
            }
        }

        private static string GetProductCsv(ProductVM p, string delimiter)
        {
            var sb = new StringBuilder();

            sb.Append(p.Model.Id + delimiter);
            sb.Append(EscapeCsvString(p.Model.Prefix.Title) + delimiter);
            sb.Append(EscapeCsvString(p.Model.Chapter.Title) + delimiter);
            sb.Append(EscapeCsvString(p.Model.Subсhapter.Title) + delimiter);
            sb.Append(EscapeCsvString(p.Model.Title) + delimiter);
            sb.Append(EscapeCsvString(p.Model.Price.ToString()) + delimiter);
            sb.Append(EscapeCsvString(p.Model.Fee.ToString()) + delimiter);
            sb.Append(EscapeCsvString(p.FeeToPrice?.ToString("#.###")) + delimiter);
            sb.Append(EscapeCsvString(p.Model.Organizer?.Nickname) + delimiter);
            sb.Append(EscapeCsvString(p.Model.DateOfCreation.ToString("dd.MM.yyyy")) + delimiter);
            sb.Append(EscapeCsvString(p.Involvement.ToString("#.###")) + delimiter);
            sb.Append(EscapeCsvString(p.Model.Popularity.ToString("#.###")) + delimiter);
            sb.Append(EscapeCsvString(p.Model.OrganizerRating.ToString("#.###")) + delimiter);
            sb.Append(EscapeCsvString(p.Model.ClubMemberRating.ToString("#.###")) + delimiter);
            sb.Append(EscapeCsvString(p.Model.UsersTotalCount.ToString()) + delimiter);
            sb.Append(EscapeCsvString(p.Model.MembersAsMainCount.ToString()) + delimiter);
            sb.Append(EscapeCsvString(p.Model.MembersAsReserveCount.ToString()) + delimiter);
            sb.Append(EscapeCsvString(p.Model.PeopleForMin.ToString()) + delimiter);
            sb.Append(EscapeCsvString(p.Model.Creator?.Nickname) + delimiter);
            sb.Append(p.Model.DateFee?.ToString("dd.MM.yyyy") + delimiter);
            sb.Append(EscapeCsvString(p.Model.ReviewCount.ToString()) + delimiter);
            sb.Append(EscapeCsvString(p.Model.Rating.ToString()) + delimiter);
            sb.Append(EscapeCsvString(p.Model.ViewCount.ToString()) + delimiter);
            sb.Append(EscapeCsvString(p.Model.Note));
            return sb.ToString();
        }

        private static string EscapeCsvString(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return null;
            bool match = str.IndexOfAny(";,\"".ToCharArray()) != -1;
            return match ? $"\"{str}\"" : str;
        }
    }
}