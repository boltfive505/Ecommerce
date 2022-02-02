using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Model;
using Egate_Ecommerce.Classes;
using Egate_Ecommerce.Quickbooks;
using bolt5.CloneCopy;

namespace Egate_Ecommerce.Objects
{
    public class ItemDetailsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public long Id { get; set; }
        public string ItemNumber { get; set; }
        public string Memo { get; set; }
        public DateTime? MemoUpdatedDate { get; set; }
        public bool ForUpdate { get; set; }

        public bool IsNew { get { return Id == 0; } }

        [CloneCopyIgnore]
        public RelayCommand ClearMemoCommand { get; set; }
        
        public ItemDetailsViewModel()
        {
            ClearMemoCommand = new RelayCommand(obj => Memo = string.Empty);
        }

        public ItemDetailsViewModel(item_details entity) : this()
        {
            this.Id = entity.Id;
            this.ItemNumber = entity.ItemNumber;
            this.Memo = entity.Memo;
            this.MemoUpdatedDate = entity.MemoUpdatedDate.ToUnixDate();
            this.ForUpdate = entity.ForUpdate.ToBool();
        }
    }
}
