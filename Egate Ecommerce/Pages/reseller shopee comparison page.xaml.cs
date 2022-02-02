using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.IO;
using Microsoft.Win32;
using Egate_Ecommerce.Classes;
using Egate_Ecommerce.Quickbooks;
using Egate_Ecommerce.Objects;
using Egate_Ecommerce.Modals;
using bolt5.ModalWpf;
using Bulk_Update;
using Egate_Ecommerce.Modals.ECommerce;

namespace Egate_Ecommerce.Pages
{
    /// <summary>
    /// Interaction logic for reseller_shopee_comparison_page.xaml
    /// </summary>
    public partial class reseller_shopee_comparison_page : Page
    {
        public class FilterGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public string FilterKeyword { get; set; }
            public string FilterDepartment { get; set; }
            public int FilterStockStatus { get; set; } //0 = all, 1 = with stock, 2 = without stock
            public bool ShowNotExists { get; set; }
            public bool ShowProductToBeUpdated { get; set; }
            public bool ShowMemo { get; set; }
            public bool ShowDetailToBeUpdated { get; set; }
            public bool CanRefresh { get; set; } = true;

            public RelayCommand ResetCommand { get; set; }

            public FilterGroup()
            {
                ResetCommand = new RelayCommand(obj =>
                {
                    Reset();
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(""));
                });
            }

            public void Reset()
            {
                CanRefresh = false;
                FilterKeyword = null;
                FilterDepartment = null;
                FilterStockStatus = 0;
                ShowNotExists = false;
                ShowProductToBeUpdated = false;
                ShowMemo = false;
                ShowDetailToBeUpdated = false;
                CanRefresh = true;
            }
        }

        public class TotalGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public int TotalCount { get; set; }
            public int NotExistsCount { get; set; }
            public DateTime? FileDate { get; set; }
        }

        #region Dependency Properties
        public static readonly DependencyProperty ShopeeItemsViewProperty = DependencyProperty.Register(nameof(ShopeeItemsView), typeof(ICollectionView), typeof(reseller_shopee_comparison_page));
        public ICollectionView ShopeeItemsView
        { 
            get { return (ICollectionView)GetValue(ShopeeItemsViewProperty); }
            set { SetValue(ShopeeItemsViewProperty, value); }
        }

        public static readonly DependencyProperty ResellerItemsViewProperty = DependencyProperty.Register(nameof(ResellerItemsView), typeof(ICollectionView), typeof(reseller_shopee_comparison_page));
        public ICollectionView ResellerItemsView
        {
            get { return (ICollectionView)GetValue(ResellerItemsViewProperty); }
            set { SetValue(ResellerItemsViewProperty, value); }
        }

        public static readonly DependencyProperty FiltersShopeeProperty = DependencyProperty.Register(nameof(FiltersShopee), typeof(FilterGroup), typeof(reseller_shopee_comparison_page), new PropertyMetadata(new FilterGroup()));
        public FilterGroup FiltersShopee
        {
            get { return (FilterGroup)GetValue(FiltersShopeeProperty); }
            set { SetValue(FiltersShopeeProperty, value); }
        }

        public static readonly DependencyProperty FiltersResellerProperty = DependencyProperty.Register(nameof(FiltersReseller), typeof(FilterGroup), typeof(reseller_shopee_comparison_page), new PropertyMetadata(new FilterGroup()));
        public FilterGroup FiltersReseller
        {
            get { return (FilterGroup)GetValue(FiltersResellerProperty); }
            set { SetValue(FiltersResellerProperty, value); }
        }

        public static readonly DependencyProperty TotalsShopeeProperty = DependencyProperty.Register(nameof(TotalsShopee), typeof(TotalGroup), typeof(reseller_shopee_comparison_page), new PropertyMetadata(new TotalGroup()));
        public TotalGroup TotalsShopee
        {
            get { return (TotalGroup)GetValue(TotalsShopeeProperty); }
            set { SetValue(TotalsShopeeProperty, value); }
        }

        public static readonly DependencyProperty TotalsResellerProperty = DependencyProperty.Register(nameof(TotalsReseller), typeof(TotalGroup), typeof(reseller_shopee_comparison_page), new PropertyMetadata(new TotalGroup()));
        public TotalGroup TotalsReseller
        {
            get { return (TotalGroup)GetValue(TotalsResellerProperty); }
            set { SetValue(TotalsResellerProperty, value); }
        }

        public static readonly DependencyProperty ResellerFileProperty = DependencyProperty.Register(nameof(ResellerFile), typeof(string), typeof(reseller_shopee_comparison_page));
        public string ResellerFile
        {
            get { return (string)GetValue(ResellerFileProperty); }
            set { SetValue(ResellerFileProperty, value); }
        }

        public static readonly DependencyProperty ShopeeFileProperty = DependencyProperty.Register(nameof(ShopeeFile), typeof(string), typeof(reseller_shopee_comparison_page));
        public string ShopeeFile
        {
            get { return (string)GetValue(ShopeeFileProperty); }
            set { SetValue(ShopeeFileProperty, value); }
        }
        #endregion

        private const string RESELLER_FILE_DIRECTORY = @"export files\reseller";
        private const string SHOPEE_FILE_DIRECTORY = @"export files\shopee";
        private List<ItemCompare> shopeeItemsList = new List<ItemCompare>();
        private List<ItemCompare> resellerItemsList = new List<ItemCompare>();

        public reseller_shopee_comparison_page()
        {
            ShopeeItemsView = new CollectionViewSource() { Source = shopeeItemsList }.View;
            ShopeeItemsView.SortDescriptions.Add(new SortDescription("IsExists", ListSortDirection.Ascending));
            ShopeeItemsView.SortDescriptions.Add(new SortDescription("ItemNumber", ListSortDirection.Ascending));
            ShopeeItemsView.Filter = x => DoFilterShopeeItem(x as ItemCompare);

            ResellerItemsView = new CollectionViewSource() { Source = resellerItemsList }.View;
            ResellerItemsView.SortDescriptions.Add(new SortDescription("IsExists", ListSortDirection.Ascending));
            ResellerItemsView.SortDescriptions.Add(new SortDescription("ItemNumber", ListSortDirection.Ascending));
            ResellerItemsView.Filter = x => DoFilterResellerItem(x as ItemCompare);

            FiltersShopee.PropertyChanged += FiltersShopee_PropertyChanged;
            FiltersReseller.PropertyChanged += FiltersReseller_PropertyChanged;

            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadShopeeData();
            LoadResellerData();
            DoComparison();
            GetTotals();
            GetDuplicateCount();
            //UpdateQuantityFromShopeeToReseller();
        }

        private void LoadShopeeData()
        {
            this.ShopeeFile = FileHelper.GetFiles("*.xlsx", reseller_shopee_comparison_page.SHOPEE_FILE_DIRECTORY).FirstOrDefault();
            if (File.Exists(this.ShopeeFile))
            {
                shopeeItemsList.Clear();
                shopeeItemsList.AddRange(GetBulkBulkItemsHelper.GetShopeeBulkItems(this.ShopeeFile).Select(i => new ItemCompare(i)));
                ShopeeItemsView.Refresh();
                TotalsShopee.FileDate = File.GetLastWriteTime(this.ShopeeFile);
            }
            else
                TotalsShopee.FileDate = null;
        }

        private void LoadResellerData()
        {
            this.ResellerFile = FileHelper.GetFiles("*.csv", reseller_shopee_comparison_page.RESELLER_FILE_DIRECTORY).FirstOrDefault();
            if (File.Exists(this.ResellerFile))
            {
                resellerItemsList.Clear();
                resellerItemsList.AddRange(GetBulkBulkItemsHelper.GetWoocommerceBulkItems(this.ResellerFile).Select(i => new ItemCompare(i)));
                ResellerItemsView.Refresh();
                TotalsReseller.FileDate = File.GetLastWriteTime(this.ResellerFile);
            }
            else
                TotalsReseller.FileDate = null;
        }

        private void DoComparison()
        {
            if (resellerItemsList.Count == 0)
            {
                //default to exists all -> if no reseller items list
                foreach (var shopee in shopeeItemsList)
                    shopee.IsExists = true;
            }
            else
            {
                //compare to shopee items
                foreach (var shopee in shopeeItemsList)
                    shopee.IsExists = resellerItemsList.Any(i => i.ItemNumber == shopee.ItemNumber);
            }
            ShopeeItemsView.Refresh();

            if (shopeeItemsList.Count == 0)
            {
                //default to exists all -> if no shopee items list
                foreach (var reseller in resellerItemsList)
                    reseller.IsExists = true;
            }
            else
            {
                //compare to reseller items
                foreach (var reseller in resellerItemsList)
                    reseller.IsExists = shopeeItemsList.Any(i => i.ItemNumber == reseller.ItemNumber);
            }
            ResellerItemsView.Refresh();
        }

        private void GetDuplicateCount()
        {
            //foreach (var shopee in shopeeItemsList)
            //{
            //    Task.Run(() =>
            //    {
            //        int? duplicate = resellerItemsList.Count(i => i.ActualItemNumber == shopee.ItemNumber);
            //        //if (duplicate <= 1) duplicate = null;
            //        shopee.DuplicateCount = duplicate;
            //    });
            //}
        }

        private void GetTotals()
        {
            //shopee items -> but has independent filtering
            var filteredShopeeItems = shopeeItemsList.Where(i =>
            {
                bool flag = true;
                //keyword
                if (!string.IsNullOrEmpty(FiltersShopee.FilterKeyword))
                {
                    string keyword = FiltersShopee.FilterKeyword.Trim();
                    flag &= (i.ItemNumber.StartsWith(keyword, StringComparison.InvariantCultureIgnoreCase) ||
                            i.ActualItemNumber.StartsWith(keyword, StringComparison.InvariantCultureIgnoreCase) ||
                            i.ItemName.IndexOf(keyword, 0, StringComparison.InvariantCultureIgnoreCase) >= 0);
                }
                //show to be updated
                if (FiltersShopee.ShowProductToBeUpdated)
                    flag &= i.NewQuantity != null;
                return flag;
            });
            TotalsShopee.TotalCount = filteredShopeeItems.Count();
            TotalsShopee.NotExistsCount = filteredShopeeItems.Count(i => !i.IsExists);

            //reseller items -> but has independent filtering
            var filteredResellerItems = resellerItemsList.Where(i =>
            {
                bool flag = true;
                //keyword
                if (!string.IsNullOrEmpty(FiltersReseller.FilterKeyword))
                {
                    string keyword = FiltersReseller.FilterKeyword.Trim();
                    flag &= (i.ItemNumber.StartsWith(keyword, StringComparison.InvariantCultureIgnoreCase) ||
                            i.ActualItemNumber.StartsWith(keyword, StringComparison.InvariantCultureIgnoreCase) ||
                            i.ItemName.IndexOf(keyword, 0, StringComparison.InvariantCultureIgnoreCase) >= 0);
                }
                //show to be updated
                if (FiltersReseller.ShowProductToBeUpdated)
                    flag &= i.NewQuantity != null;
                return flag;
            });
            TotalsReseller.TotalCount = filteredResellerItems.Count();
            TotalsReseller.NotExistsCount = filteredResellerItems.Count(i => !i.IsExists);
        }

        private void FiltersReseller_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (FiltersReseller.CanRefresh)
            {
                ResellerItemsView.Refresh();
                GetTotals();
            }
        }

        private void FiltersShopee_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (FiltersShopee.CanRefresh)
            {
                ShopeeItemsView.Refresh();
                GetTotals();
            }
        }

        private bool DoFilterShopeeItem(ItemCompare i)
        {
            bool flag = true;
            //keyword
            if (!string.IsNullOrEmpty(FiltersShopee.FilterKeyword))
            {
                string keyword = FiltersShopee.FilterKeyword.Trim();
                flag &= (i.ItemNumber.StartsWith(keyword, StringComparison.InvariantCultureIgnoreCase) ||
                        i.ActualItemNumber.StartsWith(keyword, StringComparison.InvariantCultureIgnoreCase) ||
                        i.ItemName.IndexOf(keyword, 0, StringComparison.InvariantCultureIgnoreCase) >= 0);
            }
            //show not exists
            if (FiltersShopee.ShowNotExists)
                flag &= !i.IsExists;
            //show to be updated
            if (FiltersShopee.ShowProductToBeUpdated)
                flag &= (i.NewQuantity != null || i.NewPrice != null);
            return flag;
        }

        private bool DoFilterResellerItem(ItemCompare i)
        {
            bool flag = true;
            //keyword
            if (!string.IsNullOrEmpty(FiltersReseller.FilterKeyword))
            {
                string keyword = FiltersReseller.FilterKeyword.Trim();
                flag &= (i.ItemNumber.StartsWith(keyword, StringComparison.InvariantCultureIgnoreCase) ||
                        i.ActualItemNumber.StartsWith(keyword, StringComparison.InvariantCultureIgnoreCase) ||
                        i.ItemName.IndexOf(keyword, 0, StringComparison.InvariantCultureIgnoreCase) >= 0);
            }
            //show not exists
            if (FiltersReseller.ShowNotExists)
                flag &= !i.IsExists;
            //show to be updated
            if (FiltersReseller.ShowProductToBeUpdated)
                flag &= (i.NewQuantity != null || i.NewPrice != null);
            return flag;
        }

        private void ShowImageFullView_Click(object sender, RoutedEventArgs e)
        {
            imageFullPopup.DataContext = (sender as FrameworkElement).DataContext;
            imageFullPopup.IsOpen = true;
        }

        private void ImportResellerFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Multiselect = false;
            open.Filter = BulkValues.CSV_FILTER;
            open.Title = "Import Reseller File";
            if (open.ShowDialog() == true)
            {
                //clear all files
                string[] files = FileHelper.GetFiles("*.*", reseller_shopee_comparison_page.RESELLER_FILE_DIRECTORY, SearchOption.TopDirectoryOnly);
                foreach (var f in files)
                    File.Delete(f);
                //save new excel file
                FileHelper.Upload(open.FileName, reseller_shopee_comparison_page.RESELLER_FILE_DIRECTORY);

                LoadResellerData();
                DoComparison();
                GetTotals();
                GetDuplicateCount();
                //UpdateQuantityFromShopeeToReseller();
            }
        }

        private void ImportShopeeFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Multiselect = false;
            open.Filter = BulkValues.EXCEL_FILTER;
            open.Title = "Import Shopee File";
            if (open.ShowDialog() == true)
            {
                //clear all files
                string[] files = FileHelper.GetFiles("*.*", reseller_shopee_comparison_page.SHOPEE_FILE_DIRECTORY, SearchOption.TopDirectoryOnly);
                foreach (var f in files)
                    File.Delete(f);
                //save new excel file
                FileHelper.Upload(open.FileName, reseller_shopee_comparison_page.SHOPEE_FILE_DIRECTORY);

                LoadShopeeData();
                DoComparison();
                GetTotals();
                GetDuplicateCount();
                //UpdateQuantityFromShopeeToShopee();
            }
        }

        private void ExportResellerFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //copy reseller file to temp folder
                if (File.Exists(ResellerFile))
                {
                    string copyFile = CopyToTemp(ResellerFile);
                    //resellerItemsList.ForEach(i => i.Quantity = (i.NewQuantity ?? i.Quantity));
                    IEnumerable<BulkItem> bulkItems = resellerItemsList.Select(i => i.BulkItem);
                    SetBulkItemsHelper.SetWoocommerceBulkItems(copyFile, bulkItems, BulkItemPropertiesToUpdate.QuantityOnly);
                    //save file as
                    SaveFileDialog save = new SaveFileDialog();
                    save.Filter = "Any File|*.*";
                    save.FileName = Path.GetFileName(copyFile);
                    save.Title = "Save Export Reseller File As";
                    if (save.ShowDialog() == true)
                    {
                        File.Copy(copyFile, save.FileName, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.WriteExceptionLogs(ex);
                MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportShopeeFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //copy shopee file to temp folder
                if (File.Exists(ShopeeFile))
                {
                    string copyFile = CopyToTemp(ShopeeFile);
                    //shopeeItemsList.ForEach(i => i.Quantity = (i.NewQuantity ?? i.Quantity));
                    IEnumerable<BulkItem> bulkItems = shopeeItemsList.Select(i => i.BulkItem);
                    SetBulkItemsHelper.SetShopeeBulkItems2(copyFile, bulkItems);
                    //save file as
                    SaveFileDialog save = new SaveFileDialog();
                    save.Filter = "Any File|*.*";
                    save.FileName = Path.GetFileName(copyFile);
                    save.Title = "Save Export Shopee File As";
                    if (save.ShowDialog() == true)
                    {
                        File.Copy(copyFile, save.FileName, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.WriteExceptionLogs(ex);
                MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static string CopyToTemp(string file)
        {
            string dir = Path.Combine(Path.GetTempPath(), "export files");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string tempFile = Path.Combine(dir, Path.GetFileNameWithoutExtension(file) + "_updated" + Path.GetExtension(file));
            File.Copy(file, tempFile, true);
            return tempFile;
        }

        private void ShowMissing_Click(object sender, RoutedEventArgs e)
        {
            FilterGroup filters = (sender as FrameworkElement).Tag as FilterGroup;
            filters.ShowNotExists = true;
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            string file = FileExtension.GetFileName(sender as FrameworkElement);
            if (File.Exists(file))
                FileHelper.Open(file);
            else
                MessageBox.Show("ERROR: File not found", "", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        //private void UpdateQuantityFromShopeeToReseller()
        //{
        //    //set new quantity & price
        //    foreach (var reseller in resellerItemsList)
        //    {
        //        var shopeeItem = shopeeItemsList.FirstOrDefault(i => i.ShopeeItem.ItemNumber == reseller.ActualItemNumber);
        //        if (shopeeItem != null)// && shopeeItem.Quantity != (reseller.Quantity ?? 0))
        //        {
        //            //set new quantity
        //            int newQty = (shopeeItem.Quantity ?? 0);
        //            if (newQty < 0) newQty = 0;
        //            reseller.NewQuantity = newQty;

        //            //set new price
        //            decimal newPrice = (shopeeItem.RegularPrice ?? 0);
        //            if (newPrice < 0) newPrice = 0;
        //            reseller.NewPrice = newPrice;
        //        }
        //        else
        //        {
        //            reseller.NewQuantity = null;
        //            reseller.NewPrice = null;
        //        }
        //    }
        //    ResellerItemsView.Refresh();
        //}

        private void ShowDuplicate_Click(object sender, RoutedEventArgs e)
        {
            //string itemNumber = ((sender as FrameworkElement).DataContext as ItemCompare).ItemNumber;
            //FiltersReseller.FilterKeyword = itemNumber;
        }

        private void PrintItems_Click(object sender, RoutedEventArgs e)
        {
            ICollectionView view = (ICollectionView)(sender as FrameworkElement).Tag;
            IEnumerable<IItem> items = view.OfType<IItem>();
            Reports.ReportHelper.PrintBulkItems(items);
        }

        private void EditItemName_Click(object sender, RoutedEventArgs e)
        {
            //ItemCompare item = (sender as FrameworkElement).DataContext as ItemCompare;
            //var clone = item.DeepClone();
            //editItemNamePopup.PlacementTarget = sender as UIElement;
            //editItemNamePopup.DataContext = clone;
            //editItemNamePopup.IsOpen = true;
            //if (editItemNamePopup.IsSubmitted)
            //{
            //    clone.DeepCopyTo(item);
            //    SetBulkItemsHelper.UpdateWoocommerceSingleBulkItem(this.ResellerFile, item.BulkItem, BulkItemPropertiesToUpdate.ItemNameOnly);
            //}
            //editItemNamePopup.DataContext = null;
        }

        private void UpdateQuantity_Click(object sender, RoutedEventArgs e)
        {
            //ItemCompare item = (sender as FrameworkElement).DataContext as ItemCompare;
            //item.Quantity = item.NewQuantity;
            //SetBulkItemsHelper.UpdateWoocommerceSingleBulkItem(this.ResellerFile, item.BulkItem, BulkItemPropertiesToUpdate.QuantityAndPrice);
        }

        private void UpdatePrice_Click(object sender, RoutedEventArgs e)
        {
            //ItemCompare item = (sender as FrameworkElement).DataContext as ItemCompare;
            //item.RegularPrice = item.NewPrice;
            //SetBulkItemsHelper.UpdateWoocommerceSingleBulkItem(this.ResellerFile, item.BulkItem, BulkItemPropertiesToUpdate.QuantityAndPrice);
        }

        private void SearchSku_Click(object sender, RoutedEventArgs e)
        {
            FilterGroup filters = (sender as FrameworkElement).Tag as FilterGroup;
            filters.FilterKeyword = Extensions.ItemsExtension.GetItemNumber(sender as FrameworkElement);
        }
    }
}
