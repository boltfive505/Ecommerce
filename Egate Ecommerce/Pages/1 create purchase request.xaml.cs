using System;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.IO;
using Egate_Ecommerce.Quickbooks;
using Egate_Ecommerce.Classes;
using Egate_Ecommerce.Objects;
using Egate_Ecommerce.Modals.ECommerce;
using bolt5.ModalWpf;
using bolt5.CloneCopy;

namespace Egate_Ecommerce.Pages
{
    /// <summary>
    /// Interaction logic for create_purchase_request.xaml
    /// </summary>
    public partial class create_purchase_request : Page
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

            public class SuggestedRange
            { 
                public int From { get; set; }
                public int To { get; set; }
                public string Display { get; set; }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            #region For Items
            public string FilterKeyword { get; set; }
            public string FilterDepartment { get; set; }
            public int SelectedSales { get; set; } = 0;
            public bool FilterNoStocks { get; set; }
            public SuggestedRange FilterSuggested { get; set; }
            public bool ShowGroupItems { get; set; }
            public bool ShowWithoutItemInfoCount { get; set; }

            public ObservableCollection<SuggestedRange> SuggestedList { get; set; }
            #endregion

            #region For _ItemBase Info
            public DateTime? FilterDateFrom { get; set; }
            public DateTime? FilterDateTo { get; set; }
            public long FilterUpdatedBy { get; set; }

            public ObservableCollection<EmployeeViewModel> EmployeeList { get; set; }
            public ObservableCollection<ItemInfoCategoryCountPair> CategoryShowList { get; set; }
            public event Action CategoryShowIsCheckedChanged;
            #endregion

            public bool CanRefresh { get; set; } = true;

            public FilterGroup()
            {
                SuggestedList = new ObservableCollection<SuggestedRange>();
                SuggestedList.Add(new SuggestedRange() { From = -1, To = -1, Display = "ALL" });
                SuggestedList.Add(new SuggestedRange() { From = 100, To = int.MaxValue, Display = "100%" });
                Enumerable.Range(1, 9).Reverse().ToList().ForEach(x =>
                {
                    SuggestedList.Add(new SuggestedRange() { From = x * 10, To = int.MaxValue, Display = (x * 10) + "% up" });
                });
                FilterSuggested = SuggestedList[0];

                CategoryShowList = new ObservableCollection<ItemInfoCategoryCountPair>();
                foreach (var cat in Enum.GetValues(typeof(ItemInfoCategory)).OfType<ItemInfoCategory>().Except(new ItemInfoCategory[] { ItemInfoCategory.All }))
                {
                    CategoryShowList.Add(new ItemInfoCategoryCountPair(cat, () => CategoryShowIsCheckedChanged?.Invoke()));
                }
            }

            public void Reset()
            {
                CanRefresh = false;
                //for item
                FilterKeyword = string.Empty;
                FilterDepartment = string.Empty;
                SelectedSales = 0;
                FilterNoStocks = false;
                FilterSuggested = SuggestedList[0];
                ShowGroupItems = false;
                ShowWithoutItemInfoCount = false;

                foreach (var show in CategoryShowList)
                    show.IsChecked = true;
                FilterDateFrom = null;
                FilterDateTo = null;
                FilterUpdatedBy = 0;
                CanRefresh = true;
            }
        }

        public class TotalGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public DateTime? MobileInventoryLatest { get; set; }
            public DateTime? LuckyShipLatest { get; set; }
            public int PackagesToBeReceivedCount { get; set; }
            public int InventoryCount { get; set; }
        }

        public class SelectionGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public _ItemBase SelectedItem { get; set; }

            public bool CanRefresh { get; set; } = true;

            public void Reset()
            {
                CanRefresh = false;
                SelectedItem = null;
                CanRefresh = true;
            }
        }

        #region Dependency Properties
        public static readonly DependencyProperty FiltersProperty = DependencyProperty.Register(nameof(Filters), typeof(FilterGroup), typeof(create_purchase_request));
        public FilterGroup Filters
        {
            get { return (FilterGroup)GetValue(FiltersProperty); }
            set { SetValue(FiltersProperty, value); }
        }

        public static readonly DependencyProperty FiltersItemInfoProperty = DependencyProperty.Register(nameof(FiltersItemInfo), typeof(FilterGroup), typeof(create_purchase_request));
        public FilterGroup FiltersItemInfo
        {
            get { return (FilterGroup)GetValue(FiltersItemInfoProperty); }
            set { SetValue(FiltersItemInfoProperty, value); }
        }

        public static readonly DependencyProperty TotalsProperty = DependencyProperty.Register(nameof(Totals), typeof(TotalGroup), typeof(create_purchase_request));
        public TotalGroup Totals
        {
            get { return (TotalGroup)GetValue(TotalsProperty); }
            set { SetValue(TotalsProperty, value); }
        }

        public static readonly DependencyProperty SelectionsProperty = DependencyProperty.Register(nameof(Selections), typeof(SelectionGroup), typeof(create_purchase_request));
        public SelectionGroup Selections
        {
            get { return (SelectionGroup)GetValue(SelectionsProperty); }
            set { SetValue(SelectionsProperty, value); }
        }

        public static readonly DependencyProperty ItemListProperty = DependencyProperty.Register(nameof(ItemList), typeof(ICollectionView), typeof(create_purchase_request));
        public ICollectionView ItemList
        {
            get { return (ICollectionView)GetValue(ItemListProperty); }
            set { SetValue(ItemListProperty, value); }
        }

        public static readonly DependencyProperty ItemInfoListProperty = DependencyProperty.Register(nameof(ItemInfoList), typeof(ICollectionView), typeof(create_purchase_request));
        public ICollectionView ItemInfoList
        {
            get { return (ICollectionView)GetValue(ItemInfoListProperty); }
            set { SetValue(ItemInfoListProperty, value); }
        }
        #endregion

        private List<_ItemBase> items = new List<_ItemBase>();
        private List<ItemInfoViewModel> itemInfos = new List<ItemInfoViewModel>();

        public create_purchase_request()
        {
            ItemList = new CollectionViewSource() { Source = items }.View;
            ItemList.Filter = i => Filter(i as _ItemBase);

            ItemInfoList = new CollectionViewSource() { Source = itemInfos }.View;
            ItemInfoList.Filter = i => FilterItemInfo(i as ItemInfoViewModel);
            ItemInfoList.SortDescriptions.Add(new SortDescription("CompetitorPrice", ListSortDirection.Ascending));

            Filters = new FilterGroup();
            Filters.PropertyChanged += Filters_PropertyChanged;

            FiltersItemInfo = new FilterGroup();
            FiltersItemInfo.PropertyChanged += FiltersItemInfo_PropertyChanged;
            FiltersItemInfo.CategoryShowIsCheckedChanged += FiltersItemInfo_CategoryShowIsCheckedChanged;

            Totals = new TotalGroup();

            Selections = new SelectionGroup();
            Selections.PropertyChanged += Selections_PropertyChanged;
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            GetTotals();
            SetDisplayedSalesQuantity();
            GetFiltersItemInfo();
            GetCategoryCount();
            GetInventoryCount();
            GetItemsItemInfoCount();
        }

        private void LoadData()
        {
            items.Clear();
            items.AddRange(_ItemBase.GetItemList1<_ItemBase>());
            ItemList.Refresh();

            itemInfos.Clear();
            itemInfos.AddRange(ECommerceHelper.GetItemInfoListAsync().GetResult());
            ItemInfoList.Refresh();
            //get pos items
            Task.Run(() =>
            {
                var posItemsList = DataCache.GetNonInventoryItemList2().Concat(QbPosInventory.Items);
                itemInfos.ForEach(item => item.PosItem = posItemsList.FirstOrDefault(i => i.ItemNumber == item.ItemNumber));
            });
        }

        private void GetFiltersItemInfo()
        {
            var employeeList = ECommerceHelper.GetEmployeeListAsync().GetResult().ToList();
            employeeList.Insert(0, new EmployeeViewModel() { EmployeeName = "- Updated By -" });
            FiltersItemInfo.EmployeeList = new ObservableCollection<EmployeeViewModel>(employeeList);
        }

        private void GetItemsItemInfoCount()
        {
            Task.Run(async () =>
            {
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    //independent filtering
                    var filteredList = itemInfos.Where(i =>
                    {
                        bool flag = true;

                        //show category, multiple
                        foreach (var show in FiltersItemInfo.CategoryShowList)
                        {
                            if (i.Category == show.Category)
                                flag &= show.IsChecked;
                        }
                        ////updated date range
                        //if (FiltersItemInfo.FilterDateFrom != null && FiltersItemInfo.FilterDateTo != null)
                        //    flag &= i.UpdatedDate.Date >= FiltersItemInfo.FilterDateFrom.Value.Date && i.UpdatedDate.Date <= FiltersItemInfo.FilterDateTo.Value.Date;
                        //updated by
                        if (FiltersItemInfo.FilterUpdatedBy != 0)
                            flag &= (i.UpdatedByEmployee != null && i.UpdatedByEmployee.Id == FiltersItemInfo.FilterUpdatedBy);

                        return flag;
                    });
                    foreach (var item in items)
                    {
                        item.ItemInfoCount = filteredList.Count(i => i.ItemNumber == item.PosItem.ItemNumber);
                    }
                }), System.Windows.Threading.DispatcherPriority.Background);
            });
        }

        private void Filters_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Filters.SelectedSales))
            {
                SetDisplayedSalesQuantity();
            }
            else
            {
                if (Filters.CanRefresh)
                {
                    ItemList.Refresh();
                    GetInventoryCount();
                }
            }
        }

        private void FiltersItemInfo_CategoryShowIsCheckedChanged()
        {
            FiltersItemInfo_PropertyChanged(null, null);
        }

        private void Selections_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FiltersItemInfo_PropertyChanged(null, null);
        }

        private void FiltersItemInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (FiltersItemInfo.CanRefresh & Selections.CanRefresh)
            {
                ItemInfoList.Refresh();
                GetCategoryCount();
                GetItemsItemInfoCount();
            }
        }

        private void SetDisplayedSalesQuantity()
        {
            Task.Run(() =>
            {
                Dispatcher.BeginInvoke(new Action(() => 
                {
                    foreach (var i in items)
                    {
                        if (Filters.SelectedSales == 0)
                            i.DisplayedSalesQty = i.SalesQty1;
                        else if (Filters.SelectedSales == 1)
                            i.DisplayedSalesQty = i.SalesQty2;
                        else if (Filters.SelectedSales == 2)
                            i.DisplayedSalesQty = i.SalesQty3;
                    }
                }), System.Windows.Threading.DispatcherPriority.Background);
            });
        }

        private void GetTotals()
        {
            Task.Run(() =>
            {
                Dispatcher.BeginInvoke(new Action(() =>
                {
                    var shipList = KoreaShipListHelper.GetList();
                    Totals.PackagesToBeReceivedCount = shipList.GroupBy(i => i.Subtotal).Count(g => g.Key.IsReceived == false);

                    var file = LuckyShippingHelper.GetLuckyFile();
                    Totals.LuckyShipLatest = File.Exists(file) ? (DateTime?)File.GetLastWriteTime(file) : null;

                    file = MobileInventoryHelper.GetFile();
                    Totals.MobileInventoryLatest = File.Exists(file) ? (DateTime?)File.GetLastWriteTime(file) : null;
                }), System.Windows.Threading.DispatcherPriority.Background);
            });
        }

        private void GetInventoryCount()
        {
            var filteredList = ItemList.OfType<_ItemBase>();
            Totals.InventoryCount = filteredList.Count();
        }

        private void GetCategoryCount()
        {
            //getting count will be independent from the default filtering
            var filteredList = itemInfos.Where(i =>
            {
                bool flag = true;
                //item selection
                if (Selections.SelectedItem != null)
                    flag &= i.ItemNumber == Selections.SelectedItem.PosItem.ItemNumber;
                //updated date range
                if (FiltersItemInfo.FilterDateFrom != null && FiltersItemInfo.FilterDateTo != null)
                    flag &= i.UpdatedDate.Date >= FiltersItemInfo.FilterDateFrom.Value.Date && i.UpdatedDate.Date <= FiltersItemInfo.FilterDateTo.Value.Date;
                //updated by
                if (FiltersItemInfo.FilterUpdatedBy != 0)
                    flag &= (i.UpdatedByEmployee != null && i.UpdatedByEmployee.Id == FiltersItemInfo.FilterUpdatedBy);
                return flag;
            });
            foreach (var show in FiltersItemInfo.CategoryShowList)
                show.Count = filteredList.Count(i => i.Category == show.Category);
        }

        private bool Filter(_ItemBase i)
        {
            bool flag = true;

            //keyword
            if (!string.IsNullOrEmpty(Filters.FilterKeyword))
            {
                string keyword = Filters.FilterKeyword.Trim();
                flag &= i.PosItem.ItemNumber.StartsWith(keyword, StringComparison.InvariantCultureIgnoreCase)
                    || i.PosItem.ItemName.IndexOf(keyword, 0, StringComparison.InvariantCultureIgnoreCase) >= 0
                    || (i.PosItem.ItemDescription != null && i.PosItem.ItemDescription.IndexOf(keyword, 0, StringComparison.InvariantCultureIgnoreCase) >= 0);
            }
            //department
            if (!string.IsNullOrWhiteSpace(Filters.FilterDepartment))
            {
                flag &= (i.PosItem.DepartmentName == Filters.FilterDepartment);
            }
            //no stocks
            if (Filters.FilterNoStocks)
                flag &= i.PosItem.Quantity == 0;
            //suggested
            if (Filters.FilterSuggested.From != -1)
                flag &= (i.SuggestedQuantityPercent >= Filters.FilterSuggested.From && i.SuggestedQuantityPercent <= Filters.FilterSuggested.To);
            //show group item
            flag &= Filters.ShowGroupItems ? (i.PosItem.ItemType == "Inventory" || i.PosItem.ItemType == "Non-Inventory") : true;
            //show without item info count
            flag &= Filters.ShowWithoutItemInfoCount ? i.ItemInfoCount == 0 : true;

            return flag;
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
            //item selection
            if (Selections.SelectedItem != null)
                flag &= i.ItemNumber == Selections.SelectedItem.PosItem.ItemNumber;
            //updated date range
            if (FiltersItemInfo.FilterDateFrom != null && FiltersItemInfo.FilterDateTo != null)
                flag &= i.UpdatedDate.Date >= FiltersItemInfo.FilterDateFrom.Value.Date && i.UpdatedDate.Date <= FiltersItemInfo.FilterDateTo.Value.Date;
            //updated by
            if (FiltersItemInfo.FilterUpdatedBy != 0)
                flag &= (i.UpdatedByEmployee != null && i.UpdatedByEmployee.Id == FiltersItemInfo.FilterUpdatedBy);
            //department
            if (!string.IsNullOrWhiteSpace(FiltersItemInfo.FilterDepartment))
            {
                flag &= (i.PosItem.DepartmentName == FiltersItemInfo.FilterDepartment);
            }

            return flag;
        }

        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            Filters.Reset();
            ItemList.Refresh();
        }

        private void ResetItemInfoFilters_Click(object sender, RoutedEventArgs e)
        {
            FiltersItemInfo.Reset();
            Selections.Reset();
            ItemInfoList.Refresh();
            GetCategoryCount();
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            var list = ItemList.OfType<_ItemBase>();
            if (list.Count() > 0)
                ExcelHelper.ExportItemList(list);
        }

        private void AddInfo_Click(object sender, RoutedEventArgs e)
        {
            bool isEdit = true;
            string title = "Edit Item Information";
            ModalButtons buttons = ModalButtons.SaveCancelDelete;
            var itemInfo = (sender as FrameworkElement).DataContext as ItemInfoViewModel;
            if (itemInfo == null)
            {
                isEdit = false;
                title = "Add Item Information";
                buttons = ModalButtons.SaveCancel;
                itemInfo = new ItemInfoViewModel();
                itemInfo.PosItem = ((sender as FrameworkElement).DataContext as _ItemBase).PosItem;
                itemInfo.ItemNumber = itemInfo.PosItem.ItemNumber;
            }
            var modal = new item_info_add_modal();
            var clone = itemInfo.DeepClone();
            modal.DataContext = clone;
            ModalResult result = ModalForm.ShowModal(modal, title, buttons);
            if (result == ModalResult.Save)
            {
                clone.DeepCopyTo(itemInfo);
                itemInfo.UpdatedDate = DateTime.Now;
                _ = ECommerceHelper.AddItemInfoAsync(itemInfo);
                if (!isEdit)
                    itemInfos.Add(itemInfo);
                ItemInfoList.Refresh();
            }
            else if (result == ModalResult.Delete)
            {
                _ = ECommerceHelper.DeleteItemInfoAsync(itemInfo);
                itemInfos.Remove(itemInfo);
                ItemInfoList.Refresh();
            }
        }

        private void OpenUrl_Click(object sender, RoutedEventArgs e)
        {
            string url = Convert.ToString((sender as FrameworkElement).Tag);
            System.Diagnostics.Process.Start(url);
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

        private void ImageFullView_Click(object sender, RoutedEventArgs e)
        {
            imageFullPopup.DataContext = (sender as FrameworkElement).DataContext;
            imageFullPopup.IsOpen = true;
        }
    }
}
