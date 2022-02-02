using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Egate_Ecommerce.Classes;
using Egate_Ecommerce.Objects.Tutorials;

namespace Egate_Ecommerce.Modals
{
    /// <summary>
    /// Interaction logic for categoryAddEdit_modal.xaml
    /// </summary>
    public partial class tutorialCategoryAddEdit_modal : UserControl
    {
        public tutorialCategoryAddEdit_modal()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            List<TutorialCategoryViewModel> categoryList = TutorialsHelper.GetCategoryListAsync().GetResult().ToList();
            TutorialsHelper.SetCategoryHierarchy(ref categoryList);
            categoryList = categoryList.Where(i => i.IsActive && i.IsActiveHierarchy).OrderBy(i => i.PathName).ToList();
            //set is selectable for parent category -> to prevent self parent stackoverflow
            //get id of this category
            int categoryId = (this.DataContext as TutorialCategoryViewModel).Id;
            categoryList.ForEach(i => i.IsSelectable = i.Id != categoryId);
            categoryCbox.ItemsSource = new ObservableCollection<TutorialCategoryViewModel>(categoryList);
        }
    }
}
