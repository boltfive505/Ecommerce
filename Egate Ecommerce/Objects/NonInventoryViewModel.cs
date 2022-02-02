using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using purchase_request.Model;
using System.IO;
using System.Text.RegularExpressions;

namespace Egate_Ecommerce.Objects
{
    public class NonInventoryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public long Id { get; set; }
        public string ItemNumber { get { return (Id * -1).ToString(); } }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public string ImagePath { get; set; }

        public ItemInfoViewModel ItemInfo { get; set; } = new ItemInfoViewModel();

        public NonInventoryViewModel()
        { }

        public NonInventoryViewModel(noninventory_items entity)
        {
            this.Id = entity.Id;
            this.ItemName = entity.ItemName;
            this.ItemDescription = entity.ItemDescription;
            GetImagePath();
        }

        public void GetImagePath()
        {
            this.ImagePath = Quickbooks.PosItem.GetNonInventoryImagePath(ItemNumber);
            //string directory = Path.Combine(".", "uploads", "non-inventory items");
            //this.ImagePath = Directory.GetFiles(directory, ItemNumber + ".*", SearchOption.TopDirectoryOnly)
            //    .Where(f => Regex.IsMatch(f, @"(.*?)\.(png|jpg|jpeg)$", RegexOptions.IgnoreCase))
            //    .FirstOrDefault();
        }
    }
}
