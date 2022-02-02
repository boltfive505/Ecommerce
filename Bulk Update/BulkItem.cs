using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using bolt5.CustomMappingObject;

namespace Bulk_Update
{
    public class BulkItem
    {
        public int RowIndex { get; set; } = -1; //excel row reference, starts with 0

        public string ItemName { get; set; }
        public string ItemNumber { get; set; }
        [MappingParseMethod("ParseItemDescription")]
        public string ItemDescription { get; set; }
        public int? Quantity { get; set; }
        public decimal? RegularPrice { get; set; }
        public decimal? SalePrice { get; set; }

        private static object ParseItemDescription(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return Regex.Replace(value, @"\r\n?|\n", "\r\n");
        }
    }
}
