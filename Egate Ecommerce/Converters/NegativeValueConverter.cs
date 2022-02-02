using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Egate_Ecommerce.Converters
{
    public class NegativeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0;

            Type t = value.GetType();
            if (t == typeof(int))
                return (int)value * -1;
            else if (t == typeof(long))
                return (long)value * -1;
            else if (t == typeof(double))
                return (double)value * -1;
            else if (t == typeof(decimal))
                return (decimal)value * -1;
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
