using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Model;
using Egate_Ecommerce.Classes;
using Egate_Ecommerce.Quickbooks;

namespace Egate_Ecommerce.Objects
{
    public class ItemInfoViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public long Id { get; set; }
        public string ItemNumber { get; set; }
        public ItemInfoCategory Category { get; set; } = ItemInfoCategory.Competitor_Price;
        public DateTime UpdatedDate { get; set; }
        public EmployeeViewModel UpdatedByEmployee { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string CompetitorUrl { get; set; }
        public decimal? CompetitorPrice { get; set; }
        public CompetitorLocation CompetitorLocation { get; set; } = CompetitorLocation.None;
        public int CompetitorRatings { get; set; }
        public bool CompetitorHasStocks { get; set; } = true;

        public PosItem PosItem { get; set; }

        public bool IsNew { get { return Id == 0; } }

        [CloneCopyIgnore]
        public RelayCommand OpenURLCommand { get; set; }

        public ItemInfoViewModel()
        {
            OpenURLCommand = new RelayCommand(obj => System.Diagnostics.Process.Start(this.CompetitorUrl));
        }

        public ItemInfoViewModel(item_info entity, employee updatedEmployeeEntity) : this()
        {
            this.Id = entity.Id;
            this.ItemNumber = entity.ItemNumber;
            this.Category = entity.Category.ToEnum<ItemInfoCategory>(ItemInfoCategory.Competitor_Price);
            this.UpdatedDate = entity.UpdatedDate.ToUnixDate();
            this.UpdatedByEmployee = updatedEmployeeEntity == null ? null : new EmployeeViewModel(updatedEmployeeEntity);
            this.ShortDescription = entity.ShortDescription;
            this.LongDescription = entity.LongDescription;
            this.CompetitorUrl = entity.CompetitorUrl;
            this.CompetitorPrice = entity.CompetitorPrice;
            this.CompetitorLocation = entity.CompetitorLocation.ToEnum<CompetitorLocation>();
            this.CompetitorRatings = entity.CompetitorRatings;
            this.CompetitorHasStocks = entity.CompetitorHasStocks.ToBool();
        }
    }
}
