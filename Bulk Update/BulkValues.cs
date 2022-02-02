using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bolt5.CustomMappingObject.LinkedMapping;

namespace Bulk_Update
{
    public static class BulkValues
    {
        public const string EXCEL_FILTER = "Excel File|*.xls;*.xlsx";
        public const string CSV_FILTER = "CSV File|*.csv";

        internal static readonly LinkMap[] _lazadaMap =
        {
            new LinkMap("*Product Name", "ItemName"),
            new LinkMap("SellerSKU", "ItemNumber"),
            new LinkMap("*Quantity", "Quantity"),
            new LinkMap("*Price", "RegularPrice"),
            new LinkMap("SpecialPrice", "SalePrice")
        };

        internal static readonly LinkMap[] _shopeeMap =
        {
            new LinkMap("Product Name", "ItemName"),
            new LinkMap("Parent SKU", "ItemNumber"),
            new LinkMap("Stock", "Quantity"),
            new LinkMap("Price", "RegularPrice"),
        };

        //using main export method
        internal static readonly LinkMap[] _wcMap =
        {
            new LinkMap("Name", "ItemName"),
            new LinkMap("SKU", "ItemNumber"),
            new LinkMap("Stock", "Quantity"),
            new LinkMap("Regular price", "RegularPrice"),
            new LinkMap("Sale price", "SalePrice"),
        };

        //using different export method
        internal static readonly LinkMap[] _wcMap2 =
        {
            new LinkMap("Title", "ItemName"),
            new LinkMap("Sku", "ItemNumber"),
            new LinkMap("Content", "ItemDescription"),
            new LinkMap("Stock", "Quantity"),
            new LinkMap("Regular Price", "RegularPrice"),
            new LinkMap("Sale Price", "SalePrice"),
        };

        internal static readonly LinkMap[] _posMap =
        {
            new LinkMap("Item Name", "ItemName"),
            new LinkMap("Item #", "ItemNumber"),
            new LinkMap("On-hand Qty", "Quantity"),
            new LinkMap("Regular price", "RegularPrice"),
        };
    }
}
