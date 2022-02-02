using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.HSSF.Util;
using Microsoft.Win32;
using System.IO;
using bolt5.FieldExpressions;

namespace Egate_Ecommerce.Classes
{
    public static class ExcelHelper
    {
        public static void ExportItemList(IEnumerable<_ItemBase> list)
        {
            //prepare fields
            FieldExpressionCollection<_ItemBase> expressionList = new FieldExpressionCollection<_ItemBase>();
            expressionList.Add(i => i.PosItem.ItemNumber, "Item Number");
            expressionList.Add(i => i.PosItem.ItemName, "Item Name");

            //prepare excel
            byte[] excelData = null;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    IWorkbook workbook = new XSSFWorkbook();
                    ISheet sheet = workbook.CreateSheet("items");

                    //create header
                    IRow headerRow = sheet.CreateRow(0);
                    for (int i = 0; i < expressionList.Count; i++)
                        headerRow.CreateCell(i).SetCellValue(expressionList[i].GetFieldName());
                    //create rows
                    int r = 1;
                    foreach (var i in list)
                    {
                        IRow row = sheet.CreateRow(r);
                        for (int a = 0; a < expressionList.Count; a++)
                            row.CreateCell(a).SetCellObjectValue(expressionList[a].GetValue(i));
                        r++;
                    }

                    workbook.Write(ms);
                    excelData = ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                Logs.WriteExceptionLogs(ex);
                System.Windows.MessageBox.Show("An error occured while processing the file.", "", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }
            SaveExcelData(excelData, "Save Item List", string.Format("item list_{0:yyyy-MM-dd}", DateTime.Now));
        }

        private static void SaveExcelData(byte[] excelData, string title, string fileName)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Title = title;
            save.Filter = "Excel File|*.xlsx";
            save.FileName = fileName;
            if (save.ShowDialog() == true)
            {
                try
                {
                    using (var fs = new FileStream(save.FileName, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(excelData, 0, excelData.Length);
                    }
                    FileHelper.Open(save.FileName);
                }
                catch (IOException ioEx)
                {
                    Logs.WriteExceptionLogs(ioEx);
                    System.Windows.MessageBox.Show(ioEx.Message);
                }
            }
        }
    }
}
