using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Egate_Ecommerce.Objects;

namespace Egate_Ecommerce.Classes
{
    public class ItemInfoCategoryTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CompetitorPriceTemplate { get; set; }
        public DataTemplate FaqTemplate { get; set; }
        public DataTemplate ProductInformationTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            ItemInfoViewModel itemInfo = item as ItemInfoViewModel;
            if (itemInfo != null)
            {
                switch (itemInfo.Category)
                {
                    case ItemInfoCategory.Competitor_Price: return CompetitorPriceTemplate;
                    case ItemInfoCategory.Faq: return FaqTemplate;
                    case ItemInfoCategory.Product_Information: return ProductInformationTemplate;
                }
            }
            return base.SelectTemplate(item, container);
        }
    }
}
