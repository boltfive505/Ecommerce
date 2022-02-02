using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using purchase_request.Model;
using Egate_Ecommerce.Quickbooks;
using Egate_Ecommerce.Objects;
using System.IO;

namespace Egate_Ecommerce.Classes
{
    public static class DataCache
    {
        public static IEnumerable<PurchaseInfo> GetPurchaseInfoList()
        {
            using (var context = new PurchaseRequestModel())
            {
                var purchaseList = (from p in context.purchase
                                    where p.ArrivalDate == null
                                    select new
                                    {
                                        p.ItemNumber,
                                        p.PurchaseQuantity,
                                        p.RequestDate,
                                        p.ApprovedDate,
                                        p.ReceivedDate,
                                        p.ShippedDate,
                                        p.DeliveryDate
                                    });
                return purchaseList
                    .AsNoTracking()
                    .ToList()
                    .GroupBy(p => p.ItemNumber)
                    .Select(g => new PurchaseInfo()
                    {
                        ItemNumber = g.Key,
                        RequestedQty = g.Where(p => p.RequestDate != null && p.ApprovedDate == null).Sum(p => p.PurchaseQuantity),
                        PreparedQty = g.Where(p => p.ApprovedDate != null && p.ReceivedDate == null).Sum(p => p.PurchaseQuantity),
                        PackedQty = g.Where(p => p.ReceivedDate != null && p.ShippedDate == null).Sum(p => p.PurchaseQuantity),
                        ShippedQty = g.Where(p => p.ShippedDate != null && p.DeliveryDate == null).Sum(p => p.PurchaseQuantity)
                    });
            }
        }

        public static IEnumerable<PosItem> GetNonInventoryItemList()
        {
            using (var context = new PurchaseRequestModel())
            {
                return (from i in context.noninventory_items.AsNoTracking()
                        where i.Active == true
                        select i)
                                         .ToList()
                                         .Select(i => new PosItem()
                                         {
                                             ItemNumber = i.ItemNumber,
                                             ItemName = i.ItemName,
                                             ItemDescription = i.ItemDescription,
                                             DepartmentName = string.Empty,
                                             DepartmentCode = "000",
                                             ALU = string.Empty,
                                             Quantity = 0,
                                             UnitCost = 0,
                                             RegularPrice = 0,
                                             ItemType = string.Empty,
                                             ReorderPoint = null,
                                             PartNumber = i.PartsNumber,
                                             KoreanName = i.KoreanName
                                         });
            }
        }

        public static IEnumerable<PosItem> GetNonInventoryItemList2()
        {
            using (var context = new PurchaseRequestModel())
            {
                return (from i in context.noninventory_items.AsNoTracking()
                        where i.Active == true
                        select i)
                                         .ToList()
                                         .Select(i => new PosItem()
                                         {
                                             ItemNumber = (i.Id * -1).ToString(),
                                             ItemName = i.ItemName,
                                             ItemDescription = i.ItemDescription,
                                             DepartmentCode = "000",
                                             ItemType = string.Empty
                                         });
            }
        }

        public static IEnumerable<NonInventoryViewModel> GetNonInventoryList()
        {
            using (var context = new PurchaseRequestModel())
            {
                var list = context.noninventory_items.ToList();
                return list.Select(i => new NonInventoryViewModel(i));
            }
        }

        public static IEnumerable<lucky> GetLuckyItemList()
        {
            using (var context = new PurchaseRequestModel())
            {
                return context.lucky.AsNoTracking().ToList();
            }
        }

        public static IEnumerable<LuckyPurchaseItem> GetLuckyPurchaseList()
        {
            using (var context = new PurchaseRequestModel())
            {
                return (from p in context.purchase
                                          where p.LuckyId != null
                                          select new
                                          {
                                              p.ItemNumber,
                                              p.PurchaseQuantity,
                                              p.lucky.ShippingLabel,
                                              p.packing.PackingNumber,
                                              p.BoxLabel,
                                              p.packing.ShipType,
                                              p.packing.delivery_receipt.ReceiptNumber
                                          })
                                                .AsNoTracking()
                                                .ToList()
                                                .Select(i => new LuckyPurchaseItem()
                                                {
                                                    ItemNumber = i.ItemNumber,
                                                    Quantity = i.PurchaseQuantity,
                                                    ShippingLabel = i.ShippingLabel,
                                                    PackingNumber = i.PackingNumber,
                                                    BoxLabel = i.BoxLabel,
                                                    ShipType = i.ShipType.ToEnum<ShipType>(),
                                                    DeliveryReceiptNumber = i.ReceiptNumber
                                                });
            }
        }

        public static IEnumerable<item_details> GetItemDetailsList()
        {
            using (var context = new PurchaseRequestModel())
            {
                return context.item_details.ToList();
            }
        }

        public static async Task UpdateItemDetails(_ItemBase item)
        {
            using (var context = new PurchaseRequestModel())
            {
                var details = await context.item_details.FirstOrDefaultAsync(i => i.ItemNumber == item.PosItem.ItemNumber);
                if (details == null)
                {
                    details = new item_details();
                    details.ItemNumber = item.PosItem.ItemNumber;
                    context.item_details.Add(details);
                }
                details.ReorderQuantity = item.ReorderQuantity;

                await context.SaveChangesAsync();
            }
        }

        public static async Task AddNonInventoryItemAsync(NonInventoryViewModel nonInventoryVm)
        {
            using (var context = new PurchaseRequestModel())
            {
                var item = await context.noninventory_items.FirstOrDefaultAsync(i => i.Id == nonInventoryVm.Id);
                if (item == null)
                {
                    item = new noninventory_items();
                    context.noninventory_items.Add(item);
                }
                item.ItemNumber = "0";
                item.ItemName = nonInventoryVm.ItemName;
                item.ItemDescription = nonInventoryVm.ItemDescription;
                await context.SaveChangesAsync();

                nonInventoryVm.Id = item.Id;
                //save image
                if (nonInventoryVm != null && File.Exists(nonInventoryVm.ImagePath))
                {
                    Helpers.SaveImageFileAsJpg(nonInventoryVm.ImagePath, FileHelper.GetFile(nonInventoryVm.ItemNumber + ".jpg", @"uploads\non-inventory items"));
                    nonInventoryVm.GetImagePath();
                }
            }
        }
    }
}
