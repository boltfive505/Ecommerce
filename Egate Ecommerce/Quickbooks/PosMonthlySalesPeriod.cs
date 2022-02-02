using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using NPOI.SS.UserModel;
using System.Text.RegularExpressions;
using bolt5.CustomMappingObject;
using bolt5.CloneCopy;

namespace Egate_Ecommerce.Quickbooks
{
    public class PosMonthlySalesPeriod
    {
        public DateTime ReportDate { get; private set; }
        public DateTime FromSalesDate { get; private set; }
        public DateTime ToSalesDate { get; private set; }
        public List<PosSalesItem> Items { get { return _items; } }

        private List<PosSalesItem> _items;

        public PosMonthlySalesPeriod(string dir, string fileName)
        {
            _items = new List<PosSalesItem>();
            string file = Directory.GetFiles(dir, fileName + ".*", SearchOption.TopDirectoryOnly)
                .Where(f => f.EndsWith(".xls") || f.EndsWith(".xlsx"))
                .First();
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IWorkbook wb = WorkbookFactory.Create(fs, true);
                ISheet sheet = wb.GetSheetAt(0);

                //report date & time
                DateTime datePart = (DateTime)sheet.GetRow(0).GetCell(1).GetCellValue();
                DateTime timePart = (DateTime)sheet.GetRow(1).GetCell(1).GetCellValue();
                this.ReportDate = datePart.Date.Add(timePart.TimeOfDay);
                //sales date range
                string salesDateStr = sheet.GetRow(3).GetCell(11, MissingCellPolicy.RETURN_BLANK_AS_NULL).StringCellValue;
                Match m = Regex.Match(salesDateStr, @"^Date: (?<from_date>\d{1,2}\/\d{1,2}\/\d{4}) (?<from_time>\d{1,2}:\d{1,2}:\d{1,2} (AM|PM)) to (?<to_date>\d{1,2}\/\d{1,2}\/\d{4}) (?<to_time>\d{1,2}:\d{1,2}:\d{1,2} (AM|PM))");
                if (m.Success)
                {
                    this.FromSalesDate = DateTime.Parse(m.Groups["from_date"].Value);
                    this.ToSalesDate = DateTime.Parse(m.Groups["to_date"].Value).Date.Add(timePart.TimeOfDay);
                }
                else
                    throw new ArgumentException("Monthly Sales Report: Cannot find report date range");

                //get sales items
                IRow header = sheet.GetRow(6);
                int count = header.Cells.Count;
                string[] cols = new string[count];
                for (int i = 0; i < count; i++)
                    cols[i] = header.Cells[i].ToString();

                var mapping = new MappingObject<PosSalesItem>(cols);
                for (int i = 7; i < sheet.LastRowNum - 1; i++)
                {
                    IRow row = sheet.GetRow(i);
                    PosSalesItem item = new PosSalesItem();
                    mapping.SetValues(ref item, index => row.GetCell(index, MissingCellPolicy.RETURN_BLANK_AS_NULL).GetCellValue());
                    _items.Add(item);
                }
            }
        }
    }
}
