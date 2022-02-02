using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Egate_Ecommerce.Classes;
using Egate_Ecommerce.Objects;
using Egate_Ecommerce.Quickbooks;

namespace Egate_Ecommerce.Templates
{
    /// <summary>
    /// Interaction logic for ItemInfoListPopup.xaml
    /// </summary>
    public partial class ItemInfoListPopup : UserControl
    {
        public static readonly DependencyProperty ItemInfoListProperty = DependencyProperty.Register(nameof(ItemInfoList), typeof(ICollectionView), typeof(ItemInfoListPopup));
        public ICollectionView ItemInfoList
        {
            get { return (ICollectionView)GetValue(ItemInfoListProperty); }
            set { SetValue(ItemInfoListProperty, value); }
        }

        private List<ItemInfoViewModel> itemInfos = new List<ItemInfoViewModel>();

        public ItemInfoListPopup()
        {
            ItemInfoList = new CollectionViewSource() { Source = itemInfos }.View;
            InitializeComponent();
        }

        public void LoadItems(string itemNumber)
        {
            itemInfos.Clear();
            itemInfos.AddRange(ECommerceHelper.GetItemInfoListByItemNumberAsync(itemNumber).GetResult());
            ItemInfoList.Refresh();
            //get pos items
            Task.Run(() =>
            {
                var posItemsList = DataCache.GetNonInventoryItemList2().Concat(QbPosInventory.Items);
                itemInfos.ForEach(item => item.PosItem = posItemsList.FirstOrDefault(i => i.ItemNumber == item.ItemNumber));
            });
        }
    }
}
