using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using bolt5.CustomControls;
using Egate_Ecommerce.Classes;
using Egate_Ecommerce.Objects.Tutorials;

namespace Egate_Ecommerce.Modals
{
    /// <summary>
    /// Interaction logic for tutorialVideoAddEdit_modal.xaml
    /// </summary>
    public partial class tutorialVideoAddEdit_modal : UserControl
    {
        public tutorialVideoAddEdit_modal()
        {
            InitializeComponent();

            var categoryTask = TutorialsHelper.GetCategoryListAsync();
            var employeeTask = TutorialsHelper.GetEmployeeList();
            Task.WaitAll(categoryTask, employeeTask);

            //category
            List<TutorialCategoryViewModel> categoryList = categoryTask.Result.ToList();
            TutorialsHelper.SetCategoryHierarchy(ref categoryList);
            ICollectionView categoryView = new CollectionViewSource() { Source = categoryList.Where(i => i.IsActive && i.IsActiveHierarchy) }.View;
            categoryView.SortDescriptions.Add(new SortDescription("PathName", ListSortDirection.Ascending));
            categoryCbox.ItemsSource = categoryView;
            categoryView.Refresh();
            //employee
            List<TutorialEmployeeViewModel> employeeList = employeeTask.Result.ToList();
            employeeList.Add(new TutorialEmployeeViewModel());
            ICollectionView employeeView = new CollectionViewSource() { Source = employeeList }.View;
            employeeView.SortDescriptions.Add(new SortDescription("EmployeeName", ListSortDirection.Ascending));
            AssignedToValue.ItemsSource = employeeView;
            employeeView.Refresh();
        }
    }
}
