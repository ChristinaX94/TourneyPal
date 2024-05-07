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

        public static void GeneralDataInitialize()
        {
            InitializeInternalDBObjects();

            TournamentsData = new List<TournamentData>();

            foreach (TournamentRow tournament in tournaments.rows)
            {
                var TournamentData = new TournamentData()
                {
                    ID = tournament.ID,
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
                    Game = games.rows.Where(x=>x.ID== tournament.Game_ID)?.Select(y=> ((GameRow)y).Title).FirstOrDefault(),
                    Streams = streams.rows.Where(x => ((StreamRow)x).Tournament_ID == tournament.ID)?.Select(y => "https://www.twitch.tv/" + ((StreamRow)y).Title).ToList(),
                    TournamentHostSite = tournamentHostSite.rows.Where(x => x.ID == tournament.TournamentHostSite_ID)?.Select(y => ((Tournament_Host_SitesRow)y).Site).FirstOrDefault(),
                };
            }

        }

        private static void InitializeInternalDBObjects()
        {
            sql = new SQLHandler();

            tournaments = (Tournament)sql.loadModelData(new Tournament());
            streams = (Stream)sql.loadModelData(new Stream());
            games = (Game)sql.loadModelData(new Game());
            tournamentHostSite = (Tournament_Host_Sites)sql.loadModelData(new Tournament_Host_Sites());
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
                                        existingTournament.VenueAddress.Equals(tournament.VenueAddress));
                if (tournamentEdited)
                {
                    existingTournament.Online = tournament.Online;
                    existingTournament.StartsAT = tournament.StartsAT;
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

        public static void saveTournaments()
        {
            try
            {
                
                var listToSave =TournamentsData.Select(x=>x.isModified);
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex.Message);
            }
        }
    }
}
