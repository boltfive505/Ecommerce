using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using bolt5.CustomMappingObject;
using Egate_Ecommerce.Objects.KoreaShipList;
using NPOI.SS.Util;
using NPOI.HSSF.UserModel;
using System.Globalization;
using Egate_Ecommerce.Objects.LuckyShipping;

namespace Egate_Ecommerce.Classes
{
    public static class LuckyShippingHelper
    {
        public const string LUCKY_SHIPPING_FILE = "lucky shipping.xlsx";

        public static IEnumerable<LuckyShipItemViewModel> GetList()
        {
            List<LuckyShipItemViewModel> luckyItems = new List<LuckyShipItemViewModel>();
            string file = GetLuckyFile();
            if (!File.Exists(file))
            {
                System.Windows.MessageBox.Show("Lucky Shipping File not found.");
                return new List<LuckyShipItemViewModel>();
            }
            else
            {
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    IWorkbook workbook = WorkbookFactory.Create(fs, true);
                    //get all sheets
                    for (int s = 0; s < workbook.NumberOfSheets; s++)
                    {
                        ISheet sheet = workbook.GetSheetAt(s);
                        
                        //get headers
                        IRow headerRow = sheet.GetRow(0);
                        int count = headerRow.Cells.Count;
                        string[] columns = new string[count];
                        for (int i = 0; i < count; i++)
                            columns[i] = headerRow.Cells[i].ToString();

                        //prepare mapping
                        var mapping = new MappingObject<LuckyShipItemViewModel>(columns);
                        //get rows
                        for (int i = 1; i < sheet.LastRowNum + 1; i++)
                        {
                            IRow row = sheet.GetRow(i);
                            LuckyShipItemViewModel item = new LuckyShipItemViewModel();
                            mapping.SetValues(ref item, index => row.GetCell(index, MissingCellPolicy.RETURN_BLANK_AS_NULL).GetCellValue());
                            item.RowIndex = i;
                            item.ShipName = sheet.SheetName;
                            luckyItems.Add(item);
                        }
                    }
                }
            }
            return luckyItems;
        }

        public static void SaveEditableValues(LuckyShipItemViewModel luckyItem)
        {
            string file = GetLuckyFile();
            if (!File.Exists(file))
            {
                System.Windows.MessageBox.Show("Lucky Shipping File not found.");
                return;
            }
            FileStream fs = null;
            FileStream fs2 = null;
            try
            {
                fs = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                IWorkbook workbook = WorkbookFactory.Create(fs);
                ISheet sheet = workbook.GetSheet(luckyItem.ShipName);

                //prepare mapping to get column index
                //get headers
                IRow headerRow = sheet.GetRow(0);
                int count = headerRow.Cells.Count;
                string[] columns = new string[count];
                for (int i = 0; i < count; i++)
                    columns[i] = headerRow.Cells[i].ToString();
                //prepare mapping
                var mapping = new MappingObject<LuckyShipItemViewModel>(columns);

                IRow row = sheet.GetRow(luckyItem.RowIndex);
                row.GetCell(mapping.IndexOf(i => i.ETA), MissingCellPolicy.CREATE_NULL_AS_BLANK).SetCellValue(luckyItem.ETA);
                row.GetCell(mapping.IndexOf(i => i.ArrivalMemo), MissingCellPolicy.CREATE_NULL_AS_BLANK).SetCellValue(luckyItem.ArrivalMemo);
                row.GetCell(mapping.IndexOf(i => i.FollowupDate), MissingCellPolicy.CREATE_NULL_AS_BLANK).SetCellValue(luckyItem.FollowupDate?.ToString("M/d/yyyy"));
                //save excel
                fs2 = new FileStream(file, FileMode.Create, FileAccess.ReadWrite);
                workbook.Write(fs2);
            }
            catch (IOException ioEx)
            {
                Logs.WriteExceptionLogs(ioEx);
                System.Windows.MessageBox.Show(ioEx.Message, "", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                if (fs != null) fs.Dispose();
                if (fs2 != null) fs2.Dispose();
            }
        }

        public static string GetLuckyLocation()
        {
            return Directory.GetParent(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).FullName;
        }

        public static string GetLuckyFile()
        {
            //get file name
            string file = FileHelper.GetFile("lucky shipping path.txt", "data");
            if (!File.Exists(file))
                File.WriteAllText(file, LUCKY_SHIPPING_FILE);
            return Path.Combine(GetLuckyLocation(), File.ReadAllText(file));
        }
    }
}
