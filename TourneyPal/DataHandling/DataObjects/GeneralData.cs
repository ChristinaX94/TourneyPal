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
