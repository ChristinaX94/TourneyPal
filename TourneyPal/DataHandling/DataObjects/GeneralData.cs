using TourneyPal.SQLManager.DataModels.SQLTables.Game;
using TourneyPal.SQLManager;
using TourneyPal.SQLManager.DataModels.SQLTables.Tournament;
using TourneyPal.SQLManager.DataModels.SQLTables.Stream;
using Stream = TourneyPal.SQLManager.DataModels.SQLTables.Stream.Stream;
using TourneyPal.SQLManager.DataModels.SQLTables.Tournament_Host_Sites;
using Tournament = TourneyPal.SQLManager.DataModels.SQLTables.Tournament.Tournament;
using TourneyPal.SQLManager.DataModels.SQLTables.Related_Tournaments_Api_Call;
using TourneyPal.SQLManager.DataModels.SQLTables.Tournament_Api_Data;
using EnumsNET;

namespace TourneyPal.DataHandling.DataObjects
{
    public static class GeneralData
    {
        private static Tournament tournaments { get; set; }
        private static Stream streams { get; set; }
        private static Game games { get; set; }
        private static Tournament_Host_Sites tournamentHostSites { get; set; }

        private static SQLHandler sql { get; set; }

        public static List<TournamentData> TournamentsData { get; private set; }
        public static List<ApiRequestedDataHandler> ApiRequestedData { get; private set; }

        public static void GeneralDataInitialize()
        {
            InitializeInternalDBObjects();

            foreach (TournamentRow tournament in tournaments.rows)
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
                    RegistrationOpen = tournament.IsExpired,
                    NumberOfAttendees = tournament.NumberOfAttendees == null ? 0 : (int)tournament.NumberOfAttendees,
                    Game = games.rows.Where(x => x.ID == tournament.Game_ID)?.Select(y => ((GameRow)y).Title).FirstOrDefault(),
                    Streams = streams.rows.Where(x => ((StreamRow)x).Tournament_ID == tournament.ID)?.Select(y => "https://www.twitch.tv/" + ((StreamRow)y).Title).ToList(),
                    HostSite = tournamentHostSites.rows.Where(x => x.ID == tournament.HostSite_ID)?.Select(y => ((Tournament_Host_SitesRow)y).Site).FirstOrDefault(),
                });
            }

        }

        private static void InitializeInternalDBObjects()
        {
            sql = new SQLHandler();

            tournaments = (Tournament)sql.loadModelData(new Tournament());
            streams = (Stream)sql.loadModelData(new Stream());
            games = (Game)sql.loadModelData(new Game());
            tournamentHostSites = (Tournament_Host_Sites)sql.loadModelData(new Tournament_Host_Sites());

            TournamentsData = new List<TournamentData>();
            ApiRequestedData = new List<ApiRequestedDataHandler>();
        }

        public static void addTournament(TournamentData tournament)
        {
            try
            {
                var existingTournament = TournamentsData.FirstOrDefault(x => x.ID == tournament.ID);
                if(existingTournament == null)
                {
                    tournament.isModified = true;
                    TournamentsData.Add(tournament);
                    return;
                }

                var tournamentEdited = !(existingTournament.Online == tournament.Online &&
                                        existingTournament.StartsAT == tournament.StartsAT &&
                                        existingTournament.NumberOfAttendees == tournament.NumberOfAttendees &&
                                        existingTournament.VenueAddress.Equals(tournament.VenueAddress));
                if (tournamentEdited)
                {
                    existingTournament.Online = tournament.Online;
                    existingTournament.StartsAT = tournament.StartsAT;
                    existingTournament.NumberOfAttendees = tournament.NumberOfAttendees;
                    existingTournament.VenueAddress = tournament.VenueAddress;
                    existingTournament.isModified = true;
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex.Message);
            }
        }

        public static void addApiRequestedData(ApiRequestedDataHandler requestinfo)
        {
            try
            {
                if(ApiRequestedData == null)
                {
                    ApiRequestedData = new List<ApiRequestedDataHandler>();
                }
                ApiRequestedData.Add(requestinfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex.Message);
            }
        }

        public static void saveFindings()
        {
            try
            {
                saveTournaments();
                saveApiRequestedData();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex.Message);
            }
        }

        private static void saveTournaments()
        {
            try
            {
                var listToSave =TournamentsData.Where(x=>x.isModified).ToList();
                if(listToSave== null || listToSave.Count == 0)
                {
                    return;
                }

                foreach (var item in listToSave)
                {
                    TournamentRow tournamentDataRow = (TournamentRow)tournaments.rows.FirstOrDefault(x => ((TournamentRow)x).Tournament_ID == item.ID); 
                    
                    if (tournamentDataRow == null)
                    {
                        tournamentDataRow = new TournamentRow(nameof(tournaments));
                        tournamentDataRow.insertNewRowData();
                        tournaments.rows.Add(tournamentDataRow);
                    }
                    else
                    {
                        tournamentDataRow.updateRowData();
                    }

                    tournamentDataRow.HostSite_ID = tournamentHostSites.rows.Where(x => ((Tournament_Host_SitesRow)x).Site.Equals(item.HostSite))?.Select(y => y.ID).FirstOrDefault();
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
                    tournamentDataRow.NumberOfAttendees = item.NumberOfAttendees;
                    tournamentDataRow.Game_ID = games.rows.Where(x => ((GameRow)x).Title.Equals(item.Game))?.Select(y => y.ID).FirstOrDefault();
                    tournamentDataRow.IsExpired = item.StartsAT >= Common.getDate();
                    tournamentDataRow.isModified = true;

                    foreach (var streamItem in item.Streams)
                    {
                        StreamRow streamRow = (StreamRow)streams.rows.FirstOrDefault(x => ((StreamRow)x).Tournament_ID == item.ID && ((StreamRow)x).Title.Equals(streamItem));
                        if (streamRow == null)
                        {
                            streamRow = new StreamRow(nameof(streams));
                            streamRow.insertNewRowData();
                            streamRow.Tournament_ID = item.ID;
                            streams.rows.Add(streamRow);
                        }
                        else
                        {
                            streamRow.updateRowData();
                        }
                        streamRow.isModified = true;
                        streamRow.Title = streamItem;
                    }
                }

                tournaments = (Tournament)sql.saveData(tournaments);
                streams = (Stream)sql.saveData(streams);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex.Message);
            }
        }

        private static void saveApiRequestedData()
        {
            try
            {
                var listSaved = TournamentsData.Where(x => x.isModified).ToList();
                if (listSaved == null || listSaved.Count == 0)
                {
                    return;
                }

                foreach (var request in ApiRequestedData)
                {
                    var relatedTournaments = new Related_Tournaments_Api_Call();
                    var apiData = new Tournament_Api_Data();

                    Tournament_Api_DataRow apiRow = new Tournament_Api_DataRow(nameof(apiData));
                    apiRow.insertNewRowData();
                    apiRow.RequestJSON = request.ApiRequestJson;
                    apiRow.RequestContent = request.ApiRequestContent;
                    apiRow.Response = request.ApiResponse;
                    apiRow.TournamentHostSite_ID = request.HostSite;
                    apiData.rows.Add(apiRow);

                    apiData = (Tournament_Api_Data)sql.saveData(apiData);
                    if (apiData == null)
                    {
                        return;
                    }

                    var relatedTournamentIDs = new List<int>();

                    if (request.StartGGTournaments!=null && request.StartGGTournaments.Count > 0)
                    {
                        var listSavedStartGG = listSaved.Where(y => y.HostSite.Equals(Common.TournamentSiteHost.Start.AsString(EnumFormat.Description))).Select(x => x.ID).ToList();
                        request.StartGGTournaments.RemoveAll(x => !listSavedStartGG.Contains(x));
                        relatedTournamentIDs.AddRange(request.StartGGTournaments);
                    }
                    if (request.ChallongeTournaments != null && request.ChallongeTournaments.Count > 0)
                    {
                        var listSavedChallonge = listSaved.Where(y => y.HostSite.Equals(Common.TournamentSiteHost.Challonge.AsString(EnumFormat.Description))).Select(x => x.ID).ToList();
                        request.ChallongeTournaments.RemoveAll(x => !listSavedChallonge.Contains(x));
                        relatedTournamentIDs.AddRange(request.ChallongeTournaments);
                    }

                    foreach(var relatedTournamentID in relatedTournamentIDs)
                    {
                        Related_Tournaments_Api_CallRow relatedTournament = new Related_Tournaments_Api_CallRow(nameof(relatedTournaments));
                        relatedTournament.insertNewRowData();
                        relatedTournament.Tournament_ID = relatedTournamentID;
                        relatedTournament.TournamentApiData_ID = apiRow.ID;
                        relatedTournaments.rows.Add(relatedTournament);
                    }

                    relatedTournaments = (Related_Tournaments_Api_Call)sql.saveData(relatedTournaments);
                    if (relatedTournaments == null)
                    {
                        return;
                    }
                }


                TournamentsData.ForEach(x => x.isModified = false);
                ApiRequestedData = new List<ApiRequestedDataHandler>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex.Message);
            }
        }
    }
}
