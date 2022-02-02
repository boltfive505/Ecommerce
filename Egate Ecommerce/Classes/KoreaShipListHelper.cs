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
using bolt5.CloneCopy;

namespace Egate_Ecommerce.Classes
{
    public static class KoreaShipListHelper
    {
        //private const string SHIP_LIST_FILENAME = @"Shipping from korea to Philippine.xlsx";
        //public const string SHIP_LIST_FILENAME = @"Shipping from korea to Philippine - for testing.xlsx";

        private static byte[] itemNormalRgb = null;
        private static byte[] itemProcessedRgb = { 255, 255, 0 };
        private static byte[] itemProcessedOtherPlaceRgb = { 0, 176, 240 };
        private static byte[] subtotalRgb = { 255, 192, 0 };

        public static IEnumerable<KoreaShipItemViewModel> GetList()
        {
            string file = GetShipListFile();
            if (!File.Exists(file))
            {
                System.Windows.MessageBox.Show("Korea Ship File not found.");
                return new List<KoreaShipItemViewModel>();
            }
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IWorkbook workbook = WorkbookFactory.Create(fs, true);
                ISheet sheet = workbook.GetSheet("Ship by Lucky");

                //get headers
                IRow headerRow = sheet.GetRow(4);
                int count = headerRow.Cells.Count;
                string[] columns = new string[count];
                for (int i = 0; i < count; i++)
                    columns[i] = headerRow.Cells[i].ToString();

                //prepare mapping
                var mapping = new MappingObject<KoreaShipItemViewModel>(columns);
                var mappingSubtotal = new MappingObject<KoreaShipSubtotalViewModel>(columns);
                //get rows
                List<KoreaShipItemViewModel> items = new List<KoreaShipItemViewModel>();
                List<KoreaShipItemViewModel> tempItems = new List<KoreaShipItemViewModel>();

                for (int i = 5; i < sheet.LastRowNum + 1; i++)
                {
                    IRow row = sheet.GetRow(i);
                    //check if row is item or subtotal
                    if (KoreaShipListHelper.IsRowSubTotal(row.GetCell(0)))
                    {
                        //row is sub total
                        KoreaShipSubtotalViewModel subtotal = new KoreaShipSubtotalViewModel();
                        mappingSubtotal.SetValues(ref subtotal, index => row.GetCell(index, MissingCellPolicy.RETURN_BLANK_AS_NULL).GetCellValue());
                        subtotal.RowIndex = i; //get row index for reference
                        tempItems.ForEach(item => item.Subtotal = subtotal); //apply subtotal to items info
                        subtotal.IsReceived = tempItems.Any(item => item.Status != KoreaShipStatus.NotReceived); //set subtotal isreceived
                        items.AddRange(tempItems);
                        tempItems.Clear();
                    }
                    else
                    {
                        //row is item info
                        KoreaShipItemViewModel item = new KoreaShipItemViewModel();
                        mapping.SetValues(ref item, index => row.GetCell(index, MissingCellPolicy.RETURN_BLANK_AS_NULL).GetCellValue());
                        item.RowIndex = i; //get row index for reference
                        item.Status = KoreaShipListHelper.GetStatusByColor(row.GetCell(7)); //get ship status, based by cell color - use index 7 of Item
                        tempItems.Add(item);
                    }
                }
                //in-case of items not having subtotals, usually at the end of the row
                if (tempItems.Count > 0)
                {
                    tempItems.ForEach(item => item.Subtotal = new KoreaShipSubtotalViewModel() { RowIndex = -1 });
                    items.AddRange(tempItems);
                    tempItems.Clear();
                }
                return items;
            }
        }

        private static void SaveExistingExcel(string file, IWorkbook workbook)
        {
            using (FileStream fs = new FileStream(file, FileMode.Create, FileAccess.ReadWrite))
            {
                workbook.Write(fs);
            }
        }

        public static void EditETA(KoreaShipSubtotalViewModel subtotal)
        {
            string file = GetShipListFile();
            if (!File.Exists(file))
            {
                System.Windows.MessageBox.Show("Korea Ship File not found.");
                return;
            }
            FileStream fs = null;
            try
            {
                fs = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                IWorkbook workbook = WorkbookFactory.Create(fs);
                ISheet sheet = workbook.GetSheet("Ship by Lucky");
                IRow row = sheet.GetRow(subtotal.RowIndex); //get subtotal row
                SetSubtotalValues(row, subtotal);
                //save excel
                SaveExistingExcel(file, workbook);
            }
            catch (IOException ioEx)
            {
                Logs.WriteExceptionLogs(ioEx);
                System.Windows.MessageBox.Show(ioEx.Message, "", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                if (fs != null) fs.Dispose();
            }
        }

        private static void SetSubtotalValues(IRow row, KoreaShipSubtotalViewModel subtotal)
        {
            //prepare mapping to get column index
            //get headers
            IRow headerRow = row.Sheet.GetRow(4);
            int count = headerRow.Cells.Count;
            string[] columns = new string[count];
            for (int i = 0; i < count; i++)
                columns[i] = headerRow.Cells[i].ToString();
            //prepare mapping
            var mapping = new MappingObject<KoreaShipSubtotalViewModel>(columns);
            //set values
            var index = mapping.IndexOf(i => i.ArrivalDate);
            index = mapping.IndexOf(i => i.ETA);
            index = mapping.IndexOf(i => i.DeliveryReceiptNumber);
            row.GetCell(mapping.IndexOf(i => i.ArrivalDate)).SetCellValue(subtotal.ArrivalDate?.ToString("M/d/yyyy", CultureInfo.InvariantCulture));
            row.GetCell(mapping.IndexOf(i => i.ETA)).SetCellValue(subtotal.ETA);
            row.GetCell(mapping.IndexOf(i => i.DeliveryReceiptNumber)).SetCellValue(subtotal.DeliveryReceiptNumber);
        }

        public static void ReceiveItems(IEnumerable<KoreaShipItemViewModel> list, KoreaShipSubtotalViewModel subtotal)
        {
            string file = GetShipListFile();
            FileStream fs = null;
            try
            {
                fs = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                IWorkbook workbook = WorkbookFactory.Create(fs);
                ISheet sheet = workbook.GetSheet("Ship by Lucky");

                //change row color for items
                SetRowColorStatus(sheet, list);
                //set subtotal delivery receipt number
                SetSubtotalValues(sheet.GetRow(subtotal.RowIndex), subtotal);
                //save excel
                SaveExistingExcel(file, workbook);
            }
            catch (IOException ioEx)
            {
                Logs.WriteExceptionLogs(ioEx);
                System.Windows.MessageBox.Show(ioEx.Message, "", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                if (fs != null) fs.Dispose();
            }
        }

        private static void SetRowColorStatus(ISheet sheet, IEnumerable<KoreaShipItemViewModel> list)
        {
            //get headers count
            IRow headerRow = sheet.GetRow(4);
            int count = headerRow.Cells.Count;

            foreach (var i in list)
            {
                IRow row = sheet.GetRow(i.RowIndex);
                for (int c = 0; c < count; c++)
                {
                    ICell cell = row.GetCell(c);
                    XSSFCellStyle style = (XSSFCellStyle)cell.CellStyle;
                    style.SetFillForegroundColor(new XSSFColor(KoreaShipListHelper.itemProcessedRgb));
                    style.FillPattern = FillPattern.SolidForeground;
                }
                i.Status = KoreaShipStatus.Received;
                i.Subtotal.IsReceived = true;
            }
        }

        public static string GetShipListLocation()
        {
            return Directory.GetParent(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).FullName;
        }

        public static string GetShipListFile()
        {
            //get file name
            string file = FileHelper.GetFile("shipping list path.txt", "data");
            if (!File.Exists(file))
                File.WriteAllText(file, "Shipping from korea to Philippine - for testing.xlsx");
            return Path.Combine(GetShipListLocation(), File.ReadAllText(file));
        }

        public static void SetShipListFile(string file)
        {
            string savefile = FileHelper.GetFile("shipping list path.txt", "data");
            File.WriteAllText(savefile, Path.GetFileName(file));
        }

        private static KoreaShipStatus GetStatusByColor(ICell cell)
        {
            byte[] backgroundRgb = ((XSSFCellStyle)cell.CellStyle)?.FillForegroundXSSFColor?.RGB;
            if (KoreaShipListHelper.CompareColorRgb(backgroundRgb, KoreaShipListHelper.itemProcessedRgb))
                return KoreaShipStatus.Received;
            else if (KoreaShipListHelper.CompareColorRgb(backgroundRgb, KoreaShipListHelper.itemProcessedOtherPlaceRgb))
                return KoreaShipStatus.ReceivedFromOtherLocation;
            else
                return KoreaShipStatus.NotReceived;
        }

        private static bool IsRowSubTotal(ICell cell)
        {
            string firstString = cell?.ToString();
            byte[] backgroundRgb = ((XSSFCellStyle)cell.CellStyle)?.FillForegroundXSSFColor?.RGB;
            //check cell background color or text
            if ((backgroundRgb != null && CompareColorRgb(backgroundRgb, KoreaShipListHelper.subtotalRgb)) || firstString == "Sub Total")
            {
                return true;
            }
            return false;
        }

        private static bool CompareColorRgb(byte[] value1, byte[] value2)
        {
            if (value1 == null && value2 == null) //both is null
                return true;
            else if (value1 == null || value2 == null) //only one of them is null
                return false;
            else //both are not null
                return value1[0] == value2[0] && value1[1] == value2[1] && value1[2] == value2[2];
        }
    }
}
