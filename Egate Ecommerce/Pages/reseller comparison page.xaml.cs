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
    /// Interaction logic for reseller_comparison_page.xaml
    /// </summary>
    public partial class reseller_comparison_page : Page
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
        public static readonly DependencyProperty PosItemsViewProperty = DependencyProperty.Register(nameof(PosItemsView), typeof(ICollectionView), typeof(reseller_comparison_page));
        public ICollectionView PosItemsView
        { 
            get { return (ICollectionView)GetValue(PosItemsViewProperty); }
            set { SetValue(PosItemsViewProperty, value); }
        }

        public static readonly DependencyProperty ResellerItemsViewProperty = DependencyProperty.Register(nameof(ResellerItemsView), typeof(ICollectionView), typeof(reseller_comparison_page));
        public ICollectionView ResellerItemsView
        {
            get { return (ICollectionView)GetValue(ResellerItemsViewProperty); }
            set { SetValue(ResellerItemsViewProperty, value); }
        }

        public static readonly DependencyProperty FiltersPosProperty = DependencyProperty.Register(nameof(FiltersPos), typeof(FilterGroup), typeof(reseller_comparison_page), new PropertyMetadata(new FilterGroup()));
        public FilterGroup FiltersPos
        {
            get { return (FilterGroup)GetValue(FiltersPosProperty); }
            set { SetValue(FiltersPosProperty, value); }
        }

        public static readonly DependencyProperty FiltersResellerProperty = DependencyProperty.Register(nameof(FiltersReseller), typeof(FilterGroup), typeof(reseller_comparison_page), new PropertyMetadata(new FilterGroup()));
        public FilterGroup FiltersReseller
        {
            get { return (FilterGroup)GetValue(FiltersResellerProperty); }
            set { SetValue(FiltersResellerProperty, value); }
        }

        public static readonly DependencyProperty TotalsPosProperty = DependencyProperty.Register(nameof(TotalsPos), typeof(TotalGroup), typeof(reseller_comparison_page), new PropertyMetadata(new TotalGroup()));
        public TotalGroup TotalsPos
        {
            get { return (TotalGroup)GetValue(TotalsPosProperty); }
            set { SetValue(TotalsPosProperty, value); }
        }

        public static readonly DependencyProperty TotalsResellerProperty = DependencyProperty.Register(nameof(TotalsReseller), typeof(TotalGroup), typeof(reseller_comparison_page), new PropertyMetadata(new TotalGroup()));
        public TotalGroup TotalsReseller
        {
            get { return (TotalGroup)GetValue(TotalsResellerProperty); }
            set { SetValue(TotalsResellerProperty, value); }
        }

        public static readonly DependencyProperty ResellerFileProperty = DependencyProperty.Register(nameof(ResellerFile), typeof(string), typeof(reseller_comparison_page));
        public string ResellerFile
        {
            get { return (string)GetValue(ResellerFileProperty); }
            set { SetValue(ResellerFileProperty, value); }
        }
        #endregion

        private const string RESELLER_FILE_DIRECTORY = @"export files\reseller";
        private List<PosItemCompare> posItemsList = new List<PosItemCompare>();
        private List<ItemCompare> resellerItemsList = new List<ItemCompare>();

        public reseller_comparison_page()
        {
            PosItemsView = new CollectionViewSource() { Source = posItemsList }.View;
            PosItemsView.SortDescriptions.Add(new SortDescription("IsExists", ListSortDirection.Ascending));
            PosItemsView.SortDescriptions.Add(new SortDescription("PosItem.ItemNumber", ListSortDirection.Ascending));
            PosItemsView.Filter = x => DoFilterPosItem(x as PosItemCompare);

            ResellerItemsView = new CollectionViewSource() { Source = resellerItemsList }.View;
            ResellerItemsView.SortDescriptions.Add(new SortDescription("IsExists", ListSortDirection.Ascending));
            ResellerItemsView.SortDescriptions.Add(new SortDescription("ActualItemNumber", ListSortDirection.Ascending));
            ResellerItemsView.Filter = x => DoFilterResellerItem(x as ItemCompare);

            FiltersPos.PropertyChanged += FiltersPos_PropertyChanged;
            FiltersReseller.PropertyChanged += FiltersReseller_PropertyChanged;

            InitializeComponent();

            //only need to load pos data once
            LoadPosData();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPosDetailsData();
            LoadResellerData();
            DoComparison();
            GetTotals();
            GetDuplicateCount();
            GetItemInfoCount();
            UpdateQuantityFromPosToReseller();
        }

        private void LoadPosData()
        {
            posItemsList.Clear();
            posItemsList.AddRange(QbPosInventory.Items.Select(i => new PosItemCompare(i)));
            PosItemsView.Refresh();
            //get file date
            TotalsPos.FileDate = File.Exists(QbPosInventory.InventoryFile) ? (DateTime?)File.GetLastWriteTime(QbPosInventory.InventoryFile) : null;
        }

        private void LoadResellerData()
        {
            this.ResellerFile = FileHelper.GetFiles("*.csv", reseller_comparison_page.RESELLER_FILE_DIRECTORY).FirstOrDefault();
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

        private void LoadPosDetailsData()
        {
            Task.Run(async () =>
            {
                var detailsList = await ECommerceHelper.GetItemDetailsAsync();
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    foreach (var pos in posItemsList)
                    {
                        var itemDetails = detailsList.FirstOrDefault(i => i.ItemNumber == pos.ItemNumber);
                        if (itemDetails != null)
                            pos.ItemDetails = itemDetails;
                    }
                }), System.Windows.Threading.DispatcherPriority.Background);
            });
        }

        private void DoComparison()
        {
            if (resellerItemsList.Count == 0)
            {
                //default to exists all -> if no reseller items list
                foreach (var pos in posItemsList)
                    pos.IsExists = true;
            }
            else
            {
                //compare to pos items
                foreach (var pos in posItemsList)
                    pos.IsExists = resellerItemsList.Any(i => i.ActualItemNumber == pos.PosItem.ItemNumber);
            }
            PosItemsView.Refresh();

            if (posItemsList.Count == 0)
            {
                //default to exists all -> if no pos items list
                foreach (var reseller in resellerItemsList)
                    reseller.IsExists = true;
            }
            else
            {
                //compare to reseller items
                foreach (var reseller in resellerItemsList)
                    reseller.IsExists = posItemsList.Any(i => i.PosItem.ItemNumber == reseller.ActualItemNumber);
            }
            ResellerItemsView.Refresh();
        }

        private void GetDuplicateCount()
        {
            foreach (var pos in posItemsList)
            {
                Task.Run(() =>
                {
                    int? duplicate = resellerItemsList.Count(i => i.ActualItemNumber == pos.ItemNumber);
                    //if (duplicate <= 1) duplicate = null;
                    pos.DuplicateCount = duplicate;
                });
            }
        }

        private void GetItemInfoCount()
        {
            Task.Run(async () =>
            {
                var itemInfoList = await ECommerceHelper.GetItemInfoListAsync();
                foreach (var pos in posItemsList)
                {
                    pos.ItemInfoCount = itemInfoList.Count(i => i.ItemNumber == pos.PosItem.ItemNumber);
                }
            });
        }

        private void GetTotals()
        {
            //pos items -> but has independent filtering
            var filteredPosItems = posItemsList.Where(i =>
            {
                bool flag = true;
                //stock status
                if (FiltersPos.FilterStockStatus != 0)
                {
                    if (FiltersPos.FilterStockStatus == 1) //with stock
                        flag &= i.PosItem.Quantity != 0;
                    else if (FiltersPos.FilterStockStatus == 2) //without stock
                        flag &= i.PosItem.Quantity == 0;
                }
                //keyword
                if (!string.IsNullOrEmpty(FiltersPos.FilterKeyword))
                {
                    string keyword = FiltersPos.FilterKeyword.Trim();
                    flag &= (i.PosItem.ItemNumber.StartsWith(keyword, StringComparison.InvariantCultureIgnoreCase) ||
                            i.PosItem.ItemName.IndexOf(keyword, 0, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                            (i.PosItem.ItemDescription ?? string.Empty).IndexOf(keyword, 0, StringComparison.InvariantCultureIgnoreCase) >= 0);
                }
                //department
                if (!string.IsNullOrWhiteSpace(FiltersPos.FilterDepartment))
                    flag &= (i.PosItem.DepartmentName == FiltersPos.FilterDepartment);
                return flag;
            });
            TotalsPos.TotalCount = filteredPosItems.Count();
            TotalsPos.NotExistsCount = filteredPosItems.Count(i => !i.IsExists);
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

        private void FiltersPos_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (FiltersPos.CanRefresh)
            {
                PosItemsView.Refresh();
                GetTotals();
            }
        }

        private bool DoFilterPosItem(PosItemCompare i)
        {
            bool flag = true;
            //keyword
            if (!string.IsNullOrEmpty(FiltersPos.FilterKeyword))
            {
                string keyword = FiltersPos.FilterKeyword.Trim();
                flag &= (i.PosItem.ItemNumber.StartsWith(keyword, StringComparison.InvariantCultureIgnoreCase) ||
                        i.PosItem.ItemName.IndexOf(keyword, 0, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        (i.PosItem.ItemDescription ?? string.Empty).IndexOf(keyword, 0, StringComparison.InvariantCultureIgnoreCase) >= 0);
            }
            //department
            if (!string.IsNullOrWhiteSpace(FiltersPos.FilterDepartment))
                flag &= (i.PosItem.DepartmentName == FiltersPos.FilterDepartment);
            //stock status
            if (FiltersPos.FilterStockStatus != 0)
            {
                if (FiltersPos.FilterStockStatus == 1) //with stock
                    flag &= i.PosItem.Quantity != 0;
                else if (FiltersPos.FilterStockStatus == 2) //without stock
                    flag &= i.PosItem.Quantity == 0;
            }
            //show not exists
            if (FiltersPos.ShowNotExists)
                flag &= !i.IsExists;
            //show memo
            if (FiltersPos.ShowMemo)
                flag &= !string.IsNullOrEmpty(i.ItemDetails.Memo);
            //show to be updated
            if (FiltersPos.ShowDetailToBeUpdated)
                flag &= i.ItemDetails.ForUpdate;
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
                string[] files = FileHelper.GetFiles("*.*", reseller_comparison_page.RESELLER_FILE_DIRECTORY, SearchOption.TopDirectoryOnly);
                foreach (var f in files)
                    File.Delete(f);
                //save new excel file
                FileHelper.Upload(open.FileName, reseller_comparison_page.RESELLER_FILE_DIRECTORY);

                LoadResellerData();
                DoComparison();
                GetTotals();
                GetDuplicateCount();
                UpdateQuantityFromPosToReseller();
            }
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

        private static string CopyToTemp(string file)
        {
            string dir = Path.Combine(Path.GetTempPath(), "export files");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string tempFile = Path.Combine(dir, Path.GetFileNameWithoutExtension(file) + "_updated" + Path.GetExtension(file));
            File.Copy(file, tempFile, true);
            return tempFile;
        }

        private void UpdateQuantityFromPosToReseller()
        {
            //set new quantity & price
            foreach (var reseller in resellerItemsList)
            {
                var posItem = posItemsList.FirstOrDefault(i => i.PosItem.ItemNumber == reseller.ActualItemNumber);
                if (posItem != null)// && posItem.Quantity != (reseller.Quantity ?? 0))
                {
                    //set new quantity
                    int newQty = (posItem.Quantity ?? 0);
                    if (newQty < 0) newQty = 0;
                    reseller.NewQuantity = newQty;

                    //set new price
                    decimal newPrice = (posItem.RegularPrice ?? 0);
                    if (newPrice < 0) newPrice = 0;
                    reseller.NewPrice = newPrice;
                }
                else
                {
                    reseller.NewQuantity = null;
                    reseller.NewPrice = null;
                }
            }
            ResellerItemsView.Refresh();
        }

        private void ShowDuplicate_Click(object sender, RoutedEventArgs e)
        {
            string itemNumber = ((sender as FrameworkElement).DataContext as PosItemCompare).ItemNumber;
            FiltersReseller.FilterKeyword = itemNumber;
        }

        private void PrintItems_Click(object sender, RoutedEventArgs e)
        {
            ICollectionView view = (ICollectionView)(sender as FrameworkElement).Tag;
            IEnumerable<IItem> items = view.OfType<IItem>();
            Reports.ReportHelper.PrintBulkItems(items);
        }

        private void EditItemName_Click(object sender, RoutedEventArgs e)
        {
            ItemCompare item = (sender as FrameworkElement).DataContext as ItemCompare;
            var clone = item.DeepClone();
            editItemNamePopup.PlacementTarget = sender as UIElement;
            editItemNamePopup.DataContext = clone;
            editItemNamePopup.IsOpen = true;
            if (editItemNamePopup.IsSubmitted)
            {
                clone.DeepCopyTo(item);
                SetBulkItemsHelper.UpdateWoocommerceSingleBulkItem(this.ResellerFile, item.BulkItem, BulkItemPropertiesToUpdate.ItemNameOnly);
            }
            editItemNamePopup.DataContext = null;
        }

        private void UpdateQuantity_Click(object sender, RoutedEventArgs e)
        {
            ItemCompare item = (sender as FrameworkElement).DataContext as ItemCompare;
            item.Quantity = item.NewQuantity;
            SetBulkItemsHelper.UpdateWoocommerceSingleBulkItem(this.ResellerFile, item.BulkItem, BulkItemPropertiesToUpdate.QuantityAndPrice);
        }

        private void UpdatePrice_Click(object sender, RoutedEventArgs e)
        {
            ItemCompare item = (sender as FrameworkElement).DataContext as ItemCompare;
            item.RegularPrice = item.NewPrice;
            SetBulkItemsHelper.UpdateWoocommerceSingleBulkItem(this.ResellerFile, item.BulkItem, BulkItemPropertiesToUpdate.QuantityAndPrice);
        }

        private void EditDetailsMemo_Click(object sender, RoutedEventArgs e)
        {
            PosItemCompare pos = (sender as FrameworkElement).DataContext as PosItemCompare;
            ItemDetailsViewModel details = pos.ItemDetails;
            var clone = details.DeepClone();
            editDetailsMemoPopup.PlacementTarget = sender as UIElement;
            editDetailsMemoPopup.DataContext = clone;
            editDetailsMemoPopup.IsOpen = true;
            if (editDetailsMemoPopup.IsSubmitted)
            {
                clone.DeepCopyTo(details);
                
                _ = ECommerceHelper.AddItemDetailsAsync(pos.ItemDetails);
            }
            editItemNamePopup.DataContext = null;
        }

        private void UpdateItemDetails_Click(object sender, RoutedEventArgs e)
        {
            PosItemCompare pos = (sender as FrameworkElement).DataContext as PosItemCompare;
            ItemDetailsViewModel details = pos.ItemDetails;
            details.MemoUpdatedDate = DateTime.Now;
            _ = ECommerceHelper.AddItemDetailsAsync(pos.ItemDetails);
        }

        private void ClearAllMemo_Click(object sender, RoutedEventArgs e)
        {
            var modal = new admin_password_modal();
            modal.Message = "Please enter password to Clear All Memo";
            if (ModalForm.ShowModal(modal, "Clear All Memo", ModalButtons.YesNo) == ModalResult.Yes)
            {
                Task.Run(async () =>
                {
                    //only clear existing item details
                    posItemsList.ForEach(i =>
                    {
                        if (!i.ItemDetails.IsNew)
                        {
                            i.ItemDetails.Memo = string.Empty;
                            i.ItemDetails.MemoUpdatedDate = DateTime.Now;
                        }
                    });
                    await ECommerceHelper.UpdateAllItemDetailsAsync(posItemsList.Select(i => i.ItemDetails));
                });
            }
        }

        private void ClearAllToBeUpdated_Click(object sender, RoutedEventArgs e)
        {
            var modal = new admin_password_modal();
            modal.Message = "Please enter password to Clear All Checked";
            if (ModalForm.ShowModal(modal, "Clear All Checked", ModalButtons.YesNo) == ModalResult.Yes)
            {
                Task.Run(async () =>
                {
                    //only clear existing item details
                    posItemsList.ForEach(i =>
                    {
                        if (!i.ItemDetails.IsNew)
                        {
                            i.ItemDetails.ForUpdate = false;
                            i.ItemDetails.MemoUpdatedDate = DateTime.Now;
                        }
                    });
                    await ECommerceHelper.UpdateAllItemDetailsAsync(posItemsList.Select(i => i.ItemDetails));
                });
            }
        }

        private void AddInfo_Click(object sender, RoutedEventArgs e)
        {
            PosItemCompare pos = (sender as FrameworkElement).DataContext as PosItemCompare;
            ItemInfoViewModel itemInfo = new ItemInfoViewModel();
            itemInfo.PosItem = pos.PosItem;
            itemInfo.ItemNumber = pos.ItemNumber;
            var modal = new item_info_add_modal();
            modal.DataContext = itemInfo;
            if (ModalForm.ShowModal(modal, "Add Item Information", ModalButtons.SaveCancel) == ModalResult.Save)
            {
                itemInfo.UpdatedDate = DateTime.Now;
                _ = ECommerceHelper.AddItemInfoAsync(itemInfo);
            }
        }

        private void ShowCanvasList_Click(object sender, RoutedEventArgs e)
        {
            PosItemCompare pos = (sender as FrameworkElement).DataContext as PosItemCompare;
            itemInfoList.LoadItems(pos.PosItem.ItemNumber);
            showCanvasListingPopup.DataContext = pos;
            showCanvasListingPopup.PlacementTarget = sender as UIElement;
            showCanvasListingPopup.IsOpen = true;
            showCanvasListingPopup.DataContext = null;
        }

        private void SearchSku_Click(object sender, RoutedEventArgs e)
        {
            FilterGroup filters = (sender as FrameworkElement).Tag as FilterGroup;
            filters.FilterKeyword = Extensions.ItemsExtension.GetItemNumber(sender as FrameworkElement);
        }
    }
}
