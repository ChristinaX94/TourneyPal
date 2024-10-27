using System.Reflection;
using TourneyPal.Api;
using TourneyPal.Commons;
using TourneyPal.Commons.DataObjects;
using TourneyPal.DataAccessLayer.DataHandling;

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

            CallApiAndSave();
        }

        private async Task CallApiAndSave()
        {
            var response = await ApiHandler.examineAllDataAsync(GeneralData.GetChallongeTournamentUrls());
            if (!response.Success)
            {
                Log(response.Error.FoundInItem, response.Error.MessageItem, response.Error.ExceptionMessageItem);
            }
            else
            {
                GeneralData.SaveFindings(response.Tournaments, response.Requests);
            }
        }

        public async Task RunApiHandler()
        {
            await Task.WhenAll(RunAsync());
        }

        private async Task RunAsync()
        {
            try
            {
                var timer = new PeriodicTimer(TimeSpan.FromHours(1));
                while (await timer.WaitForNextTickAsync())
                {
                    if (Common.getDate().Hour != Common.TimeOfDayRefreshData)
                    {
                        continue;
                    }

                    CallApiAndSave();
                }
            }
            catch (Exception ex)
            {
                Log(foundInItem: MethodBase.GetCurrentMethod(),
                    exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        public async Task SearchChallongeEntry(string url)
        {
            var response = await ApiHandler.examineSingleChallongeRequest(url);
            if (!response.Success)
            {
                Log(response.Error.FoundInItem, response.Error.MessageItem, response.Error.ExceptionMessageItem);
            }
            else
            {
                GeneralData.SaveFindings(response.Tournaments, response.Requests);
            }
        }

        public void Log(MethodBase? foundInItem, string? messageItem = null, string? exceptionMessageItem = null)
        {
            Logger.log(foundInItem, messageItem, exceptionMessageItem);
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
