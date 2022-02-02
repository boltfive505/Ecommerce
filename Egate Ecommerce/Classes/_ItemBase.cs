using System;
using purchase_request.Model;
using Egate_Ecommerce.Quickbooks;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using Egate_Ecommerce.Objects;
using Egate_Ecommerce.Objects.KoreaShipList;
using Egate_Ecommerce.Objects.MobileInventory;
using PropertyChanged;

namespace Egate_Ecommerce.Classes
{
    public class _ItemBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public purchase Purchase { get; set; }
        public PosItem PosItem { get; set; }

        public double SalesQty1 { get; set; }
        public double SalesQty2 { get; set; }
        public double SalesQty3 { get; set; }
        public double DisplayedSalesQty { get; set; } //display only

        public int RequestedQty { get; set; }
        public int PreparedQty { get; set; }
        public int PackedQty { get; set; }
        public int ShippedQty { get; set; }

        public string ImagePath { get { return PosItem?.ImagePath; } }

        public int SuggestedQuantity
        {
            get
            {
                int availableQty = this.PosItem.Quantity + this.OnTheWayTotal;
                int reorderPoint = (int)(this.PosItem.ReorderPoint ?? 0);
                if (reorderPoint < availableQty) return 0;
                return reorderPoint - availableQty;
            }
        }
        public double SuggestedQuantityPercent
        {
            get
            {
                if ((this.PosItem.ReorderPoint ?? 0) == 0) return 0;
                return (double)Math.Round(this.SuggestedQuantity / this.PosItem.ReorderPoint.Value, 2) * 100;
            }
        }

        public int ItemInfoCount { get; set; }

        //item details
        public int? ReorderQuantity { get; set; }
        private bool _canUpdateDetails;

        public List<KoreaShipItemViewModel> KoreaShipItemList { get; set; }
        public MobileInventoryItem MobileInventoryItem { get; set; }

        public int OnTheWayTotal { get { return KoreaShipItemList?.Where(i => i.Status == KoreaShipStatus.NotReceived).Sum(i => i.Quantity) ?? 0; } }

        public long OtherQtySort
        {
            get { return (RequestedQty + PreparedQty + PackedQty + ShippedQty); }
        }

        public bool IsSelected { get; set; }

        public virtual void Add()
        {
            IsSelected = true;
        }

        public virtual void Remove()
        {
            IsSelected = false;
        }

        private void OnReorderQuantityChanged()
        {
            if (_canUpdateDetails)
                _ = DataCache.UpdateItemDetails(this);
        }

        private static int NullableToLong(int? value)
        {
            if (value == null) value = 0;
            return (int)value;
        }

        private static void SetSalesInfo<T>(ref T item) where T :_ItemBase
        {
            var itemNumber = item.Purchase != null ? item.Purchase.ItemNumber : (item.PosItem != null ? item.PosItem.ItemNumber : null);
            item.SalesQty1 = QbPosMonthlySales.MonthlySalesReport_1.Items.Where(s => s.ItemNumber == itemNumber).Sum(s => s.Quantity);
            item.SalesQty2 = QbPosMonthlySales.MonthlySalesReport_3.Items.Where(s => s.ItemNumber == itemNumber).Sum(s => s.Quantity);
            item.SalesQty2 = Math.Round(item.SalesQty2 / 3, 2);
            item.SalesQty3 = QbPosMonthlySales.MonthlySalesReport_6.Items.Where(s => s.ItemNumber == itemNumber).Sum(s => s.Quantity);
            item.SalesQty3 = Math.Round(item.SalesQty3 / 6, 2);
        }

        private static void SetPurchaseInfo<T>(ref T item, IEnumerable<PurchaseInfo> purchaseInfo) where T : _ItemBase
        {
            var itemNumber = item.Purchase != null ? item.Purchase.ItemNumber : (item.PosItem != null ? item.PosItem.ItemNumber : null);
            if (purchaseInfo != null)
            {
                item.RequestedQty = NullableToLong(purchaseInfo.FirstOrDefault(pi => pi.ItemNumber == itemNumber)?.RequestedQty);
                item.PreparedQty = NullableToLong(purchaseInfo.FirstOrDefault(pi => pi.ItemNumber == itemNumber)?.PreparedQty);
                item.PackedQty = NullableToLong(purchaseInfo.FirstOrDefault(pi => pi.ItemNumber == itemNumber)?.PackedQty);
                item.ShippedQty = NullableToLong(purchaseInfo.FirstOrDefault(pi => pi.ItemNumber == itemNumber)?.ShippedQty);
            }
        }

        public static List<T> GetItemListSimple<T>() where T : _ItemBase, new()
        {
            var nonInvItems = DataCache.GetNonInventoryItemList();
            var posItems = QbPosInventory.Items.Concat(nonInvItems);
            var items = posItems
               .Select(i =>
               {
                   T item = new T();
                   item.PosItem = i;
                   return item;
               })
               .ToList();
            return items;
        }

        public static List<T> GetItemList1<T>() where T : _ItemBase, new()
        {
            var posItems = QbPosInventory.Items;
            var purchaseInfo = DataCache.GetPurchaseInfoList();
            var mobileInventoryList = MobileInventoryHelper.GetList();
            var koreaShipItemList = KoreaShipListHelper.GetList();
            var itemDetailsList = DataCache.GetItemDetailsList();

            var items = posItems
                .Select(i =>
                {
                    T item = new T();
                    item.PosItem = i;
                    SetSalesInfo(ref item);
                    SetPurchaseInfo(ref item, purchaseInfo);
                    item.MobileInventoryItem = mobileInventoryList.FirstOrDefault(m => m.ItemNumber == item.PosItem.ItemNumber);
                    item.KoreaShipItemList = koreaShipItemList.Where(k => k.Sku == item.PosItem.ItemNumber).ToList();
                    //set item details
                    var details = itemDetailsList.FirstOrDefault(d => d.ItemNumber == item.PosItem.ItemNumber);
                    if (details != null)
                    {
                        item.ReorderQuantity = details.ReorderQuantity;
                    }
                    item._canUpdateDetails = true;

                    return item;
                }).ToList();
            return items;
        }

        public static List<T> GetItemList2<T>(IEnumerable<purchase> purchaseList, PropertyChangedEventHandler PropertyChanged) where T : _ItemBase, new()
        {
            var nonInvItems = DataCache.GetNonInventoryItemList();
            var posItems = QbPosInventory.Items.Concat(nonInvItems);
            var purchaseInfo = DataCache.GetPurchaseInfoList();

            var items = purchaseList
                .Select(i =>
                {
                    T item = new T();
                    item.Purchase = i;
                    item.PosItem = posItems.FirstOrDefault(pos => pos.ItemNumber == i.ItemNumber);
                    SetSalesInfo(ref item);
                    SetPurchaseInfo(ref item, purchaseInfo);
                    return item;
                });
            return items.ToList();
        }

        public static string GetUniquePackingNumber(string packingNumber)
        {
            using (var context = new PurchaseRequestModel())
            {
                context.Database.Log = x => System.Diagnostics.Debug.WriteLine(x);
                var matches = (from pk in context.packing
                               where pk.PackingNumber.StartsWith(packingNumber)
                               select pk.PackingNumber)
                               .ToList();
                if (matches.Count > 0)
                {
                    int next = 0;
                    if (matches.Contains(packingNumber) && matches.Count == 1)
                    {
                        //do nothing
                    }
                    else
                    {
                        next = matches
                            .Where(m => m.StartsWith(packingNumber + "-"))
                            .Select(m =>
                            {
                                string result = m.Substring((packingNumber + "-").Length);
                                int num = -1;
                                int.TryParse(result, out num);
                                return num;
                            })
                            .Max();
                    }
                    next += 1;
                    packingNumber += "-" + next;
                }
                return packingNumber;
            }
        }
    }
}
