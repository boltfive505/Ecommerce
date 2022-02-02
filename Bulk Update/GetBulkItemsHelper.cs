using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NPOI.SS.UserModel;
using CsvHelper;
using CsvHelper.Configuration;
using bolt5.CustomMappingObject.LinkedMapping;

namespace Bulk_Update
{
    public static class GetBulkBulkItemsHelper
    {
        public static IEnumerable<BulkItem> GetLazadaBulkItems(string excelFile)
        {
            List<BulkItem> items = new List<BulkItem>();
            using (FileStream fs = new FileStream(excelFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IWorkbook workbook = WorkbookFactory.Create(fs, true);
                ISheet sheet = workbook.GetSheet("template");

                //get headers
                IRow headerRow = sheet.GetRow(0);
                string[] columns = headerRow.Cells.Select(i => i.ToString()).ToArray();

                //prepare mapping
                var mapping = new LinkedMappingObject<BulkItem>(columns, BulkValues._lazadaMap);

                //get rows
                for (int i = 1; i < sheet.LastRowNum + 1; i++)
                {
                    IRow row = sheet.GetRow(i);
                    BulkItem item = new BulkItem();
                    mapping.SetValues(ref item, index => row.GetCell(index, MissingCellPolicy.RETURN_BLANK_AS_NULL).GetCellValue());
                    item.RowIndex = row.RowNum;
                    items.Add(item);
                }
            }
            return items;
        }

        public static IEnumerable<BulkItem> GetShopeeBulkItems(string excelFile)
        {
            List<BulkItem> items = new List<BulkItem>();
            using (FileStream fs = new FileStream(excelFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IWorkbook workbook = WorkbookFactory.Create(fs, true);
                ISheet sheet = workbook.GetSheetAt(0);

                //get headers
                IRow headerRow = sheet.GetRow(3);
                string[] columns = headerRow.Cells.Select(i => i.ToString()).ToArray();

                //prepare mapping
                var mapping = new LinkedMappingObject<BulkItem>(columns, BulkValues._shopeeMap);

                //get rows
                for (int i = 4; i < sheet.LastRowNum + 1; i++)
                {
                    IRow row = sheet.GetRow(i);
                    BulkItem item = new BulkItem();
                    mapping.SetValues(ref item, index => row.GetCell(index, MissingCellPolicy.RETURN_BLANK_AS_NULL).GetCellValue());
                    item.RowIndex = row.RowNum;
                    items.Add(item);
                }
            }
            return items;
        }

        public static IEnumerable<BulkItem> GetWoocommerceBulkItems(string csvFile)
        {
            List<BulkItem> items = new List<BulkItem>();
            using (FileStream fs = new FileStream(csvFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                StreamReader reader = new StreamReader(fs);
                CsvConfiguration config = new CsvConfiguration();
                config.Delimiter = ",";
                CsvReader csvReader = new CsvReader(reader, config);

                //get headers
                csvReader.ReadHeader();

                //prepare mapping
                var mapping = new LinkedMappingObject<BulkItem>(csvReader.FieldHeaders, BulkValues._wcMap2);

                //get rows
                int r = 1;
                while (csvReader.Read())
                {
                    BulkItem item = new BulkItem();
                    try
                    {
                        mapping.SetValues(ref item, index => csvReader.CurrentRecord[index]);
                    }
                    catch (Exception ex)
                    {
                        item.ItemNumber = "#ERROR#";
                        Logs.WriteException(ex);
                    }
                    item.RowIndex = r;
                    items.Add(item);
                    r++;
                }
            }
            return items;
        }

        public static IEnumerable<BulkItem> GetQbPosBulkItems(string excelFile)
        {
            List<BulkItem> items = new List<BulkItem>();
            using (FileStream fs = new FileStream(excelFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IWorkbook workbook = WorkbookFactory.Create(fs, true);
                ISheet sheet = workbook.GetSheetAt(0);

                //get headers
                IRow headerRow = sheet.GetRow(5);
                string[] columns = headerRow.Cells.Select(i => i.ToString()).ToArray();

                //prepare mapping
                var mapping = new LinkedMappingObject<BulkItem>(columns, BulkValues._posMap);

                //get rows
                for (int i = 6; i < sheet.LastRowNum - 1; i++)
                {
                    IRow row = sheet.GetRow(i);
                    BulkItem item = new BulkItem();
                    mapping.SetValues(ref item, index => row.GetCell(index, MissingCellPolicy.RETURN_BLANK_AS_NULL).GetCellValue());
                    item.RowIndex = row.RowNum;
                    items.Add(item);
                }
            }
            return items;
        }
    }
}
