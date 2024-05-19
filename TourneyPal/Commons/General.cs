using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public enum TournamentSiteHost
        {
            [Description("https://www.start.gg/")]
            Start = 1,
            [Description("https://challonge.com/")]
            Challonge = 2
        }
    }
}
