using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Bulk_Update
{
    public static class BulkItemNumberComparisons
    {
        public static Func<string, string, bool> Default => (sku1, sku2) => 
        {
            Match m = Regex.Match(sku2, @"(?<item_number>(^(\d{6}|\d{5}))|\d{6})");
            if (m.Success)
                return sku1 == m.Groups["item_number"].Value;
            else
                return false;
        };
    }
}
