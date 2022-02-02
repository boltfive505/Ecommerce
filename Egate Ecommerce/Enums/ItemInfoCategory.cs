using System;
using System.ComponentModel;

namespace Egate_Ecommerce
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ItemInfoCategory
    {
        [Description("")]
        All,
        [Description("Competitor Price")]
        Competitor_Price,
        [Description("FAQ")]
        Faq,
        [Description("Product Info")]
        Product_Information,
        [Description("Q&A")]
        Questions_Answers,
        Issue,
        Suggestion
    }
}
