using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bolt5.CustomMappingObject;
using bolt5.CloneCopy;

namespace Egate_Ecommerce.Objects.MobileInventory
{
    public class MobileInventoryItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [ColumnMapping("item_number")]
        public string ItemNumber { get; set; }

        [ColumnMapping("item_name")]
        public string ItemName { get; set; }

        [ColumnMapping("current_qty")]
        public int CurrentQuantity { get; set; }

        [ColumnMapping("new_qty")]
        [ColumnMapping("qty")]
        public int? NewQuantity { get; set; }

        public int? DifferenceQuantity
        {
            get
            {
                if (NewQuantity == null) return null;
                return NewQuantity.Value - CurrentQuantity;
            }
        }

        public int DifferenceType
        {
            get
            {
                if (DifferenceQuantity == null) return -1;
                else if (DifferenceQuantity > 0) return 1;
                else if (DifferenceQuantity < 0) return 2;
                else return 0;
            }
        }

        [ColumnMapping("expiry_date")]
        [MappingParseMethod("ParseExpiryDate")]
        public DateTime? ExpiryDate { get; set; }

        [ColumnMapping("input_datetime")]
        [SkipMappingIfMissing]
        public DateTime? InputDate { get; set; }

        [ColumnMapping("rack_name")]
        public string RackName { get; set; }

        [ColumnMapping("location_name")]
        public string LocationName { get; set; }

        private static object ParseExpiryDate(string value)
        {
            // 3/8/2023 12:00:00 AM
            DateTime date;
            bool parse = DateTime.TryParseExact(value, "M/d/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
            if (!parse)
                return null;
            else
                return date;
        }
    }
}
