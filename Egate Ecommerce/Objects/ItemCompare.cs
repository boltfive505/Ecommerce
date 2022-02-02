using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Egate_Ecommerce.Classes;
using Egate_Ecommerce.Quickbooks;
using Bulk_Update;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections.ObjectModel;

namespace Egate_Ecommerce.Objects
{
    public class ItemCompare : INotifyPropertyChanged, IItem
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public BulkItem BulkItem { get; set; }

        public string ItemName { get => BulkItem.ItemName; set => BulkItem.ItemName = value; }
        public string ItemNumber { get => BulkItem.ItemNumber; set => BulkItem.ItemNumber = value; }
        public string ItemDescription { get => BulkItem.ItemDescription; set => BulkItem.ItemDescription = value; }
        public int? Quantity { get => BulkItem.Quantity; set => BulkItem.Quantity = value; }
        public decimal? RegularPrice { get => BulkItem.RegularPrice; set => BulkItem.RegularPrice = value; }
        public decimal? SalePrice { get => BulkItem.SalePrice; set => BulkItem.SalePrice = value; }
        public string Memo { get => string.Empty; set => throw new NotImplementedException(); }

        public string ActualItemNumber { get; set; }
        public string ImagePath { get; set; }
        public string ItemDescriptionPlainText { get; private set; }

        [CloneCopyIgnore]
        public int? NewQuantity { get; set; }
        [CloneCopyIgnore]
        public decimal? NewPrice { get; set; }
        [CloneCopyIgnore]
        public bool IsExists { get; set; } = true;
        [CloneCopyIgnore]
        public RelayCommand CopySkuCommand { get; set; }

        public List<ItemPicturesItem> ItemPicturesList { get; set; }

        public ItemCompare()
        { }

        public ItemCompare(BulkItem item) : this()
        {
            this.BulkItem = item;
            OnBulkItemChanged();
            CopySkuCommand = new RelayCommand(obj => Helpers.CopyToClipboard(ItemNumber));

            ItemPicturesList = new List<ItemPicturesItem>();
            ItemPicturesList.Add(new ItemPicturesItem(this.ActualItemNumber, null, (num, suff) => PosItem.GetInventoryImagePath(num), false)); //add main image
            //add the other images
            ItemPicturesList.Add(new ItemPicturesItem(this.ActualItemNumber, "2"));
            ItemPicturesList.Add(new ItemPicturesItem(this.ActualItemNumber, "3"));
            ItemPicturesList.Add(new ItemPicturesItem(this.ActualItemNumber, "4"));
            ItemPicturesList.Add(new ItemPicturesItem(this.ActualItemNumber, "5"));
        }

        private void OnBulkItemChanged()
        {
            //get actual item number
            ActualItemNumber = ItemNumber;
            if (!string.IsNullOrEmpty(ItemNumber))
            {
                Match m = Regex.Match(ItemNumber, @"(?<item_number>(^(\d{6}|\d{5}))|\d{6})");
                if (m.Success)
                    ActualItemNumber = m.Groups["item_number"].Value;
            }
            
            //get image path
            ImagePath = PosItem.GetInventoryImagePath(ActualItemNumber);
        }

        private void OnItemDescriptionChanged()
        {
            ItemDescriptionPlainText = Converters.HtmlToPlainTextConverter.Convert(ItemDescription);
        }
    }
}
