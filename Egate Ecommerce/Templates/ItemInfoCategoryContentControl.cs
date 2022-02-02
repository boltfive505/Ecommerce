using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Egate_Ecommerce.Templates
{
    public class ItemInfoCategoryContentControl : ContentControl
    {
        public static readonly DependencyProperty CategoryProperty = DependencyProperty.Register(nameof(Category), typeof(ItemInfoCategory), typeof(ItemInfoCategoryContentControl), new FrameworkPropertyMetadata(ItemInfoCategory.Competitor_Price, new PropertyChangedCallback(OnCategoryPropertyChanged)));
        public ItemInfoCategory Category
        {
            get { return (ItemInfoCategory)GetValue(CategoryProperty); }
            set { SetValue(CategoryProperty, value); }
        }

        public static readonly DependencyProperty SuffixKeyProperty = DependencyProperty.Register(nameof(SuffixKey), typeof(string), typeof(ItemInfoCategoryContentControl));
        public string SuffixKey
        {
            get { return (string)GetValue(SuffixKeyProperty); }
            set { SetValue(SuffixKeyProperty, value); }
        }

        //public DataTemplate CompetitorPriceTemplate { get; set; }
        //public DataTemplate FaqTemplate { get; set; }
        //public DataTemplate ProductInfoTemplate { get; set; }
        //public DataTemplate QuestionsAnswersTemplate { get; set; }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            SetTemplate();
        }

        private static void OnCategoryPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (obj is ItemInfoCategoryContentControl)
            {
                ItemInfoCategoryContentControl control = obj as ItemInfoCategoryContentControl;
                control.SetTemplate();
            }
        }

        private void SetTemplate()
        {
            ContentTemplate = (DataTemplate)Application.Current.FindResource(Category.ToString() + "_" + SuffixKey);
            //switch (Category)
            //{
            //    case ItemInfoCategory.Competitor_Price:
            //        ContentTemplate = CompetitorPriceTemplate;
            //        break;
            //    case ItemInfoCategory.Faq:
            //        ContentTemplate = FaqTemplate;
            //        break;
            //    case ItemInfoCategory.Product_Information:
            //        ContentTemplate = ProductInfoTemplate;
            //        break;
            //    case ItemInfoCategory.Questions_Answers:
            //        ContentTemplate = QuestionsAnswersTemplate;
            //        break;
            //}
        }
    }
}
