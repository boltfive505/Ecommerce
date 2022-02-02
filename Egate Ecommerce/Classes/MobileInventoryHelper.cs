using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Egate_Ecommerce.Objects.MobileInventory;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using bolt5.CustomMappingObject;

namespace Egate_Ecommerce.Classes
{
    public static class MobileInventoryHelper
    {
        public static IEnumerable<MobileInventoryItem> GetList()
        {
            var list = new List<MobileInventoryItem>();
            string file = GetFile();
            if (!File.Exists(file))
            {
                Logs.WriteExceptionLogs(new Exception("Mobile Inventory file not found"));
                //System.Windows.MessageBox.Show("Korea Ship File not found.");
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
                    var mapping = new MappingObject<MobileInventoryItem>(columns);
                    //get rows
                    for (int i = 1; i < sheet.LastRowNum - 1; i++)
                    {
                        IRow row = sheet.GetRow(i);
                        MobileInventoryItem item = new MobileInventoryItem();
                        mapping.SetValues(ref item, index => row.GetCell(index, MissingCellPolicy.RETURN_BLANK_AS_NULL).GetCellValue());
                        list.Add(item);
                    }
                }
            }
            return list;
        }

        public static string GetLocation()
        {
            return @"..\qb";
        }

        public static string GetFile()
        {
            string[] files = Directory.GetFiles(GetLocation(), "Physical Inventory Sheet*.xls", SearchOption.TopDirectoryOnly);
            if (files.Length == 0) return string.Empty;
            return files[0];
        }
    }
}
