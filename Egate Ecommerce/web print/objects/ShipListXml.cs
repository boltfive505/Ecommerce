using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Ecommerce.web_print.objects
{
    public class ShipListXml
    {
        public List<ShipItem> ItemList { get; set; }
        public ShipSubtotal Subtotal { get; set; }

        public class ShipItem
        {
            public string ItemNumber { get; set; }
            public string ItemName { get; set; }
            public string ItemDescription { get; set; }
            public long Quantity { get; set; }
            public string ImagePath { get; set; }
        }

        public class ShipSubtotal
        {
            public string ShipNumber { get; set; }
            public int Quantity { get; set; }
            public string ShipBy { get; set; }
            public string Status { get; set; }
        }
    }
}
