using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bolt5.CustomMappingObject;
using bolt5.CloneCopy;

namespace Egate_Ecommerce.Objects.LuckyShipping
{
    public class LuckyShipItemViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [IgnoreMapping]
        public int RowIndex { get; set; } = -1; //excel row index reference

        [IgnoreMapping]
        public int RowNumber { get { return RowIndex == -1 ? -1 : RowIndex + 1; } } //this should match row number in excel

        [IgnoreMapping]
        public string ShipName { get; set; } //this value will get from sheet name

        [ColumnMapping("Wearing in Korea")]
        [ColumnMapping("Received Date by Lucky", ColumnMappingComparison.StartsWith)]
        public DateTime WearingInKorea { get; set; }

        [ColumnMapping("Location")]
        [ColumnMapping("Current Location", ColumnMappingComparison.StartsWith)]
        public string Location { get; set; }

        [ColumnMapping("Processing date", ColumnMappingComparison.StartsWith)]
        public DateTime ProcessingDate { get; set; }

        [ColumnMapping("Item Name", ColumnMappingComparison.StartsWith)]
        public string ItemName { get; set; }

        [ColumnMapping("Quantity", ColumnMappingComparison.StartsWith)]
        public int Quantity { get; set; }

        [ColumnMapping("Weight", ColumnMappingComparison.StartsWith)]
        public decimal Weight { get; set; }

        [ColumnMapping("CBM", ColumnMappingComparison.StartsWith)]
        public string CBM { get; set; }

        [ColumnMapping("Memo 1", ColumnMappingComparison.StartsWith)]
        public string Memo1 { get; set; }

        [ColumnMapping("Memo 2", ColumnMappingComparison.StartsWith)]
        public string Memo2 { get; set; }

        [ColumnMapping("Shipment result")]
        [ColumnMapping("Shipped No#", ColumnMappingComparison.StartsWith)]
        public string ShipmentResult { get; set; }

        [ColumnMapping("Completion Date", ColumnMappingComparison.StartsWith)]
        public string CompletionDate { get; set; }

        //fields for editing
        [ColumnMapping("ETA", ColumnMappingComparison.StartsWith)]
        public string ETA { get; set; }

        [ColumnMapping("Arrival Memo", ColumnMappingComparison.StartsWith)]
        public string ArrivalMemo { get; set; }

        [ColumnMapping("F.Updated", ColumnMappingComparison.StartsWith)]
        public DateTime? FollowupDate { get; set; }
    }
}
