using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.SQLManager.DataModels.SQLTables.Game;
using TourneyPal.SQLManager;
using TourneyPal.SQLManager.DataModels.SQLTables.Tournament;
using TourneyPal.SQLManager.DataModels.SQLTables.Stream;
using Stream = TourneyPal.SQLManager.DataModels.SQLTables.Stream.Stream;
using TourneyPal.SQLManager.DataModels.SQLTables.Tournament_Host_Sites;
using TourneyPal.GeneralData;
using static TourneyPal.DataHandling.ChallongeHelper.ChallongeJsonObject;
using Tournament = TourneyPal.SQLManager.DataModels.SQLTables.Tournament.Tournament;

namespace TourneyPal.DataHandling.DataObjects
{
    public static class GeneralData
    {
        private static Tournament tournaments { get; set; }
        private static Stream streams { get; set; }
        private static Game games { get; set; }
        private static Tournament_Host_Sites tournamentHostSite { get; set; }

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
                    ID = tournament.TournamentHostSite_ID,
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
                    NumberOfAttendees = tournament.NumberOfAttendees,
                    Game = games.rows.Where(x => x.ID == tournament.Game_ID)?.Select(y => ((GameRow)y).Title).FirstOrDefault(),
                    Streams = streams.rows.Where(x => ((StreamRow)x).Tournament_ID == tournament.ID)?.Select(y => "https://www.twitch.tv/" + ((StreamRow)y).Title).ToList(),
                    TournamentHostSite = tournamentHostSite.rows.Where(x => x.ID == tournament.TournamentHostSite_ID)?.Select(y => ((Tournament_Host_SitesRow)y).Site).FirstOrDefault(),
                });
            }

        }

        private static void InitializeInternalDBObjects()
        {
            sql = new SQLHandler();

            tournaments = (Tournament)sql.loadModelData(new Tournament());
            streams = (Stream)sql.loadModelData(new Stream());
            games = (Game)sql.loadModelData(new Game());
            tournamentHostSite = (Tournament_Host_Sites)sql.loadModelData(new Tournament_Host_Sites());

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

        public static void saveTournaments()
        {
            try
            {
                var listToSave =TournamentsData.Where(x=>x.isModified).ToList();
                foreach (var item in listToSave)
                {
                    TournamentRow tournamentDataRow = (TournamentRow)tournaments.rows.FirstOrDefault(x => ((TournamentRow)x).TournamentHostSite_ID == item.ID); 
                    
                    if (tournamentDataRow == null)
                    {
                        tournamentDataRow = new TournamentRow(nameof(tournaments));
                        tournamentDataRow.insertNewRowData();
                        tournaments.rows.Add(tournamentDataRow);
                    }
                    else
                    {
                        tournamentDataRow.insertRowData();
                    }

                    tournamentDataRow.HostSite_ID = tournamentHostSite.rows.Where(x => ((Tournament_Host_SitesRow)x).Site.Equals(item.TournamentHostSite))?.Select(y => y.ID).FirstOrDefault();
                    tournamentDataRow.TournamentHostSite_ID = item.ID;
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
                    tournamentDataRow.IsExpired = item.StartsAT >= General.getDate();
                }
                //save stuff

                sql.saveData(tournaments);
                ApiRequestedData = new List<ApiRequestedDataHandler>();
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex.Message);
            }
        }
    }
}
