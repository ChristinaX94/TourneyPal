using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;
using TourneyPal.Data.Commons;
using TourneyPal.DataHandling;
using TourneyPal.DataHandling.DataObjects;

namespace TourneyPal.Service
{
    public class TourneyPalService: ITourneyPalService
    {
        public string Ping() 
        {
            return "Pong from service!";
        }

        public void InitializeData()
        {
            GeneralData.GeneralDataInitialize();
        }

        public void Log(MethodBase? foundInItem, string? messageItem = null, string? exceptionMessageItem = null)
        {
            Logger.log(foundInItem, messageItem, exceptionMessageItem);
        }

        public async Task RunApiHandler()
        {
            await Task.WhenAll(ApiHandler.runAsync());
        }

        public async Task SearchChallongeEntry(string url)
        {
            await ApiHandler.examineSingleChallongeRequest(url);
        }

        public List<TournamentData> getNewTournaments()
        {
            return GeneralData.TournamentsData.Where(x => x.StartsAT >= Common.getDate()).ToList();
        }

        public List<TournamentData> getOldTournaments()
        {
            return GeneralData.TournamentsData.Where(x => x.StartsAT < Common.getDate()).ToList();
        }

        public List<TournamentData> getAllTournaments()
        {
            return GeneralData.TournamentsData.ToList();
        }

        public List<TournamentData> getNewTournamentsByCountryCode(string countryCode)
        {
            return GeneralData.TournamentsData.Where(x => x.CountryCode.Equals(countryCode) && x.StartsAT >= Common.getDate()).ToList();
        }

        public async Task<TournamentData?> getTournamentByURL(string url)
        {
            var tournamentFound = GeneralData.TournamentsData.FirstOrDefault(x => x.URL.Equals(url));
            if (tournamentFound == null) 
            {
                await SearchChallongeEntry(url);
                tournamentFound = GeneralData.TournamentsData.FirstOrDefault(x => x.URL.Equals(url));
            }

            return tournamentFound;
        }
    }
}
