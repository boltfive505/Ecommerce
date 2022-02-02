using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using bolt5.CustomMappingObject;

namespace Egate_Ecommerce.Objects.KoreaShipList
{
    public class KoreaShipSubtotalViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [IgnoreMapping]
        public int RowIndex { get; set; } = -1; //excel row index reference

        [IgnoreMapping]
        public int RowNumber { get { return RowIndex == -1 ? -1 : RowIndex + 1; } } //this should match row number in excel

        [ColumnMapping("SKU"), MappingParseMethod("ParsePackingQuantity")]
        public int? PackingQuantity { get; set; }

        [ColumnMapping("Shipping Note")]
        public string ETA { get; set; }

        [ColumnMapping("COST (peso)"),SkipMappingIfConvertionError]
        public DateTime? ArrivalDate { get; set; }

        [ColumnMapping("Specification"), MappingParseMethod("ParseShippingNumber")]
        public string ShippingNumber { get; set; }

        [ColumnMapping("Qty"), MappingParseMethod("ParseShipBy")]
        public string ShipBy { get; set; }

        [ColumnMapping("Status")]
        public string Status { get; set; }

        [ColumnMapping("D.R. No.")]
        public string DeliveryReceiptNumber { get; set; }

        [IgnoreMapping]
        public bool IsReceived { get; set; }

        private static object ParsePackingQuantity(string value)
        {
            Match m = Regex.Match(value, @"^Packing Quantity\s?[:]\s?(?<qty>\d+)", RegexOptions.IgnoreCase);
            if (m.Success)
                return Convert.ToInt32(m.Groups["qty"].Value.Trim());
            else
                return null;
        }

        private static object ParseShippingNumber(string value)
        {
            Match m = Regex.Match(value, @"^Shipping NO[.]\s?(?<shipping>.*)", RegexOptions.IgnoreCase);
            if (m.Success)
                return m.Groups["shipping"].Value.Trim();
            else
                return null;
        }

        private static object ParseShipBy(string value)
        {
            Match m = Regex.Match(value, @"^Ship by\s?[:]\s?(?<shipby>.*)", RegexOptions.IgnoreCase);
            if (m.Success)
                return m.Groups["shipby"].Value.Trim();
            else
                return null;
        }

        public override bool Equals(object obj)
        {
            if (obj is KoreaShipSubtotalViewModel)
            {
                KoreaShipSubtotalViewModel o = obj as KoreaShipSubtotalViewModel;
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
