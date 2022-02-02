using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Ecommerce.Classes
{
    public class NonInventoryItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public string Url { get; set; }
        public string ImagePath { get; set; }
    }
}
