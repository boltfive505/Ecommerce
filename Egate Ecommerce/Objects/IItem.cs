using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Ecommerce.Objects
{
    public interface IItem
    {
        string ItemNumber { get; set; }
        string ItemName { get; set; }
        string ItemDescription { get; set; }
        string ImagePath { get; set; }
        int? Quantity { get; set; }
        decimal? RegularPrice { get; set; }
        string Memo { get; set; }
    }
}
