using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Egate_Ecommerce.Objects;
using Egate_Ecommerce.Quickbooks;
using ECommerce.Model;

namespace Egate_Ecommerce.Classes
{
    public static class ECommerceHelper
    {
        public static async Task<IEnumerable<ItemInfoViewModel>> GetItemInfoListAsync()
        {
            using (var context = new ECommerceModel())
            {
                var query = from info in context.item_info
                            join emp in context.employee on info.UpdatedByEmployeeId equals emp.Id into tempEmp
                            from info_emp in tempEmp.DefaultIfEmpty()
                            select new { ItemInfo = info, UpdatedEmployee = info_emp };
                var list = await query.ToListAsync();
                return list.Select(i => new ItemInfoViewModel(i.ItemInfo, i.UpdatedEmployee));
            }
        }

        public static async Task<IEnumerable<ItemInfoViewModel>> GetItemInfoListByItemNumberAsync(string itemNumber)
        {
            using (var context = new ECommerceModel())
            {
                var query = from info in context.item_info
                            join emp in context.employee on info.UpdatedByEmployeeId equals emp.Id into tempEmp
                            from info_emp in tempEmp.DefaultIfEmpty()
                            where info.ItemNumber == itemNumber
                            select new { ItemInfo = info, UpdatedEmployee = info_emp };
                var list = await query.ToListAsync();
                return list.Select(i => new ItemInfoViewModel(i.ItemInfo, i.UpdatedEmployee));
            }
        }

        public static async Task<IEnumerable<EmployeeViewModel>> GetEmployeeListAsync(bool includeInactive = false)
        {
            using (var context = new ECommerceModel())
            {
                var query = from emp in context.employee
                            where includeInactive ? true : emp.IsActive == 1
                            orderby emp.EmployeeName ascending
                            select emp;
                var list = await query.ToListAsync();
                return list.Select(i => new EmployeeViewModel(i));
            }
        }

        public static async Task<IEnumerable<ItemDetailsViewModel>> GetItemDetailsAsync()
        {
            using (var context = new ECommerceModel())
            {
                var list = await context.item_details.ToListAsync();
                return list.Select(i => new ItemDetailsViewModel(i));
            }
        }

        public static async Task<IEnumerable<ItemKeywordViewModel>> GetItemKeywordListAsync()
        {
            using (var context = new ECommerceModel())
            {
                var list = await context.item_keyword.ToListAsync();
                return list.Select(i => new ItemKeywordViewModel(i));
            }
        }

        public static async Task AddItemInfoAsync(ItemInfoViewModel itemVm)
        {
            using (var context = new ECommerceModel())
            {
                var item = await context.item_info.FirstOrDefaultAsync(i => i.Id == itemVm.Id);
                if (item == null)
                {
                    item = new item_info();
                    context.item_info.Add(item);
                }
                item.ItemNumber = itemVm.ItemNumber;
                item.Category = itemVm.Category.ToString();
                item.UpdatedDate = itemVm.UpdatedDate.ToUnixLong();
                item.UpdatedByEmployeeId = itemVm.UpdatedByEmployee?.Id;
                item.ShortDescription = itemVm.ShortDescription;
                item.LongDescription = itemVm.LongDescription;
                item.CompetitorUrl = itemVm.CompetitorUrl;
                item.CompetitorPrice = itemVm.CompetitorPrice;
                item.CompetitorLocation = itemVm.CompetitorLocation.ToString();
                item.CompetitorRatings = itemVm.CompetitorRatings;
                item.CompetitorHasStocks = (int)itemVm.CompetitorHasStocks.ToLong();

                await context.SaveChangesAsync();
                itemVm.Id = item.Id;
            }
        }

        public static async Task DeleteItemInfoAsync(ItemInfoViewModel itemVm)
        {
            using (var context = new ECommerceModel())
            {
                var item = await context.item_info.FirstOrDefaultAsync(i => i.Id == itemVm.Id);
                if (item != null)
                {
                    context.item_info.Remove(item);
                    await context.SaveChangesAsync();
                }
            }
        }

        public static async Task AddEmployeeAsync(EmployeeViewModel employeeVm)
        {
            using (var context = new ECommerceModel())
            {
                var employee = await context.employee.FirstOrDefaultAsync(i => i.Id == employeeVm.Id);
                if (employee == null)
                {
                    employee = new employee();
                    context.employee.Add(employee);
                }
                employee.EmployeeName = employeeVm.EmployeeName;
                employee.IsActive = employeeVm.IsActive.ToLong();

                await context.SaveChangesAsync();
                employeeVm.Id = employee.Id;
            }
        }

        public static async Task AddItemDetailsAsync(ItemDetailsViewModel detailsVm)
        {
            using (var context = new ECommerceModel())
            {
                //use item number to search -> to make it unique
                //so dont allow details view model with blank item number
                if (string.IsNullOrEmpty(detailsVm.ItemNumber))
                {
                    throw new ArgumentNullException("(ItemDetailsViewModel) ItemNumber is blank");
                }
                var details = await context.item_details.FirstOrDefaultAsync(i => i.ItemNumber == detailsVm.ItemNumber);
                if (details == null)
                {
                    details = new item_details();
                    details.ItemNumber = detailsVm.ItemNumber;
                    context.item_details.Add(details);
                }
                details.Memo = detailsVm.Memo;
                details.MemoUpdatedDate = detailsVm.MemoUpdatedDate.ToUnixLong();
                details.ForUpdate = detailsVm.ForUpdate.ToLong();

                await context.SaveChangesAsync();
                detailsVm.Id = details.Id;
            }
        }

        public static async Task UpdateAllItemDetailsAsync(IEnumerable<ItemDetailsViewModel> detailsVmList)
        {
            //this will only update existing item details -> no adding
            using (var context = new ECommerceModel())
            {
                //use item number to search -> to make it unique
                //so dont allow details view model with blank item number
                foreach (var detailsVm in detailsVmList)
                {
                    if (string.IsNullOrEmpty(detailsVm.ItemNumber))
                        continue;
                    var details = await context.item_details.FirstOrDefaultAsync(i => i.ItemNumber == detailsVm.ItemNumber);
                    if (details != null)
                    {
                        details.Memo = detailsVm.Memo;
                        details.MemoUpdatedDate = detailsVm.MemoUpdatedDate.ToUnixLong();
                    }
                }
                
                await context.SaveChangesAsync();
            }
        }

        public static async Task AddItemKeywordAsync(ItemKeywordViewModel keywordVm)
        {
            using (var context = new ECommerceModel())
            {
                if (string.IsNullOrEmpty(keywordVm.ItemNumber)) return;
                //use item number for searching
                var keyword = await context.item_keyword.FirstOrDefaultAsync(i => i.ItemNumber == keywordVm.ItemNumber);
                if (keyword == null)
                {
                    keyword = new item_keyword();
                    keyword.ItemNumber = keywordVm.ItemNumber;
                    context.item_keyword.Add(keyword);
                }
                keyword.Keywords = keywordVm.Keywords;
                keyword.SuggestedName = keywordVm.SuggestedName;
                keyword.UpdatedDate = keywordVm.UpdatedDate.ToUnixLong();

                await context.SaveChangesAsync();
                keywordVm.Id = keyword.Id;
            }
        }
    }
}
