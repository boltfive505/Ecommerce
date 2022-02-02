using System;
using System.ComponentModel;

namespace Egate_Ecommerce
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ShipType
    {
        [Description("By Sea")]
        BySea,
        [Description("By Airship")]
        ByAirship
    }
}
