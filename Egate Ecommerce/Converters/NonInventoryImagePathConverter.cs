using System;
using System.Windows.Data;
using System.IO;
using System.Reflection;
using System.Globalization;

namespace Egate_Ecommerce.Converters
{
    public class NonInventoryImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string itemNumber = (string)value;
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "uploads", "non-inventory items", itemNumber + ".jpg");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
