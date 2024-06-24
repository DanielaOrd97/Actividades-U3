using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace U3Cliente.Helpers
{
    public static class ZonaHoraria
    {
        public static DateTime ToMexicoTime(this DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTime, "Central America Standard Time");
        }
    }
}
