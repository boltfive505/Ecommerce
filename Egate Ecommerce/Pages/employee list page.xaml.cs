using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Egate_Ecommerce.Classes;
using Egate_Ecommerce.Objects;
using Egate_Ecommerce.Modals.ECommerce;
using bolt5.ModalWpf;

namespace Egate_Ecommerce.Pages
{
    /// <summary>
    /// Interaction logic for employee_list_page.xaml
    /// </summary>
    public partial class employee_list_page : Page
    {
        public static readonly DependencyProperty EmployeeViewProperty = DependencyProperty.Register(nameof(EmployeeView), typeof(ICollectionView), typeof(employee_list_page));
        public ICollectionView EmployeeView
        {
            get { return (ICollectionView)GetValue(EmployeeViewProperty); }
            set { SetValue(EmployeeViewProperty, value); }
        }

        private List<EmployeeViewModel> employeeList = new List<EmployeeViewModel>();

        public employee_list_page()
        {
            EmployeeView = new CollectionViewSource() { Source = employeeList }.View;
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            employeeList.Clear();
            employeeList.AddRange(ECommerceHelper.GetEmployeeListAsync().GetResult());
            EmployeeView.Refresh();
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            bool isEdit = true;
            string title = "Edit Employee";
            EmployeeViewModel employee = (sender as FrameworkElement).DataContext as EmployeeViewModel;
            if (employee == null)
            {
                isEdit = false;
                title = "Add Employee";
                employee = new EmployeeViewModel();
            }
            var modal = new employee_add_modal();
            var clone = employee.DeepClone();
            modal.DataContext = clone;
            if (ModalForm.ShowModal(modal, title, ModalButtons.SaveCancel) == ModalResult.Save)
            {
                clone.DeepCopyTo(employee);
                _ = ECommerceHelper.AddEmployeeAsync(employee);
                if (!isEdit)
                    employeeList.Add(employee);
                EmployeeView.Refresh();
            }
        }
    }
}
