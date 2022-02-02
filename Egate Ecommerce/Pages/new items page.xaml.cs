using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Egate_Ecommerce.Classes;
using Egate_Ecommerce.Objects;
using Egate_Ecommerce.Modals.ECommerce;
using bolt5.ModalWpf;
using System.ComponentModel;

namespace Egate_Ecommerce.Pages
{
    /// <summary>
    /// Interaction logic for new_items_page.xaml
    /// </summary>
    public partial class new_items_page : Page
    {
        public static readonly DependencyProperty NewItemsViewProperty = DependencyProperty.Register(nameof(NewItemsView), typeof(ICollectionView), typeof(new_items_page));
        public ICollectionView NewItemsView
        {
            get { return (ICollectionView)GetValue(NewItemsViewProperty); }
            set { SetValue(NewItemsViewProperty, value); }
        }

        private List<NonInventoryViewModel> newItemsList = new List<NonInventoryViewModel>();

        public new_items_page()
        {
            NewItemsView = new CollectionViewSource() { Source = newItemsList }.View;
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            newItemsList.Clear();
            newItemsList.AddRange(DataCache.GetNonInventoryList());
            NewItemsView.Refresh();
            //set item infos
            Task.Run(async () =>
            {
                var list = await ECommerceHelper.GetItemInfoListAsync();
                newItemsList.ForEach(item => item.ItemInfo = list.FirstOrDefault(i => i.ItemNumber == item.ItemNumber));
            });
        }

        private void AddNewItem_Click(object sender, RoutedEventArgs e)
        {
            bool isEdit = true;
            string title = "Edit New Item";
            NonInventoryViewModel newItem = (sender as FrameworkElement).DataContext as NonInventoryViewModel;
            if (newItem == null)
            {
                isEdit = false;
                title = "Add New Item";
                newItem = new NonInventoryViewModel();
            }
            var modal = new new_item_modal();
            var clone = newItem.DeepClone();
            modal.DataContext = clone;
            if (ModalForm.ShowModal(modal, title, ModalButtons.SaveCancel) == ModalResult.Save)
            {
                clone.DeepCopyTo(newItem);
                //add both non-inventory and item info
                Task.Run(async () =>
                {
                    await DataCache.AddNonInventoryItemAsync(newItem);
                    newItem.ItemInfo.ItemNumber = newItem.ItemNumber;
                    newItem.ItemInfo.UpdatedDate = DateTime.Now;
                    await ECommerceHelper.AddItemInfoAsync(newItem.ItemInfo);
                });
                if (!isEdit)
                    newItemsList.Add(newItem);
                NewItemsView.Refresh();
            }
        }
    }
}
