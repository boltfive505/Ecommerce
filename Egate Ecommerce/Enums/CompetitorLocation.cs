using System;
using System.ComponentModel;

namespace Egate_Ecommerce
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum CompetitorLocation
    {
        [Description("")]
        None,
        NCR,
        Province,
        China,
        Korea,
        Others
    }
}
