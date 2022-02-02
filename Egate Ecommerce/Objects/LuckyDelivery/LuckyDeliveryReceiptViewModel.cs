using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bolt5.CustomMappingObject;
using bolt5.CloneCopy;

namespace Egate_Ecommerce.Objects.LuckyDelivery
{
    public class LuckyDeliveryReceiptViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        [IgnoreMapping]
        public int RowIndex { get; set; } = -1; //excel row index reference
        [IgnoreMapping]
        public int RowNumber { get { return RowIndex == -1 ? -1 : RowIndex + 1; } }

        [ColumnMapping("DR No.")]
        public string ReceiptNumber { get; set; }
        [ColumnMapping("Date")]
        public DateTime? Date { get; set; }
        [ColumnMapping("PO No.")]
        public string PurchaseOrderNumber { get; set; }
        [ColumnMapping("Qty")]
        public int? Quantity { get; set; }
        [ColumnMapping("Weight")]
        public decimal? Weight { get; set; }
        [ColumnMapping("Articles")]
        public string Articles { get; set; }
        [ColumnMapping("Amount")]
        public decimal? Amount { get; set; }
    }
}
