using System;
using System.Collections.Generic;
using System.Linq;

namespace Egate_Ecommerce
{
    public static class EnumExt
    {
        public static TEnum ToEnum<TEnum>(this string value) where TEnum : struct
        {
            TEnum e = default(TEnum);
            Enum.TryParse(value, out e);
            return e;
        }

        public static TEnum ToEnum<TEnum>(this string value, TEnum defaultValue) where TEnum : struct
        {
            TEnum e = default(TEnum);
            bool parse = Enum.TryParse(value, out e);
            if (parse)
                return e;
            else
                return defaultValue;
            
        }

        public static IEnumerable<object> GetValues(Type enumType, params object[] exceptions)
        {
            return Enum.GetValues(enumType).OfType<object>().Except(exceptions);
        }
    }
}
