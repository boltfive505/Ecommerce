using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Model;

namespace Egate_Ecommerce.Objects
{
    public class ItemKeywordViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public long Id { get; set; }
        public string ItemNumber { get; set; }
        public string Keywords { get; set; }
        public string SuggestedName { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public ItemKeywordViewModel()
        { }

        public ItemKeywordViewModel(item_keyword entity)
        {
            this.Id = entity.Id;
            this.ItemNumber = entity.ItemNumber;
            this.Keywords = entity.Keywords;
            this.SuggestedName = entity.SuggestedName;
            this.UpdatedDate = entity.UpdatedDate.ToUnixDate();
        }
    }
}
