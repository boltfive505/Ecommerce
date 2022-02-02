using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Egate_Ecommerce.Objects;

namespace Egate_Ecommerce.Reports.Objects
{
    [Serializable]
    public class Item : IItem
    {
        private string _itemNumber;
        private string _imagePath;
        private int? _quantity;

        public string ItemNumber
        {
            get { return _itemNumber; }
            set
            {
                _itemNumber = value;
                ItemNumberBarcode = _itemNumber.GetBarcodeData(width: 230);
            }
        }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public string ImagePath
        {
            get { return _imagePath; }
            set
            {
                _imagePath = value;
                ImageAbsolutePath = new Uri(_imagePath).AbsoluteUri;
            }
        }
        public int? Quantity
        { 
            get { return _quantity; }
            set
            {
                _quantity = value;
                QuantityBarcode = (_quantity ?? 0).ToString().GetBarcodeData(width: 230, format: ZXing.BarcodeFormat.CODE_39);
            }
        }
        public decimal? RegularPrice { get; set; }
        public string Memo { get; set; }

        public byte[] ItemNumberBarcode { get; private set; }
        public byte[] QuantityBarcode { get; private set; }
        public byte[] Image { get; private set; }
        public string ImageAbsolutePath { get; private set; }

        public static Item Parse(IItem item)
        {
            Item newItem = new Item();
            newItem.ItemNumber = item.ItemNumber;
            newItem.ItemName = item.ItemName;
            newItem.ImagePath = item.ImagePath;
            newItem.Quantity = item.Quantity;
            newItem.RegularPrice = item.RegularPrice;
            newItem.Memo = item.Memo;
            return newItem;
        }
    }
}
