using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Egate_Ecommerce.Quickbooks;

namespace Egate_Ecommerce.Classes
{
    public class LuckyPurchaseItem
    {
        public PosItem PosItem { get; set; }
        public string ItemNumber { get; set; }
        public long Quantity { get; set; }
        public string ShippingLabel { get; set; }
        public string PackingNumber { get; set; }
        public string BoxLabel { get; set; }
        public ShipType ShipType { get; set; }
        public string DeliveryReceiptNumber { get; set; }
    }
}
