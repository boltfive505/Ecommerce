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
using Egate_Ecommerce.Objects.LuckyDelivery;

namespace Egate_Ecommerce.Classes
{
    public static class LuckyDeliveryHelper
    {
        public const string LUCKY_DELIVERY_FILE = "lucky delivery.xlsx";

        public static IEnumerable<LuckyDeliveryReceiptViewModel> GetList()
        {
            List<LuckyDeliveryReceiptViewModel> receiptItems = new List<LuckyDeliveryReceiptViewModel>();
            string file = GetLuckyFile();
            if (!File.Exists(file))
            {
                System.Windows.MessageBox.Show("Lucky Delivery File not found.");
                return new List<LuckyDeliveryReceiptViewModel>();
            }
            else
            {
                using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    IWorkbook workbook = WorkbookFactory.Create(fs, true);
                    ISheet sheet = workbook.GetSheetAt(0);

                    //get headers
                    IRow headerRow = sheet.GetRow(0);
                    int count = headerRow.Cells.Count;
                    string[] columns = new string[count];
                    for (int i = 0; i < count; i++)
                        columns[i] = headerRow.Cells[i].ToString();

                    //prepare mapping
                    var mapping = new MappingObject<LuckyDeliveryReceiptViewModel>(columns);
                    //get rows
                    for (int i = 1; i < sheet.LastRowNum + 1; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        LuckyDeliveryReceiptViewModel item = new LuckyDeliveryReceiptViewModel();
                        mapping.SetValues(ref item, index => row.GetCell(index, MissingCellPolicy.RETURN_BLANK_AS_NULL).GetCellValue());
                        item.RowIndex = i;
                        receiptItems.Add(item);
                    }
                }
            }
            return receiptItems;
        }

        public static string GetLuckyLocation()
        {
            return Directory.GetParent(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).FullName;
        }

        public static string GetLuckyFile()
        {
            //get file name
            string file = FileHelper.GetFile("lucky delivery path.txt", "data");
            if (!File.Exists(file))
                File.WriteAllText(file, LUCKY_DELIVERY_FILE);
            return Path.Combine(GetLuckyLocation(), File.ReadAllText(file));
        }

        public static bool AddDeliveryReceipt(LuckyDeliveryReceiptViewModel receiptVm)
        {
            string file = GetLuckyFile();
            if (!File.Exists(file))
            {
                System.Windows.MessageBox.Show("Lucky Delivery File not found.");
                return false;
            }
            FileStream fs = null;
            FileStream fs2 = null;
            try
            {
                fs = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                IWorkbook workbook = WorkbookFactory.Create(fs);
                ISheet sheet = workbook.GetSheetAt(0);

                //prepare mapping to get column index
                //get headers
                IRow headerRow = sheet.GetRow(0);
                int count = headerRow.Cells.Count;
                string[] columns = new string[count];
                for (int i = 0; i < count; i++)
                    columns[i] = headerRow.Cells[i].ToString();
                //prepare mapping
                var mapping = new MappingObject<LuckyDeliveryReceiptViewModel>(columns);
                IRow row = null;
                if (receiptVm.RowIndex == -1)
                {
                    //add new value
                    row = sheet.CreateRow(sheet.LastRowNum + 1); //create new row
                    mapping.GetValuesWithType(receiptVm, (i, type, obj) => row.CreateCell(i, type).SetCellObjectValue(obj));
                }
                else
                {
                    //edit existing value
                    row = sheet.GetRow(receiptVm.RowIndex); //get row
                    mapping.GetValues(receiptVm, (i, obj) => row.GetCell(i).SetCellObjectValue(obj));
                }
                //save excel
                fs2 = new FileStream(file, FileMode.Create, FileAccess.ReadWrite);
                workbook.Write(fs2);
                receiptVm.RowIndex = row.RowNum;
            }
            catch (IOException ioEx)
            {
                Logs.WriteExceptionLogs(ioEx);
                System.Windows.MessageBox.Show(ioEx.Message, "", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return false;
            }
            finally
            {
                if (fs != null) fs.Dispose();
                if (fs2 != null) fs2.Dispose();
            }
            return true;
        }
    }
}
