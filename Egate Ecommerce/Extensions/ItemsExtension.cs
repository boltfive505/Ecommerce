using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Egate_Ecommerce.Extensions
{
    public class ItemsExtension : DependencyObject
    {
        public static readonly DependencyProperty ItemNumberProperty = DependencyProperty.RegisterAttached("ItemNumber", typeof(string), typeof(ItemsExtension));
        
        public static string GetItemNumber(DependencyObject obj)
        {
            return (string)obj.GetValue(ItemNumberProperty);
        }

        public static void SetItemNumber(DependencyObject obj, string value)
        {
            obj.SetValue(ItemNumberProperty, value);
        }
    }
}
