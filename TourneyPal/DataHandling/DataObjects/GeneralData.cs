using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourneyPal.DataHandling.DataObjects
{
    public static class GeneralData
    {
        public enum YesNo
        {
            No = 0,
            Yes = 1
        }

        public enum TournamentSiteHost
        {
            [Description("https://www.start.gg/")]
            Start = 0,
            [Description("https://challonge.com/")]
            Challonge = 1
        }

        public static List<Tournament> Tournaments { get; private set; }

        public static void GeneralDataInitialize()
        {
            Tournaments = new List<Tournament>();
        }

        public static void addTournament(Tournament tournament)
        {
            try
            {
                if(Tournaments.Any(x=>x.ID == tournament.ID))
                {
                    return;
                }
                Tournaments.Add(tournament);
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex.Message);
            }
        }
    }
}
