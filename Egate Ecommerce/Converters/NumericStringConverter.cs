using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Egate_Ecommerce.Converters
{
    [ValueConversion(typeof(decimal), typeof(string))]
    public class NumericStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal returnedValue;
            if (decimal.TryParse((string)value, out returnedValue))
            {
                return returnedValue;
            }
            return null;
        }
    }
}
