using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Bulk_Update
{
    public static class BulkItemPropertiesToUpdate
    {
        public static Expression<Func<BulkItem, object>>[] ItemNameOnly =
        {
            e => e.ItemName
        };

        public static Expression<Func<BulkItem, object>>[] QuantityOnly =
        {
            e => e.Quantity
        };

        public static Expression<Func<BulkItem, object>>[] QuantityAndPrice =
        {
            e => e.Quantity,
            e => e.RegularPrice,
            e => e.SalePrice
        };

        public static Expression<Func<BulkItem, object>>[] All =
        {
            e => e.ItemName,
            e => e.ItemNumber,
            e => e.ItemDescription,
            e => e.Quantity,
            e => e.RegularPrice,
            e => e.SalePrice
        };
    }
}
