using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Egate_Ecommerce.Quickbooks;
using Egate_Ecommerce.Classes;


namespace Egate_Ecommerce.Objects
{
    public class PosItemCompare : INotifyPropertyChanged, IItem
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public PosItem PosItem { get; set; }
        public bool IsExists { get; set; } = true;
        public int? DuplicateCount { get; set; }
        public ItemDetailsViewModel ItemDetails { get; set; }

        [CloneCopyIgnore]
        public string ItemNumber { get => PosItem.ItemNumber; set => throw new NotImplementedException(); }
        [CloneCopyIgnore]
        public string ItemName { get => PosItem.ItemName; set => throw new NotImplementedException(); }
        [CloneCopyIgnore]
        public string ItemDescription { get => PosItem.ItemDescription; set => throw new NotImplementedException(); }
        [CloneCopyIgnore]
        public string ImagePath { get => PosItem.ImagePath; set => throw new NotImplementedException(); }
        [CloneCopyIgnore]
        public int? Quantity { get => PosItem.Quantity; set => throw new NotImplementedException(); }
        [CloneCopyIgnore]
        public decimal? RegularPrice { get => PosItem.RegularPrice; set => throw new NotImplementedException(); }
        [CloneCopyIgnore]
        public string Memo { get => ItemDetails.Memo; set => ItemDetails.Memo = value; }

        [CloneCopyIgnore]
        public int ItemInfoCount { get; set; }

        [CloneCopyIgnore]
        public RelayCommand CopySkuCommand { get; set; }

        public PosItemCompare(PosItem item)
        {
            this.PosItem = item;
            CopySkuCommand = new RelayCommand(obj => Helpers.CopyToClipboard(PosItem.ItemNumber));
            ItemDetails = new ItemDetailsViewModel() { ItemNumber = this.PosItem.ItemNumber };
        }
    }
}
