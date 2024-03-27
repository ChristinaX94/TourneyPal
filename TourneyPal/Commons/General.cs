using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourneyPal.GeneralData
{
    public static class General
    {
        public enum YesNo
        {
            No = 0,
            Yes = 1
        }

        public static DateTime? getDate()
        {
            return DateTime.Now;
        }
    }
}
