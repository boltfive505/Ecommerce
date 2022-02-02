using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Egate_Ecommerce.Objects.KoreaShipList;
using Egate_Ecommerce.Pages;
using Egate_Ecommerce.Reports.Objects;
using Egate_Ecommerce.Objects;

namespace Egate_Ecommerce.Reports
{
    public static class ReportHelper
    {       
        public static void PrintBulkItems<TItem>(IEnumerable<TItem> items) where TItem : class, IItem
        {
            IEnumerable<Item> items2 = items.Select(i => Item.Parse(i));
            report_view_frm.ShowReport("bulk items report", "ItemDataset", items2);
        }
    }
}
