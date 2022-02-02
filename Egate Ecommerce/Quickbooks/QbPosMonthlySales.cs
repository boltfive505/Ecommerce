using System;
using System.IO;
using System.Reflection;

namespace Egate_Ecommerce.Quickbooks
{
    public static class QbPosMonthlySales
    {
        private static PosMonthlySalesPeriod[] _monthlySales = new PosMonthlySalesPeriod[3];

        public static PosMonthlySalesPeriod MonthlySalesReport_1 { get { return _monthlySales[0]; } }
        public static PosMonthlySalesPeriod MonthlySalesReport_3 { get { return _monthlySales[1]; } }
        public static PosMonthlySalesPeriod MonthlySalesReport_6 { get { return _monthlySales[2]; } }
        
        public static DateTime? LatestSalesDate_1 { get { return MonthlySalesReport_1?.ToSalesDate; } }
        public static DateTime? LatestSalesDate_3 { get { return MonthlySalesReport_3?.ToSalesDate; } }
        public static DateTime? LatestSalesDate_6 { get { return MonthlySalesReport_6?.ToSalesDate; } }

        public static void Load()
        {
            string dir = @"..\qb\monthly sales report"; //Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "qb", "monthly sales report");
            _monthlySales = new PosMonthlySalesPeriod[]
            {
                new PosMonthlySalesPeriod(dir, "monthly sales report_1"),
                new PosMonthlySalesPeriod(dir, "monthly sales report_3"),
                new PosMonthlySalesPeriod(dir, "monthly sales report_6")
            };
        }
    }
}
