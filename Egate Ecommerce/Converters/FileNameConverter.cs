using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;

namespace Egate_Ecommerce.Converters
{
    public class FileNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string file = (string)value;
            return string.IsNullOrEmpty(file) ? string.Empty : Path.GetFileName(file);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
