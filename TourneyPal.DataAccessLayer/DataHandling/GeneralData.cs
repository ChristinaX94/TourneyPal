using TourneyPal.SQLManager.DataModels.SQLTables.Game;
using TourneyPal.SQLManager;
using TourneyPal.SQLManager.DataModels.SQLTables.Tournament;
using TourneyPal.SQLManager.DataModels.SQLTables.Stream;
using Stream = TourneyPal.SQLManager.DataModels.SQLTables.Stream.Stream;
using TourneyPal.SQLManager.DataModels.SQLTables.Tournament_Host_Sites;
using Tournament = TourneyPal.SQLManager.DataModels.SQLTables.Tournament.Tournament;
using TourneyPal.SQLManager.DataModels.SQLTables.Tournament_Api_Data;
using EnumsNET;
using System.Reflection;
using TourneyPal.Commons;
using TourneyPal.Commons.DataObjects;
using TourneyPal.Commons.DataObjects.ApiResponseModels;
using TourneyPal.SQLManager.DataModels.SQLTables.Game_On_Tournament_Host_Site;

namespace TourneyPal.DataAccessLayer.DataHandling
{
    public static class GeneralData
    {
        private static Tournament Tournaments { get; set; } = new Tournament();
        private static Stream Streams { get; set; } = new Stream();
        private static Game Games { get; set; } = new Game();
        private static Tournament_Host_Sites TournamentHostSites { get; set; } = new Tournament_Host_Sites();
        private static Game_On_Tournament_Host_Site Games_On_Tournament_Host_Site { get; set; } = new Game_On_Tournament_Host_Site();
        public static List<TournamentData> TournamentsData { get; private set; } = new List<TournamentData>();
        public static List<TournamentData> NewlyAddedTournamentsData { get; private set; } = new List<TournamentData>();

        public static void GeneralDataInitialize()
        {
            try
            {
                InitializeInternalDBObjects();

                foreach (TournamentRow tournament in Tournaments.Rows)
                {
                    TournamentsData.Add(new TournamentData()
                    {
                        ID = tournament.Tournament_ID,
                        Name = tournament.Name,
                        CountryCode = tournament.CountryCode,
                        City = tournament.City,
                        AddrState = tournament.AddrState,
                        StartsAT = tournament.StartsAT,
                        Online = tournament.Online,
                        URL = tournament.URL,
                        State = tournament.State,
                        VenueAddress = tournament.VenueAddress,
                        VenueName = tournament.VenueName,
                        RegistrationOpen = tournament.RegistrationOpen,
                        NumberOfEntrants = tournament.NumberOfEntrants == null ? 0 : (int)tournament.NumberOfEntrants,
                        GameEnum = (Common.Game)Games.Rows.Where(x => x.ID == tournament.Game_ID)?.Select(y => ((GameRow)y).ID).FirstOrDefault(),
                        Streams = Streams.Rows.Where(x => ((StreamRow)x).Tournament_ID == tournament.ID)?.Select(y => ((StreamRow)y).Title).ToList(),
                        HostSite = TournamentHostSites.Rows.Where(x => x.ID == tournament.HostSite_ID)?.Select(y => ((Tournament_Host_SitesRow)y).Site).FirstOrDefault(),
                    });
                }
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        private static void InitializeInternalDBObjects()
        {
            try
            {
                Tournaments = (Tournament)SQLHandler.loadModelData(new Tournament());
                Streams = (Stream)SQLHandler.loadModelData(new Stream());
                Games = (Game)SQLHandler.loadModelData(new Game());
                TournamentHostSites = (Tournament_Host_Sites)SQLHandler.loadModelData(new Tournament_Host_Sites());
                Games_On_Tournament_Host_Site = (Game_On_Tournament_Host_Site)SQLHandler.loadModelData(new Game_On_Tournament_Host_Site());
                TournamentsData = new List<TournamentData>();
                NewlyAddedTournamentsData = new List<TournamentData>();
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        public static void SaveFindings(List<TournamentData> tournaments, List<ApiRequestedDataHandler> requests)
        {
            try
            {
                AddTournaments(tournaments);
                SaveTournaments();
                SaveApiRequestedData(requests);
                SortTournaments();
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        private static void AddTournaments(List<TournamentData> tournaments)
        {
            try
            {
                NewlyAddedTournamentsData = new List<TournamentData>();
                foreach (var tournament in tournaments)
                {
                    var existingTournament = TournamentsData.FirstOrDefault(x => x.ID == tournament.ID && x.GameEnum == tournament.GameEnum);
                    if (existingTournament == null)
                    {
                        tournament.isModified = true;
                        TournamentsData.Add(tournament);
                        NewlyAddedTournamentsData.Add(tournament);
                        continue;
                    }

                    var tournamentEdited = !(existingTournament.Online == tournament.Online &&
                                            existingTournament.StartsAT == tournament.StartsAT &&
                                            existingTournament.NumberOfEntrants == tournament.NumberOfEntrants &&
                                            existingTournament.VenueAddress.Equals(tournament.VenueAddress));
                    if (tournamentEdited)
                    {
                        existingTournament.Online = tournament.Online;
                        existingTournament.StartsAT = tournament.StartsAT;
                        existingTournament.NumberOfEntrants = tournament.NumberOfEntrants;
                        existingTournament.VenueAddress = tournament.VenueAddress;
                        existingTournament.isModified = true;
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }
        private static void SaveTournaments()
        {
            try
            {
                var listToSave = TournamentsData.Where(x => x.isModified).ToList();
                if (listToSave == null || listToSave.Count == 0)
                {
                    return;
                }

                foreach (var item in listToSave)
                {
                    TournamentRow tournamentDataRow = (TournamentRow)Tournaments.Rows.FirstOrDefault(x => ((TournamentRow)x).Tournament_ID == item.ID);

                    if (tournamentDataRow == null)
                    {
                        tournamentDataRow = new TournamentRow(nameof(Tournaments));
                        tournamentDataRow.insertNewRowData();
                        Tournaments.Rows.Add(tournamentDataRow);
                    }
                    else
                    {
                        tournamentDataRow.updateRowData();
                    }

                    tournamentDataRow.HostSite_ID = TournamentHostSites.Rows.Where(x => ((Tournament_Host_SitesRow)x).Site.Equals(item.HostSite))?.Select(y => y.ID).FirstOrDefault();
                    tournamentDataRow.Tournament_ID = item.ID;
                    tournamentDataRow.Name = item.Name;
                    tournamentDataRow.CountryCode = item.CountryCode;
                    tournamentDataRow.City = item.City;
                    tournamentDataRow.AddrState = item.AddrState;
                    tournamentDataRow.StartsAT = item.StartsAT;
                    tournamentDataRow.Online = item.Online;
                    tournamentDataRow.URL = item.URL;
                    tournamentDataRow.State = item.State;
                    tournamentDataRow.VenueAddress = item.VenueAddress;
                    tournamentDataRow.VenueName = item.VenueName;
                    tournamentDataRow.RegistrationOpen = item.RegistrationOpen;
                    tournamentDataRow.NumberOfEntrants = item.NumberOfEntrants;
                    tournamentDataRow.Game_ID = Games.Rows.Where(x => ((GameRow)x).Title.Equals(item.Game))?.Select(y => y.ID).FirstOrDefault();
                    tournamentDataRow.isModified = true;

                    foreach (var streamItem in item.Streams)
                    {
                        StreamRow streamRow = (StreamRow)Streams.Rows.FirstOrDefault(x => ((StreamRow)x).Tournament_ID == item.ID && ((StreamRow)x).Title.Equals(streamItem));
                        if (streamRow == null)
                        {
                            streamRow = new StreamRow(nameof(Streams));
                            streamRow.insertNewRowData();
                            streamRow.Tournament_ID = item.ID;
                            Streams.Rows.Add(streamRow);
                        }
                        else
                        {
                            streamRow.updateRowData();
                        }
                        streamRow.isModified = true;
                        streamRow.Title = streamItem;
                    }
                }

                Tournaments = (Tournament)SQLHandler.saveData(Tournaments);
                Streams = (Stream)SQLHandler.saveData(Streams);

                TournamentsData.ForEach(x => x.isModified = false);
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }
        private static void SaveApiRequestedData(List<ApiRequestedDataHandler> requests)
        {
            try
            {
                var apiData = new Tournament_Api_Data();
                foreach (var request in requests)
                {
                    Tournament_Api_DataRow apiRow = new Tournament_Api_DataRow(nameof(apiData));
                    apiRow.insertNewRowData();
                    apiRow.RequestJSON = request.ApiRequestJson;
                    apiRow.RequestContent = request.ApiRequestContent;
                    apiRow.Response = request.ApiResponse;
                    apiRow.TournamentHostSite_ID = request.HostSite;
                    apiData.Rows.Add(apiRow);
                }

                apiData = (Tournament_Api_Data)SQLHandler.saveData(apiData);
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }
        private static void SortTournaments()
        {
            try
            {
                TournamentsData = TournamentsData.OrderBy(x => x.StartsAT).ToList();
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        public static List<int> GetStartGGGameIDs()
        {
            try
            {
                return new List<int> { 904, 5582 };
                //return games_On_Tournament_Host_Site.rows.Where(x => (Common.TournamentSiteHost)((Game_On_Tournament_Host_SiteRow)x).HostSite_ID == Common.TournamentSiteHost.Start).Select(y => (int)((Game_On_Tournament_Host_SiteRow)y).SpecificHostGameID).ToList();
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return new List<int>();
        }

        public static List<string> GetChallongeTournamentUrls()
        {
            try
            {
                return TournamentsData.Where(x => x.HostSite.Equals(Common.TournamentSiteHost.Challonge.AsString(EnumFormat.Description))).Select(y => y.URL).ToList();
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return new List<string>();
        }
    }
}
