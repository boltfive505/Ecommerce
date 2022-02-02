using System;
using System.ComponentModel;

namespace Egate_Ecommerce
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum EntryLevel
    {
        Beginner,
        Intermediate,
        Advanced,
        Expert
    }
}
