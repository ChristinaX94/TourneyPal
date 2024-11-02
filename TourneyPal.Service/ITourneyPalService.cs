using System.Reflection;
using TourneyPal.Commons.DataObjects;

namespace TourneyPal.Service
{
    public interface ITourneyPalService
    {
        public string Ping();
        public void InitializeData();
        public void Log(MethodBase? foundInItem, string? messageItem = null, string? exceptionMessageItem = null);
        public Task RunApiHandler();
        public Task SearchChallongeEntry(string url);
        public TournamentData? getTournamentByName(string name);
        public List<TournamentData> getNewTournaments();
        public List<TournamentData> getOldTournaments();
        public List<TournamentData> getAllTournaments();
        public List<TournamentData> getNewlyAddedTournaments();
        public List<TournamentData> getNewTournamentsByCountryCode(string countryCode);
        public List<TournamentData> searchTournaments(string term);
        public Task<TournamentData?> getChallongeTournamentByURL(string url);
    }
}
