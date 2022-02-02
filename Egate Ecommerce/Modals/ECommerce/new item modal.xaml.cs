using Egate_Ecommerce.Objects;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Egate_Ecommerce.Modals.ECommerce
{
    /// <summary>
    /// Interaction logic for new_item_modal.xaml
    /// </summary>
    public partial class new_item_modal : UserControl
    {
        public new_item_modal()
        {
            InitializeComponent();
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Multiselect = false;
            open.Filter = "Image Files|*.png;*.jpg;*.jpeg";
            if (open.ShowDialog() == true)
            {
                (this.DataContext as NonInventoryViewModel).ImagePath = open.FileName;
            }
        }
    }
}
