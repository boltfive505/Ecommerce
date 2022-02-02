using System;
using System.ComponentModel;

namespace Egate_Ecommerce
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum TutorialPeriodType
    {
        [Description("N/A")]
        None,
        Daily,
        Weekly,
        Monthly
    }
}
