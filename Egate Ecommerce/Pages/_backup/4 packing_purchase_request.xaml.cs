using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using purchase_request.Model;
using purchase_request.Quickbooks;
using PropertyChanged;
using System.Collections.Generic;
using purchase_request.Classes;

namespace purchase_request.Pages
{
    /// <summary>
    /// Interaction logic for packing_purchase_request.xaml
    /// </summary>
    public partial class packing_purchase_request : Page, INotifyPropertyChanged
    {
        public class Item : _ItemBase, INotifyPropertyChanged
        {
            public long Key { get { return Purchase.Id; } }

            [AlsoNotifyFor("TotalWeight", "RemainingQty", "IsExceedingShipQty")]
            public int ShipQty { get; set; }
            [AlsoNotifyFor("TotalWeight")]
            public decimal UnitWeight { get; set; }
            public decimal TotalWeight { get { return ShipQty * UnitWeight; } }
            public string PackingNote { get; set; }
            public bool IsShippedDirect { get; set; }
            public string BoxLabel { get; set; }
            public int RemainingQty { get { return Purchase == null ? 0 : (int)Purchase.PurchaseQuantity - ShipQty; } }
            public bool IsExceedingShipQty { get { return Purchase == null ? false : ShipQty > Purchase.PurchaseQuantity; } }
            public bool IsChecked2 { get; set; }

            public override void Add()
            {
                base.Add();
                ShipQty = (int)Purchase.PurchaseQuantity;
                UnitWeight = 0;
                PackingNote = string.Empty;
                IsShippedDirect = false;
                BoxLabel = string.Empty;
                IsChecked2 = false;
            }

            public override void Remove()
            {
                base.Remove();
                ShipQty = 0;
            }
        }

        public class Details : INotifyPropertyChanged
        {
            public long Key { get; set; }
            public int Quantity { get; set; }
            public int MaxQuantity { get; set; }
            public decimal UnitWeight { get; set; }
            public string Note { get; set; }            
            public bool IsShippedDirect { get; set; }
            public string BoxLabel { get; set; }
            public event PropertyChangedEventHandler PropertyChanged;
        }

        public class PackingDetails : INotifyPropertyChanged
        {
            public DateTime PackingDate { get; set; }
            public ShipType ShipType { get; set; }
            public bool IsShippedDirect { get; set; }
            [AlsoNotifyFor("BoxLabelList")]
            public int BoxQty { get; set; }
            public decimal TotalWeight { get; set; }

            public IEnumerable<string> BoxLabelList { get { return Enumerable.Range(1, BoxQty).Select(q => string.Format("{0} / {1}", q, BoxQty)); } }
            public event PropertyChangedEventHandler PropertyChanged;

            public PackingDetails()
            {
                PackingDate = DateTime.Now;
                BoxQty = 1;
            }
        }

        public static readonly RoutedEvent SubmitRequestEvent = EventManager.RegisterRoutedEvent("SubmitRequest", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(packing_purchase_request));
        public event RoutedEventHandler SubmitRequest
        {
            add { AddHandler(SubmitRequestEvent, value); }
            remove { RemoveHandler(SubmitRequestEvent, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public ICollectionView ItemList { get; set; }
        public ICollectionView RequestItemList { get; set; }
        public PackingDetails PackingDetail { get; set; }
        public string SelectedBoxLabel { get; set; }

        public string FilterKeyword { get; set; }
        public string FilterDepartment { get; set; }

        private PurchaseRequestModel context;
        private ObservableCollection<Item> items;
        private IEnumerable<Details> _preReloadRequestList;
        private bool filterRefreshFlag;

        public packing_purchase_request()
        {
            InitializeComponent();
            PackingDetail = new PackingDetails();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            context = new PurchaseRequestModel();

            //GetPreReloadRequest();
            var query = from p in context.purchase
                        where p.ReceivedDate != null && p.ShippedDate == null
                        select p;
            var list = _ItemBase.GetItemList2<Item>(query.AsEnumerable(), Item_PropertyChanged);
            items = new ObservableCollection<Item>(list);
            ItemList = new CollectionViewSource() { Source = items }.View;
            ItemList.Filter = i => Filter(i as Item);
            //SetPostReloadRequest();

            RequestItemList = new CollectionViewSource() { Source = items }.View;
            RequestItemList.Filter = i => (i as Item).IsSelected;
            RefreshSelection();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            context.Dispose();
            context = null;
        }

        private void GetPreReloadRequest()
        {
            if (RequestItemList == null) return;
            _preReloadRequestList = RequestItemList.Cast<Item>()
                .Select(i => new Details()
                {
                    Key = i.Key,
                    Quantity = i.ShipQty,
                    Note = i.PackingNote,
                    UnitWeight = i.UnitWeight,
                    IsShippedDirect = i.IsShippedDirect,
                    BoxLabel = i.BoxLabel
                });
        }

        private void SetPostReloadRequest()
        {
            if (_preReloadRequestList == null || _preReloadRequestList.Count() == 0) return;
            if (items == null || items.Count == 0) return;
            _preReloadRequestList.ToList().ForEach(r =>
            {
                Item item = items.FirstOrDefault(i => i.Key == r.Key);
                if (item != null)
                {
                    item.IsSelected = true;
                    item.ShipQty = r.Quantity;
                    item.PackingNote = r.Note;
                    item.UnitWeight = r.UnitWeight;
                    item.IsShippedDirect = r.IsShippedDirect;
                    item.BoxLabel = r.BoxLabel;
                }
            });
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (!filterRefreshFlag && propertyName.StartsWith("Filter"))
                ItemList?.Refresh();
        }

        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            context.SaveChanges();
            if (e.PropertyName == "TotalWeight")
                PackingDetail.TotalWeight = RequestItemList.Cast<Item>().Sum(i => i.TotalWeight);
        }

        private void OnSelectedBoxLabelChanged()
        {
            foreach (var i in RequestItemList.Cast<Item>())
            {
                if (i.IsChecked2)
                    i.BoxLabel = SelectedBoxLabel;
            }
        }

        private bool Filter(Item i)
        {
            if (i.Purchase == null) return false;

            bool flag = true;

            //request
            flag &= i.Purchase.ReceivedDate != null && i.Purchase.ShippedDate == null;

            if (i.PosItem != null)
            {
                //keyword
                if (!string.IsNullOrEmpty(FilterKeyword))
                {
                    string keyword = FilterKeyword.Trim();
                    flag &= (i.PosItem.ItemNumber.IndexOf(keyword, 0, StringComparison.InvariantCultureIgnoreCase) >= 0
                                || i.PosItem.ItemName.IndexOf(keyword, 0, StringComparison.InvariantCultureIgnoreCase) >= 0
                                || (i.PosItem.ItemDescription != null && i.PosItem.ItemDescription.IndexOf(keyword, 0, StringComparison.InvariantCultureIgnoreCase) >= 0));
                }

                //department
                if (!string.IsNullOrWhiteSpace(FilterDepartment))
                {
                    flag &= (i.PosItem.DepartmentName == FilterDepartment);
                }
            }

            //selected or partial quantity
            if (i.IsSelected) flag &= i.RemainingQty > 0;

            return flag;
        }

        private void RefreshSelection()
        {
            ItemList.Refresh();
            RequestItemList.Refresh();
        }

        private void reset_filters_Click(object sender, RoutedEventArgs e)
        {
            filterRefreshFlag = true;
            using (ItemList.DeferRefresh())
            {
                FilterKeyword = null;
                FilterDepartment = null;
            }
            ItemList.Refresh();
            filterRefreshFlag = false;
        }

        private void ClearRequestItemsList()
        {
            RequestItemList.Cast<Item>()
                .ToList()
                .ForEach(i => i.IsSelected = false);
            RefreshSelection();
        }

        private void CheckAllHeader_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var i in RequestItemList.Cast<Item>())
                i.IsChecked2 = true;
        }

        private void CheckAllHeader_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (var i in RequestItemList.Cast<Item>())
                i.IsChecked2 = false;
        }

        private void add_request_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Item item = ((FrameworkElement)sender).DataContext as Item;
            if (item == null) return;
            item.Add();
            RefreshSelection();
        }

        private void remove_request_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Item item = ((FrameworkElement)sender).DataContext as Item;
            item.Remove();
            RefreshSelection();
        }

        private void create_request_btn_Click(object sender, RoutedEventArgs e)
        {
            var requestItems = RequestItemList.Cast<Item>().ToList();
            if (requestItems.Count() == 0) return;

            string packingNumber = _ItemBase.GetUniquePackingNumber(PackingDetail.PackingDate.ToString("yyMMdd"));
            var modal = new Modals.packing_request_modal();

            modal.ItemsDataContext = RequestItemList;
            modal.PackingDetailsContext = PackingDetail;
            modal.ReportDataSource = requestItems.Select(i => new Reports.Objects.packing
            {
                PackingNumber = packingNumber,
                BoxLabel = i.BoxLabel,
                ItemNumber = i.PosItem.ItemNumber,
                IsNonInventory = i.PosItem.IsNonInventory,
                ItemName = i.PosItem.ItemName,
                ItemDescription = i.PosItem.ItemDescription,
                Quantity = i.Purchase.PurchaseQuantity,
                ImagePath = i.PosItem.ImagePath
            }).ToList();

            if (Modals.ModalForm.ShowModalYesNo(modal, "Packing Details", "", App.GetResource("PackingPurchaseRequest.Label", "Proceed"), App.GetResource("CancelButtonLabel", "Cancel")) == Modals.ModalResult.Yes)
            {
                packing pk = new packing();
                pk.PackingNumber = packingNumber;
                pk.PackingDate = PackingDetail.PackingDate.ToUnixLong();
                pk.ShipType = PackingDetail.ShipType.ToString();
                pk.TotalBoxQuantity = PackingDetail.BoxQty;
                pk.TotalWeight = PackingDetail.TotalWeight;

                DateTime now = DateTime.Now;
                foreach (var item in requestItems)
                {
                    purchase p;
                    //has remaining quantity
                    if (item.RemainingQty > 0)
                    {
                        //only take partial purchased quantity
                        p = ClonePurchase(item.Purchase);
                        p.PurchaseQuantity = item.ShipQty;
                        context.purchase.Add(p);
                        item.Purchase.PurchaseQuantity -= item.ShipQty;
                    }
                    else
                    {
                        p = item.Purchase;
                        p.PurchaseQuantity = item.ShipQty; //allow for exceeding quantity
                    }

                    p.UnitWeight = item.UnitWeight;
                    p.ShippedDate = now.ToUnixLong();
                    p.ShippingNote = item.PackingNote;
                    p.IsShippedDirect = item.IsShippedDirect.ToLong();
                    p.BoxLabel = item.BoxLabel;
                    p.packing = pk;
                }
                context.SaveChanges();
                ClearRequestItemsList();
                UpdatedTableHelper.SetTableUpdate(typeof(purchase));
                PackingDetail = new PackingDetails();
                RaiseEvent(new RoutedEventArgs(SubmitRequestEvent));
            }
        }

        private void ClearSelection_Button_Click(object sender, RoutedEventArgs e)
        {
            ClearRequestItemsList();
        }

        private static purchase ClonePurchase(purchase src)
        {
            purchase p = new purchase();
            p.ItemNumber = src.ItemNumber;
            p.PurchaseQuantity = src.PurchaseQuantity;
            p.UnitPrice = src.UnitPrice;
            p.RequestDate = src.RequestDate;
            p.ApprovedDate = src.ApprovedDate;
            p.ReceivedDate = src.ReceivedDate;
            p.PurchaseNote = src.PurchaseNote;
            p.IncomingNote = src.IncomingNote;
            p.IssueNote = src.IssueNote;
            return p;
        }

        private void resetBoxNo_btn_Click(object sender, RoutedEventArgs e)
        {
            foreach (var i in RequestItemList.Cast<Item>())
                i.BoxLabel = null;
        }
    }
}
