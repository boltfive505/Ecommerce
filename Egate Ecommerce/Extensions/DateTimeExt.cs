using System;

namespace Egate_Ecommerce
{
    public static class DateTimeExt
    {
        private static DateTime _unixDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);

        public static DateTime ToUnixDate(this long value)
        {
            return new DateTime(_unixDate.Year, _unixDate.Month, _unixDate.Day).AddSeconds(value);
        }

        public static long ToUnixLong(this DateTime value)
        {
            if (_unixDate > value)
                throw new ArgumentException("DateTime value is older that Unix Time (January 1, 1970)");
            return (long)(value - _unixDate).TotalSeconds;
        }

        public static DateTime? ToUnixDate(this long? value)
        {
            if (value == null) return null;
            return (DateTime?)ToUnixDate((long)value);
        }

        public static long? ToUnixLong(this DateTime? value)
        {
            if (value == null) return null;
            return (long?)ToUnixLong((DateTime)value);
        }
    }
}
