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
        public List<TournamentData> getNewTournaments();
        public List<TournamentData> getOldTournaments();
        public List<TournamentData> getAllTournaments();
        public List<TournamentData> getNewTournamentsByCountryCode(string countryCode);
        public Task<TournamentData?> getTournamentByURL(string url);
    }
}
