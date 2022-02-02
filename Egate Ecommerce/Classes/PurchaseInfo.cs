using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Ecommerce.Classes
{
    public class PurchaseInfo
    {
        public string ItemNumber { get; set; }
        public int RequestedQty { get; set; }
        public int PreparedQty { get; set; }
        public int PackedQty { get; set; }
        public int ShippedQty { get; set; }
    }
}
