using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using bolt5.CustomMappingObject;

namespace Egate_Ecommerce.Objects.KoreaShipList
{
    public class KoreaShipItemViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [IgnoreMapping]
        public int RowIndex { get; set; } = -1; //excel row index reference

        [IgnoreMapping]
        public int RowNumber { get { return RowIndex == -1 ? -1 : RowIndex + 1; } } //this should match row number in excel

        [IgnoreMapping]
        public KoreaShipStatus Status { get; set; }

        [IgnoreMapping]
        public KoreaShipSubtotalViewModel Subtotal { get; set; }

        [ColumnMapping("Box NO")]
        public string BoxNumber { get; set; }

        [ColumnMapping("box label")]
        public string BoxLabel { get; set; }

        [ColumnMapping("Ship Date"), SkipMappingIfConvertionError]
        public DateTime? ShipDate { get; set; }

        [ColumnMapping("Lucky Date")]
        public DateTime? LuckyDate { get; set; }

        [ColumnMapping("PR-SHIP Packing NO.")]
        public string PackingNumber { get; set; }

        [ColumnMapping("SKU")]
        public string Sku { get; set; }

        [ColumnMapping("Item")]
        public string ItemName { get; set; }

        [ColumnMapping("Specification")]
        public string Specification { get; set; }
        
        [ColumnMapping("Qty")]
        public int? Quantity { get; set; }

        [ColumnMapping("COST (peso)")]
        public decimal? Cost { get; set; }
        
        [ColumnMapping("Weight")]
        public string Weight { get; set; }

        [ColumnMapping("Shipping Note")]
        public string Note { get; set; }

        public string ImagePath
        {
            get
            {
                return Quickbooks.PosItem.GetInventoryImagePath(Sku);
                //return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "qb", "Item Pictures", Sku + "_Def.jpg");
            }
        }

        public DateTime? EstimatedDate
        {
            get { return LuckyDate?.AddDays(20); }
        }

        public bool IsBeyondEstimatedDate
        { 
            get { return EstimatedDate == null ? false : DateTime.Now.Date >= EstimatedDate.Value.Date; }
        }

        public override bool Equals(object obj)
        {
            if (obj is KoreaShipItemViewModel)
            {
                KoreaShipItemViewModel o = obj as KoreaShipItemViewModel;
                return o.RowIndex == this.RowIndex;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.RowIndex.GetHashCode();
        }
    }
}
