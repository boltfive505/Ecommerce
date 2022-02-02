using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Text.RegularExpressions;
using Egate_Ecommerce.Classes;
using Egate_Ecommerce.Objects;
using Egate_Ecommerce.Modals.ECommerce;
using Egate_Ecommerce.Quickbooks;
using bolt5.ModalWpf;
using bolt5.CloneCopy;
using System.ComponentModel;

namespace Egate_Ecommerce.Pages
{
    /// <summary>
    /// Interaction logic for item_keywords_page.xaml
    /// </summary>
    public partial class item_keywords_page : Page
    {
        public class PosItemKeywordPair : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public PosItem PosItem { get; set;}
            public ItemKeywordViewModel ItemKeyword { get; set; }

            public PosItemKeywordPair()
            { }

            public PosItemKeywordPair(PosItem posItem)
            {
                this.PosItem = posItem;
                ItemKeyword = new ItemKeywordViewModel() { ItemNumber = this.PosItem.ItemNumber };
            }
        }

        public class FilterGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public string FilterKeyword { get; set; }
            public string FilterDepartment { get; set; }
            public bool ShowWithKeywords { get; set; }

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
                //for item
                FilterKeyword = string.Empty;
                FilterDepartment = string.Empty;
                ShowWithKeywords = false;
                CanRefresh = true;
            }
        }

        public class TotalGroup : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public int TotalItems { get; set; }
            public int TotalKeywords { get; set; }
        }

        #region Dependency Properties
        public static readonly DependencyProperty PosItemKeywordViewProperty = DependencyProperty.Register(nameof(PosItemKeywordView), typeof(ICollectionView), typeof(item_keywords_page));
        public ICollectionView PosItemKeywordView
        { 
            get { return (ICollectionView)GetValue(PosItemKeywordViewProperty); }
            set { SetValue(PosItemKeywordViewProperty, value); }
        }

        public static readonly DependencyProperty FiltersPosItemKeywordProperty = DependencyProperty.Register(nameof(FiltersPosItemKeyword), typeof(FilterGroup), typeof(item_keywords_page), new PropertyMetadata(new FilterGroup()));
        public FilterGroup FiltersPosItemKeyword
        {
            get { return (FilterGroup)GetValue(FiltersPosItemKeywordProperty); }
            set { SetValue(FiltersPosItemKeywordProperty, value); }
        }

        public static readonly DependencyProperty TotalsPosItemKeywordProperty = DependencyProperty.Register(nameof(TotalsPosItemKeyword), typeof(TotalGroup), typeof(item_keywords_page), new PropertyMetadata(new TotalGroup()));
        public TotalGroup TotalsPosItemKeyword
        {
            get { return (TotalGroup)GetValue(TotalsPosItemKeywordProperty); }
            set { SetValue(TotalsPosItemKeywordProperty, value); }
        }
        #endregion

        private List<PosItemKeywordPair> posItemKeywordList = new List<PosItemKeywordPair>();

        public item_keywords_page()
        {
            PosItemKeywordView = new CollectionViewSource() { Source = posItemKeywordList }.View;
            PosItemKeywordView.Filter = x => DoFilterPosItemKeyword(x as PosItemKeywordPair);
            PosItemKeywordView.SortDescriptions.Add(new SortDescription("PosItem.ItemNumber", ListSortDirection.Ascending));

            FiltersPosItemKeyword.PropertyChanged += FiltersPosItemKeyword_PropertyChanged;
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            GetTotals();
        }

        private void LoadData()
        {
            posItemKeywordList.Clear();
            posItemKeywordList.AddRange(QbPosInventory.Items.Select(i => new PosItemKeywordPair(i)));
            PosItemKeywordView.Refresh();
            Task.Run(async () =>
            {
                var keywordList = await ECommerceHelper.GetItemKeywordListAsync();
                foreach (var l in posItemKeywordList)
                {
                    var keywordItem = keywordList.FirstOrDefault(i => i.ItemNumber == l.PosItem.ItemNumber);
                    if (keywordItem != null)
                        l.ItemKeyword = keywordItem;
                }
            });
        }

        private void GetTotals()
        {

        }

        private void FiltersPosItemKeyword_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (FiltersPosItemKeyword.CanRefresh)
            {
                PosItemKeywordView.Refresh();
            }
        }

        private bool DoFilterPosItemKeyword(PosItemKeywordPair i)
        {
            bool flag = true;
            //keyword -> both positem and itemkeyword
            if (!string.IsNullOrWhiteSpace(FiltersPosItemKeyword.FilterKeyword))
            {
                //for item number
                string keyword = FiltersPosItemKeyword.FilterKeyword.Trim();
                bool flag2 = true;
                flag2 &= i.PosItem.ItemNumber.StartsWith(keyword, StringComparison.InvariantCultureIgnoreCase);

                //for other else
                string[] keywords = FiltersPosItemKeyword.FilterKeyword.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                string txt = string.Join(" ", i.PosItem.ItemName, i.PosItem.ItemDescription, i.ItemKeyword.Keywords, i.ItemKeyword.SuggestedName);
                bool flag3 = true;
                foreach (var k in keywords)
                    flag3 &= txt.IndexOf(k, 0, StringComparison.InvariantCultureIgnoreCase) >= 0;
                flag &= (flag2 || flag3);
            }
            //department
            if (!string.IsNullOrWhiteSpace(FiltersPosItemKeyword.FilterDepartment))
                flag &= (i.PosItem.DepartmentName == FiltersPosItemKeyword.FilterDepartment);
            //show with keywords
            if (FiltersPosItemKeyword.ShowWithKeywords)
                flag &= !string.IsNullOrEmpty(i.ItemKeyword?.Keywords);
            return flag;
        }

        private void AddKeywords_Click(object sender, RoutedEventArgs e)
        {
            var posItemKeywords = (sender as FrameworkElement).DataContext as PosItemKeywordPair;
            var keywordsItem = posItemKeywords.ItemKeyword;
            var modal = new item_keywords_add_modal();
            var clone = keywordsItem.DeepClone();
            modal.DataContext = clone;
            if (ModalForm.ShowModal(modal, "Edit Item Keywords", ModalButtons.SaveCancel) == ModalResult.Save)
            {
                clone.DeepCopyTo(keywordsItem);
                keywordsItem.UpdatedDate = DateTime.Now;
                _ = ECommerceHelper.AddItemKeywordAsync(keywordsItem);
                PosItemKeywordView.Refresh();
            }
        }
    }
}
