using System.Reflection;
using TourneyPal.Commons.DataObjects;
using static TourneyPal.Common;

namespace TourneyPal.DataService
{
    public interface ITourneyPalDataService
    {
        public string Ping();
        public void InitializeData();
        public void Log(MethodBase? foundInItem, string? messageItem = null, string? exceptionMessageItem = null);
        public Task RunApiHandler();
        public Task SearchChallongeEntry(string url);
        public TournamentData? getTournamentByName(Game SelectedGame, string name);
        public List<TournamentData> getNewTournaments(Game SelectedGame);
        public List<TournamentData> getOldTournaments(Game SelectedGame);
        public List<TournamentData> getAllTournaments(Game SelectedGame);
        public List<TournamentData> getNewlyAddedTournaments(Game SelectedGame);
        public List<TournamentData> getNewTournamentsByCountryCode(Game SelectedGame, string countryCode);
        public List<TournamentData> searchTournaments(Game SelectedGame, string term);
        public Task<TournamentData?> getChallongeTournamentByURL(string url);
    }
}
