using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using bolt5.CustomMappingObject;
using bolt5.CloneCopy;
using NPOI.SS.UserModel;

namespace Egate_Ecommerce.Quickbooks
{
    public static class QbPosInventory
    {
        private static List<PosItem> _items = new List<PosItem>();
        private static IEnumerable<string> _departmentList, _departmentListSelection;
        
        public static IEnumerable<PosItem> Items { get { return _items; } }
        public static DateTime? InventoryDate { get; private set; }
        public static string InventoryFile { get; private set; }

        public static IEnumerable<string> DepartmentList
        {
            get
            {
                if (_departmentList == null)
                {
                    _departmentList = _items
                                        .Select(i => i.DepartmentName)
                                        .Distinct()
                                        .OrderBy(x => x);
                }
                return _departmentList;
            }
        }

        public static IEnumerable<string> DepartmentListSelection
        {
            get
            {
                if (_departmentListSelection == null)
                {
                    List<string> lst = new List<string>(DepartmentList);
                    lst.Insert(0, "     ");
                    _departmentListSelection = lst;
                }
                return _departmentListSelection;
            }
        }

        public static void Load()
        {
            _items = new List<PosItem>();
            string dir = @"..\qb"; //Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "qb");
            string file = Directory.GetFiles(dir, "QB POS Inventory Items Export.*", SearchOption.TopDirectoryOnly)
                .Where(f => f.EndsWith(".xls") || f.EndsWith(".xlsx"))
                .First();
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IWorkbook wb = WorkbookFactory.Create(fs, true);
                ISheet sheet = wb.GetSheetAt(0);

                IRow header = sheet.GetRow(0);
                int count = header.Cells.Count;
                string[] cols = new string[count];
                for (int i = 0; i < count; i++)
                    cols[i] = header.Cells[i].ToString();

                var mapping = new MappingObject<PosItem>(cols);
                for (int i = 1; i < sheet.LastRowNum + 1; i++)
                {
                    IRow row = sheet.GetRow(i);
                    PosItem item = new PosItem();
                    mapping.SetValues(ref item, index => row.GetCell(index, MissingCellPolicy.RETURN_BLANK_AS_NULL).GetCellValue());
                    _items.Add(item);
                }
            }
            InventoryFile = file;
            InventoryDate = File.GetLastWriteTime(file);
        }
    }
}
