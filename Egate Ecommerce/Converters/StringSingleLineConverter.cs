using System;
using System.Windows.Data;

namespace Egate_Ecommerce.Converters
{
    public class StringSingleLineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var val = value as string ?? string.Empty;
            return string.Join(" ", val.Split(new string[] { "\r", "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
            //return val.Replace(Environment.NewLine, string.Empty);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("Method not implemented");
        }
    }
}
