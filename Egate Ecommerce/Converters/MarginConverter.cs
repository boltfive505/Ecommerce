using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Egate_Ecommerce.Converters
{
    public class MarginConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            
            return new Thickness(ToDouble(values[0]),
                            ToDouble(values[1]),
                            ToDouble(values[2]),
                            ToDouble(values[3]));
        }

        private static double ToDouble(object value)
        {
            Type t = value.GetType();
            switch (Type.GetTypeCode(t))
            {
                case TypeCode.Double:
                case TypeCode.Int32:
                    return System.Convert.ToDouble(value);
                default:
                    return 0;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
