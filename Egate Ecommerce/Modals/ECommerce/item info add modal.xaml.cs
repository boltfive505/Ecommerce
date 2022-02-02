using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Egate_Ecommerce.Objects;
using Egate_Ecommerce.Classes;
using bolt5.ModalWpf;

namespace Egate_Ecommerce.Modals.ECommerce
{
    /// <summary>
    /// Interaction logic for item_info_add_modal.xaml
    /// </summary>
    public partial class item_info_add_modal : UserControl, IModalClosing
    {
        public item_info_add_modal()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var employeeList = ECommerceHelper.GetEmployeeListAsync().GetResult().ToList();
            employeeList.Insert(0, new EmployeeViewModel());
            UpdatedByEmployeeValue.ItemsSource = new ObservableCollection<EmployeeViewModel>(employeeList);
        }

        public void ModalClosing(ModalClosingArgs e)
        {
            if (e.Result == ModalResult.Delete)
            {
                e.Cancel = MessageBox.Show("Do you want to DELETE this info?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes;
            }
        }
    }
}
