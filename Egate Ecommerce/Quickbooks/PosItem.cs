using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using bolt5.CustomMappingObject;

namespace Egate_Ecommerce.Quickbooks
{
    public class PosItem
    {
        public string ImagePath
        {
            get
            {
                if (IsNonInventory)
                    return PosItem.GetNonInventoryImagePath(ItemNumber);
                else
                    return PosItem.GetInventoryImagePath(ItemNumber);
            }
        }

        public bool IsNonInventory { get { return DepartmentCode == "000"; } }

        [ColumnMapping("Item Number")]
        public string ItemNumber { get; set; }

        [ColumnMapping("Item Name")]
        public string ItemName { get; set; }

        [ColumnMapping("Item Description")]
        public string ItemDescription { get; set; }

        [ColumnMapping("Department Name")]
        public string DepartmentName { get; set; }

        [ColumnMapping("Department Code")]
        public string DepartmentCode { get; set; }

        [ColumnMapping("Alternate Lookup")]
        public string ALU { get; set; }

        [ColumnMapping("Qty 1")]
        public int Quantity { get; set; }

        [ColumnMapping("Average Unit Cost")]
        public decimal UnitCost { get; set; }

        [ColumnMapping("Regular Price")]
        public decimal RegularPrice { get; set; }

        [ColumnMapping("Order Cost")]
        public decimal OrderCost { get; set; }

        [ColumnMapping("Item Type")]
        public string ItemType { get; set; }

        [ColumnMapping("Company Reorder Point")]
        public decimal? ReorderPoint { get; set; }

        [ColumnMapping("Custom Field 2")]
        public string PartNumber { get; set; }

        [IgnoreMapping]
        public string KoreanName { get; set; }

        public static string GetInventoryImagePath(string itemNumber)
        {
            return Path.GetFullPath(Path.Combine("..", "qb", "Item Pictures", itemNumber + "_Def.jpg"));
        }

        public static string GetNonInventoryImagePath(string itemNumber)
        {
            string dir = @".\uploads\non-inventory items";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            return Directory.GetFiles(dir, itemNumber + ".*", SearchOption.TopDirectoryOnly)
                .Where(f => Regex.IsMatch(f, @"(.*?)\.(png|jpg|jpeg)$", RegexOptions.IgnoreCase))
                .FirstOrDefault();
        }
    }
}
