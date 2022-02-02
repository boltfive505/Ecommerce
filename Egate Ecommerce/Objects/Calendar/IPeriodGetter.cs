using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egate_Ecommerce.Objects.Calendar
{
    public interface IPeriodGetter
    {
        IEnumerable<DateTime> GetPeriodDates(int year, int month);
    }
}
