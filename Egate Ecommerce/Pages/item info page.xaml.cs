using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Egate_Ecommerce.Quickbooks;
using Egate_Ecommerce.Classes;
using Egate_Ecommerce.Objects;
using Egate_Ecommerce.Modals.ECommerce;
using bolt5.ModalWpf;

namespace Egate_Ecommerce.Pages
{
    /// <summary>
    /// Interaction logic for item_info_page.xaml
    /// </summary>
    public partial class item_info_page : Page
    {
        public class FilterGroup : INotifyPropertyChanged
        {
            public class ItemInfoCategoryCountPair : INotifyPropertyChanged
            {
                public event PropertyChangedEventHandler PropertyChanged;
                public ItemInfoCategory Category { get; set; }
                public int Count { get; set; }
                public bool IsChecked { get; set; } = true;

                public string Display { get { return TypeDescriptor.GetConverter(typeof(ItemInfoCategory)).ConvertToString(Category) + (Count > 0 ? " (" + Count + ")" : ""); } }

                private Action _isCheckedChanged;

                public ItemInfoCategoryCountPair(ItemInfoCategory category, Action isCheckedChanged)
                {
                    this.Category = category;
                    this._isCheckedChanged = isCheckedChanged;
                }

                private void OnIsCheckedChanged()
                {
                    _isCheckedChanged?.Invoke();
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            #region For Item Info
            public string FilterKeyword { get; set; }
            public string FilterDepartment { get; set; }
            public DateTime? FilterDateFrom { get; set; }
            public DateTime? FilterDateTo { get; set; }
            public long FilterUpdatedBy { get; set; }
            public bool ShowMoreExpensivePrice { get; set; }

            public ObservableCollection<EmployeeViewModel> EmployeeList { get; set; }
            public ObservableCollection<ItemInfoCategoryCountPair> CategoryShowList { get; set; }
            //public event Action CategoryShowIsCheckedChanged;
            #endregion

            public bool CanRefresh { get; set; } = true;
            public RelayCommand ResetCommand { get; set; }

            public FilterGroup()
            {
                CategoryShowList = new ObservableCollection<ItemInfoCategoryCountPair>();
                foreach (var cat in Enum.GetValues(typeof(ItemInfoCategory)).OfType<ItemInfoCategory>().Except(new ItemInfoCategory[] { ItemInfoCategory.All }))
                {
                    CategoryShowList.Add(new ItemInfoCategoryCountPair(cat, () => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Format("{0}.{1}", nameof(CategoryShowList), cat.ToString())))));
                }
                ResetCommand = new RelayCommand(obj =>
                {
                    Reset();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
                });
            }

            public void Reset()
            {
                CanRefresh = false;
                //for item
                foreach (var show in CategoryShowList)
                    show.IsChecked = true;
                FilterKeyword = string.Empty;
                FilterDepartment = string.Empty;
                FilterDateFrom = null;
                FilterDateTo = null;
                FilterUpdatedBy = 0;
                ShowMoreExpensivePrice = false;
                CanRefresh = true;
            }
        }

        public class TotalGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public int TotalItemInfoCount { get; set; }
        }

        #region Dependency Properties
        public static readonly DependencyProperty FiltersItemInfoProperty = DependencyProperty.Register(nameof(FiltersItemInfo), typeof(FilterGroup), typeof(item_info_page));
        public FilterGroup FiltersItemInfo
        {
            get { return (FilterGroup)GetValue(FiltersItemInfoProperty); }
            set { SetValue(FiltersItemInfoProperty, value); }
        }

        public static readonly DependencyProperty TotalsItemInfoProperty = DependencyProperty.Register(nameof(TotalsItemInfo), typeof(TotalGroup), typeof(item_info_page));
        public TotalGroup TotalsItemInfo
        {
            get { return (TotalGroup)GetValue(TotalsItemInfoProperty); }
            set { SetValue(TotalsItemInfoProperty, value); }
        }

        public static readonly DependencyProperty ItemInfoListProperty = DependencyProperty.Register(nameof(ItemInfoList), typeof(ICollectionView), typeof(item_info_page));
        public ICollectionView ItemInfoList
        {
            get { return (ICollectionView)GetValue(ItemInfoListProperty); }
            set { SetValue(ItemInfoListProperty, value); }
        }
        #endregion

        private List<ItemInfoViewModel> itemInfos = new List<ItemInfoViewModel>();

        public item_info_page()
        {
            ItemInfoList = new CollectionViewSource() { Source = itemInfos }.View;
            ItemInfoList.Filter = i => FilterItemInfo(i as ItemInfoViewModel);

            FiltersItemInfo = new FilterGroup();
            FiltersItemInfo.PropertyChanged += FiltersItemInfo_PropertyChanged;

            TotalsItemInfo = new TotalGroup();

            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            GetFiltersItemInfo();
            GetTotals();
        }

        private void LoadData()
        {
            itemInfos.Clear();
            itemInfos.AddRange(ECommerceHelper.GetItemInfoListAsync().GetResult());
            ItemInfoList.Refresh();
            //get pos items
            Task.Run(async () =>
            {
                var posItemsList = DataCache.GetNonInventoryItemList2().Concat(QbPosInventory.Items);
                itemInfos.ForEach(item => item.PosItem = posItemsList.FirstOrDefault(i => i.ItemNumber == item.ItemNumber));
                await Dispatcher.BeginInvoke(new Action(() => ItemInfoList.Refresh()), System.Windows.Threading.DispatcherPriority.Background); //refresh again
            });
        }

        private void GetFiltersItemInfo()
        {
            var employeeList = ECommerceHelper.GetEmployeeListAsync().GetResult().ToList();
            employeeList.Insert(0, new EmployeeViewModel() { EmployeeName = "- Updated By -" });
            FiltersItemInfo.EmployeeList = new ObservableCollection<EmployeeViewModel>(employeeList);
        }

        private void GetTotals()
        {
            Task.Run(async () =>
            {
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    var filteredItemInfoList = ItemInfoList.OfType<ItemInfoViewModel>();
                    TotalsItemInfo.TotalItemInfoCount = filteredItemInfoList.Count();
                }), System.Windows.Threading.DispatcherPriority.Background);
            });
        }

        private void FiltersItemInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (FiltersItemInfo.CanRefresh)
            {
                ItemInfoList.Refresh();
                GetTotals();
            }
        }

        private bool FilterItemInfo(ItemInfoViewModel i)
        {
            bool flag = true;

            //show category, multiple
            foreach (var show in FiltersItemInfo.CategoryShowList)
            {
                if (i.Category == show.Category)
                    flag &= show.IsChecked;
            }
            //keyword
            if (!string.IsNullOrEmpty(FiltersItemInfo.FilterKeyword))
            {
                string keyword = FiltersItemInfo.FilterKeyword.Trim();
                flag &= (i.PosItem?.ItemNumber ?? string.Empty).StartsWith(keyword, StringComparison.InvariantCultureIgnoreCase)
                    || (i.PosItem?.ItemName ?? string.Empty).IndexOf(keyword, 0, StringComparison.InvariantCultureIgnoreCase) >= 0
                    || (i.PosItem?.ItemDescription ?? string.Empty).IndexOf(keyword, 0, StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            //department
            if (!string.IsNullOrWhiteSpace(FiltersItemInfo.FilterDepartment))
                flag &= (i.PosItem.DepartmentName == FiltersItemInfo.FilterDepartment);
            //updated date range
            if (FiltersItemInfo.FilterDateFrom != null && FiltersItemInfo.FilterDateTo != null)
                flag &= i.UpdatedDate.Date >= FiltersItemInfo.FilterDateFrom.Value.Date && i.UpdatedDate.Date <= FiltersItemInfo.FilterDateTo.Value.Date;
            //updated by
            if (FiltersItemInfo.FilterUpdatedBy != 0)
                flag &= (i.UpdatedByEmployee != null && i.UpdatedByEmployee.Id == FiltersItemInfo.FilterUpdatedBy);
            //show more expensive price
            if (FiltersItemInfo.ShowMoreExpensivePrice)
                flag &= (i.CompetitorPrice != null && i.PosItem.RegularPrice > i.CompetitorPrice);

            return flag;
        }

        private void AddInfo_Click(object sender, RoutedEventArgs e)
        {
            var itemInfo = (sender as FrameworkElement).DataContext as ItemInfoViewModel;
            var modal = new item_info_add_modal();
            var clone = itemInfo.DeepClone();
            modal.DataContext = clone;
            ModalResult result = ModalForm.ShowModal(modal, "Edit Item Information", ModalButtons.SaveCancelDelete);
            if (result == ModalResult.Save)
            {
                clone.DeepCopyTo(itemInfo);
                itemInfo.UpdatedDate = DateTime.Now;
                _ = ECommerceHelper.AddItemInfoAsync(itemInfo);
                ItemInfoList.Refresh();
            }
            else if (result == ModalResult.Delete)
            {
                _ = ECommerceHelper.DeleteItemInfoAsync(itemInfo);
                itemInfos.Remove(itemInfo);
                ItemInfoList.Refresh();
            }
        }

        private void ApplyDateRange_btn_Click(object sender, RoutedEventArgs e)
        {
            FiltersItemInfo.CanRefresh = false;
            DateRangeType? dateRange = (sender as FrameworkElement).Tag as DateRangeType?;
            if (dateRange != null)
            {
                DateTime start, end;
                DateRangeHelper.GetDateRange(dateRange.Value, out start, out end);
                FiltersItemInfo.FilterDateFrom = start;
                FiltersItemInfo.FilterDateTo = end;
            }
            FiltersItemInfo.CanRefresh = true;
            ItemInfoList.Refresh();
        }

        private void ShowImageFullView_Click(object sender, RoutedEventArgs e)
        {
            imageFullPopup.DataContext = (sender as FrameworkElement).DataContext;
            imageFullPopup.IsOpen = true;
        }

        private void SearchSku_Click(object sender, RoutedEventArgs e)
        {
            FilterGroup filters = (sender as FrameworkElement).Tag as FilterGroup;
            string itemNumber = Extensions.ItemsExtension.GetItemNumber(sender as FrameworkElement);
            filters.FilterKeyword = itemNumber;
        }
    }
}
