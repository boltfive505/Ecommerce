using System;
using bolt5.CustomMappingObject;
using bolt5.CloneCopy;

namespace Egate_Ecommerce.Quickbooks
{
    public class PosSalesItem
    {
        [ColumnMapping("Item #")]
        public string ItemNumber { get; set; }

        [ColumnMapping("Alternate Lookup")]
        [SkipMappingIfMissing]
        public string ALU { get; set; }

        [ColumnMapping("Qty Sold")]
        public double Quantity { get; set; }
    }
}
