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
using System.Text.RegularExpressions;
using Microsoft.Win32;
using Egate_Ecommerce.Classes;
using Egate_Ecommerce.Objects;
using bolt5.ModalWpf;
using bolt5.CloneCopy;
using Bulk_Update;
using Egate_Ecommerce.Modals.ECommerce;

namespace Egate_Ecommerce.Pages
{
    /// <summary>
    /// Interaction logic for reseller_details_page.xaml
    /// </summary>
    public partial class reseller_details_page : Page
    {
        public class FilterGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public string FilterKeyword { get; set; }
            public string FilterDepartment { get; set; }
            public string FilterKeywordSpecial { get; set; }
            public bool ShowDuplicateProductName { get; set; }
            public bool ShowDuplicateProductDescription { get; set; }
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
                FilterKeywordSpecial = null;
                FilterDepartment = null;
                ShowDuplicateProductName = false;
                ShowDuplicateProductDescription = false;
                CanRefresh = true;
            }
        }

        public class TotalGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public int TotalCount { get; set; }
            public int DuplicateProductNameCount { get; set; }
            public int DuplicateProductDescriptionCount { get; set; }
            public DateTime? FileDate { get; set; }
        }

        #region Dependency Properties
        public static readonly DependencyProperty ResellerItemsViewProperty = DependencyProperty.Register(nameof(ResellerItemsView), typeof(ICollectionView), typeof(reseller_details_page));
        public ICollectionView ResellerItemsView
        {
            get { return (ICollectionView)GetValue(ResellerItemsViewProperty); }
            set { SetValue(ResellerItemsViewProperty, value); }
        }
        
        public static readonly DependencyProperty FiltersResellerProperty = DependencyProperty.Register(nameof(FiltersReseller), typeof(FilterGroup), typeof(reseller_details_page), new PropertyMetadata(new FilterGroup()));
        public FilterGroup FiltersReseller
        {
            get { return (FilterGroup)GetValue(FiltersResellerProperty); }
            set { SetValue(FiltersResellerProperty, value); }
        }

        public static readonly DependencyProperty TotalsResellerProperty = DependencyProperty.Register(nameof(TotalsReseller), typeof(TotalGroup), typeof(reseller_details_page), new PropertyMetadata(new TotalGroup()));
        public TotalGroup TotalsReseller
        {
            get { return (TotalGroup)GetValue(TotalsResellerProperty); }
            set { SetValue(TotalsResellerProperty, value); }
        }

        public static readonly DependencyProperty ResellerFileProperty = DependencyProperty.Register(nameof(ResellerFile), typeof(string), typeof(reseller_details_page));
        public string ResellerFile
        {
            get { return (string)GetValue(ResellerFileProperty); }
            set { SetValue(ResellerFileProperty, value); }
        }
        #endregion

        private const string RESELLER_FILE_DIRECTORY = @"export files\reseller";
        private List<ItemCompare> resellerItemsList = new List<ItemCompare>();

        public reseller_details_page()
        {
            ResellerItemsView = new CollectionViewSource() { Source = resellerItemsList }.View;
            ResellerItemsView.SortDescriptions.Add(new SortDescription("IsExists", ListSortDirection.Ascending));
            ResellerItemsView.SortDescriptions.Add(new SortDescription("ActualItemNumber", ListSortDirection.Ascending));
            ResellerItemsView.Filter = x => DoFilterResellerItem(x as ItemCompare);
            ResellerItemsView.CollectionChanged += ResellerItemsView_CollectionChanged;

            FiltersReseller.PropertyChanged += FiltersReseller_PropertyChanged;

            InitializeComponent();
        }

        private void ResellerItemsView_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            int viewCount = ResellerItemsView.OfType<object>().Count();
            int selectedIndex = resellerItemsDataGrid.SelectedIndex;
            if (viewCount > 0 && selectedIndex == -1)
                resellerItemsDataGrid.SelectedIndex = 0;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadResellerData();
            GetTotals();
        }

        private void LoadResellerData()
        {
            this.ResellerFile = FileHelper.GetFiles("*.csv", reseller_details_page.RESELLER_FILE_DIRECTORY).FirstOrDefault();
            if (File.Exists(this.ResellerFile))
            {
                resellerItemsList.Clear();
                var bulkItems = GetBulkBulkItemsHelper.GetWoocommerceBulkItems(this.ResellerFile);
                resellerItemsList.AddRange(bulkItems.Select(i => new ItemCompare(i)));
                ResellerItemsView.Refresh();
                TotalsReseller.FileDate = File.GetLastWriteTime(this.ResellerFile);
            }
            else
                TotalsReseller.FileDate = null;
        }

        private void GetTotals()
        {
            Task.Run(async () =>
            {
                IEnumerable<ItemCompare> filteredResellerItems = null;
                await Dispatcher.BeginInvoke(new Action(() =>
                {
                    //reseller items -> but has independent filtering
                    filteredResellerItems = resellerItemsList.Where(i =>
                    {
                        bool flag = true;
                        //keyword
                        if (!string.IsNullOrEmpty(FiltersReseller.FilterKeyword))
                        {
                            string keyword = FiltersReseller.FilterKeyword.Trim();
                            flag &= ((i.ItemNumber ?? string.Empty).StartsWith(keyword, StringComparison.InvariantCultureIgnoreCase) ||
                                    (i.ActualItemNumber ?? string.Empty).StartsWith(keyword, StringComparison.InvariantCultureIgnoreCase) ||
                                    (i.ItemName ?? string.Empty).IndexOf(keyword, 0, StringComparison.InvariantCultureIgnoreCase) >= 0);
                        }
                        //keyword special
                        if (!string.IsNullOrWhiteSpace(FiltersReseller.FilterKeywordSpecial))
                        {
                            string[] keywords = FiltersReseller.FilterKeywordSpecial.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                            bool flag2 = true;
                            foreach (var k in keywords)
                                flag2 &= (Regex.IsMatch(i.ItemName + " " + i.ItemDescriptionPlainText, "\\b" + Regex.Escape(k) + "\\b", RegexOptions.IgnoreCase));
                            flag &= flag2;
                        }
                        return flag;
                    });
                }), System.Windows.Threading.DispatcherPriority.Background);
                await Dispatcher.BeginInvoke(new Action(()=>
                {
                    TotalsReseller.TotalCount = filteredResellerItems.Count();
                    TotalsReseller.DuplicateProductNameCount = filteredResellerItems.GroupBy(i => i.ItemName).Where(g => g.Count() > 1).Sum(g => g.Count());
                    TotalsReseller.DuplicateProductDescriptionCount = filteredResellerItems.GroupBy(i => i.ItemDescription).Where(g => g.Count() > 1).Sum(g => g.Count());
                }), System.Windows.Threading.DispatcherPriority.Background);
            });
        }

        private void FiltersReseller_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (FiltersReseller.CanRefresh)
            {
                ResellerItemsView.Refresh();
                GetTotals();
            }
        }

        private bool DoFilterResellerItem(ItemCompare i)
        {
            bool flag = true;
            //keyword
            if (!string.IsNullOrEmpty(FiltersReseller.FilterKeyword))
            {
                string keyword = FiltersReseller.FilterKeyword.Trim();
                flag &= ((i.ItemNumber ?? string.Empty).StartsWith(keyword, StringComparison.InvariantCultureIgnoreCase) ||
                        (i.ActualItemNumber ?? string.Empty).StartsWith(keyword, StringComparison.InvariantCultureIgnoreCase) ||
                        (i.ItemName ?? string.Empty).IndexOf(keyword, 0, StringComparison.InvariantCultureIgnoreCase) >= 0);
            }
            //keyword special
            if (!string.IsNullOrWhiteSpace(FiltersReseller.FilterKeywordSpecial))
            {
                string[] keywords = FiltersReseller.FilterKeywordSpecial.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                bool flag2 = true;
                foreach (var k in keywords)
                    flag2 &= (Regex.IsMatch(i.ItemName + " " + i.ItemDescriptionPlainText, "\\b" + Regex.Escape(k) + "\\b", RegexOptions.IgnoreCase));
                flag &= flag2;
            }
            //duplicate name
            if (FiltersReseller.ShowDuplicateProductName)
            {
                flag &= resellerItemsList.Where(x => x.ItemName == i.ItemName).Count() > 1;
            }
            //duplicate description
            if (FiltersReseller.ShowDuplicateProductDescription)
            {
                flag &= resellerItemsList.Where(x => x.ItemDescription == i.ItemDescription).Count() > 1;
            }
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
                string[] files = FileHelper.GetFiles("*.*", reseller_details_page.RESELLER_FILE_DIRECTORY, SearchOption.TopDirectoryOnly);
                foreach (var f in files)
                    File.Delete(f);
                //save excel file
                FileHelper.Upload(open.FileName, reseller_details_page.RESELLER_FILE_DIRECTORY);

                LoadResellerData();
                GetTotals();
            }
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
                    //save file as
                    SaveFileDialog save = new SaveFileDialog();
                    save.Filter = "Any File|*.*";
                    save.FileName = Path.GetFileNameWithoutExtension(ResellerFile) + "_updated" + Path.GetExtension(ResellerFile);
                    save.Title = "Save Export Reseller File As";
                    if (save.ShowDialog() == true)
                    {
                        File.Copy(ResellerFile, save.FileName, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Logs.WriteExceptionLogs(ex);
                MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PrintItems_Click(object sender, RoutedEventArgs e)
        {
            ICollectionView view = (ICollectionView)(sender as FrameworkElement).Tag;
            IEnumerable<IItem> items = view.OfType<IItem>();
            Reports.ReportHelper.PrintBulkItems(items);
        }

        private void EditItem_Click(object sender, RoutedEventArgs e)
        {
            ItemCompare item = (sender as FrameworkElement).DataContext as ItemCompare;
            var clone = item.BulkItem.DeepClone();
            var modal = new reseller_item_edit_modal();
            modal.DataContext = clone;
            if (ModalForm.ShowModal(modal, "Edit Reseller Item", ModalButtons.SaveCancel) == ModalResult.Save)
            {
                clone.DeepCopyTo(item.BulkItem);
                SetBulkItemsHelper.UpdateWoocommerceSingleBulkItem(this.ResellerFile, item.BulkItem, BulkItemPropertiesToUpdate.All);
                ResellerItemsView.Refresh();
            }
        }
    }
}
