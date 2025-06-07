using System.Reflection;
using TourneyPal.Api;
using TourneyPal.Commons;
using TourneyPal.Commons.DataObjects;
using TourneyPal.DataAccessLayer.DataHandling;
using static TourneyPal.Commons.Common;

namespace TourneyPal.DataService
{
    public class TourneyPalDataService: ITourneyPalDataService
    {
        public string Ping() 
        {
            return "Pong from service!";
        }

        public void InitializeData()
        {
            GeneralData.GeneralDataInitialize();

           _ = CallApiAndSave();
        }

        private async Task CallApiAndSave()
        {
            var response = await ApiHandler.examineAllDataAsync(GeneralData.GetStartGGGameIDs(),GeneralData.GetChallongeTournamentUrls());
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

                    _ = CallApiAndSave();
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

        public TournamentData? getTournamentByName(Game SelectedGame, string name)
        {
            return GeneralData.TournamentsData.FirstOrDefault(x => x.GameEnum == SelectedGame && x.Name.Equals(name));
        }

        public List<TournamentData> getNewTournaments(Game SelectedGame)
        {
            return GeneralData.TournamentsData.Where(x => x.GameEnum == SelectedGame && x.StartsAT >= Common.getDate()).ToList();
        }

        public List<TournamentData> getOldTournaments(Game SelectedGame)
        {
            return GeneralData.TournamentsData.Where(x => x.GameEnum == SelectedGame && x.StartsAT < Common.getDate()).ToList();
        }

        public List<TournamentData> getAllTournaments(Game SelectedGame)
        {
            return GeneralData.TournamentsData.Where(x=> x.GameEnum == SelectedGame).ToList();
        }

        public List<TournamentData> getNewlyAddedTournaments(List<Game> SelectedGames)
        {
            return GeneralData.NewlyAddedTournamentsData.Where(x => SelectedGames.Contains(x.GameEnum)).OrderBy(x=>x.GameEnum).ToList();
        }

        public List<TournamentData> getNewTournamentsByCountryCode(Game SelectedGame, string countryCode)
        {
            return GeneralData.TournamentsData.Where(x => x.GameEnum == SelectedGame && x.CountryCode.Equals(countryCode) && x.StartsAT >= Common.getDate()).ToList();
        }

        public List<TournamentData> searchTournaments(Game SelectedGame, string term)
        {
            return GeneralData.TournamentsData.Where(x => x.GameEnum == SelectedGame && x.Name.ToLower().Contains(term.ToLower()) && x.StartsAT >= Common.getDate()).ToList();
        }

        public async Task<TournamentData?> getChallongeTournamentByURL(string url)
        {
            await SearchChallongeEntry(url);
            return GeneralData.TournamentsData.FirstOrDefault(x => x.URL.Equals(url));
        }
    }
}
