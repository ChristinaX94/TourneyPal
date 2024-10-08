using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;

namespace TourneyPal.SQLManager.DataModels.SQLTables.Tournament
{
    public class TournamentRow : ModelRow
    {
        public TournamentRow(string? tableName) : base(tableName)
        {
        }

        public int? HostSite_ID { get; set; }
        public int? Tournament_ID { get; set; }
        public string? Name { get; set; }
        public string? CountryCode { get; set; }
        public string? City { get; set; }
        public string? AddrState { get; set; }
        public DateTime? StartsAT { get; set; }
        public bool? Online { get; set; }
        public string? URL { get; set; }
        public int? State { get; set; }
        public string? VenueAddress { get; set; }
        public string? VenueName { get; set; }
        public bool? RegistrationOpen { get; set; }
        public int? NumberOfAttendees { get; set; }
        public int? Game_ID { get; set; }
        public bool? IsExpired { get; set; }

        public override Result loadRow(MySqlDataReader reader)
        {
            Result result = new Result();
            try
            {
                result = base.loadRow(reader);
                if (!result.success)
                {
                    return result;
                }

                //HostSite_ID
                var hostSite_ID = convertToInt(nameof(HostSite_ID), reader[nameof(HostSite_ID)]?.ToString());
                if (hostSite_ID == null)
                {
                    result.success = false;
                    return result;
                }
                HostSite_ID = (int)hostSite_ID;

                //Tournament_ID
                var tournament_ID = convertToInt(nameof(Tournament_ID), reader[nameof(Tournament_ID)]?.ToString());
                if (tournament_ID == null)
                {
                    result.success = false;
                    return result;
                }
                Tournament_ID = (int)tournament_ID;

                //Name
                Name = reader[nameof(Name)]?.ToString();

                //CountryCode
                CountryCode = reader[nameof(CountryCode)]?.ToString();

                //City
                City = reader[nameof(City)]?.ToString();

                //AddrState
                AddrState = reader[nameof(AddrState)]?.ToString();

                //StartsAT
                var startsAT = convertToDateTime(nameof(StartsAT), reader[nameof(StartsAT)]?.ToString());
                if (startsAT == null)
                {
                    result.success = false;
                    return result;
                }
                StartsAT = (DateTime)startsAT;

                //Online
                var online = convertToBool(nameof(Online), reader[nameof(Online)]?.ToString());
                if (online == null)
                {
                    result.success = false;
                    return result;
                }
                Online = (bool)online;

                //URL
                if (reader[nameof(URL)] == null ||
                    string.IsNullOrEmpty(reader[nameof(URL)].ToString()))
                {
                    result.success = false;
                    return result;
                }
                URL = reader[nameof(URL)].ToString();

                //State
                if (reader[nameof(State)] != null)
                {
                    State = convertToInt(nameof(State), reader[nameof(State)]?.ToString());
                }

                //VenueAddress
                VenueAddress = reader[nameof(VenueAddress)]?.ToString();

                //VenueName
                VenueName = reader[nameof(VenueName)]?.ToString();

                //RegistrationOpen
                var registrationOpen = convertToBool(nameof(RegistrationOpen), reader[nameof(RegistrationOpen)]?.ToString());
                if (registrationOpen == null)
                {
                    result.success = false;
                    return result;
                }
                RegistrationOpen = (bool)registrationOpen;

                //NumberOfAttendees
                if (reader[nameof(NumberOfAttendees)] != null)
                {
                    NumberOfAttendees = convertToInt(nameof(NumberOfAttendees), reader[nameof(NumberOfAttendees)]?.ToString());
                }

                //Game_ID
                var game_ID = convertToInt(nameof(Game_ID), reader[nameof(Game_ID)]?.ToString());
                if (game_ID == null)
                {
                    result.success = false;
                    return result;
                }
                Game_ID = (int)game_ID;

                //IsExpired
                var isExpired = convertToBool(nameof(IsExpired), reader[nameof(IsExpired)]?.ToString());
                if (isExpired == null)
                {
                    result.success = false;
                    return result;
                }
                IsExpired = (bool)isExpired;

                result.success = true;
            }
            catch (Exception ex)
            {
                result.success = false;
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return result;
        }

        public override Result validateRow()
        {
            Result result = new Result();
            try
            {
                result = base.validateRow();
                if (!result.success)
                {
                    return result;
                }

                if (this.HostSite_ID == null)
                {
                    result.success = false;
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(this.HostSite_ID) + ", of table: " + this.tableName + "-- Cannot be null");
                }

                if (this.Tournament_ID == null)
                {
                    result.success = false;
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(this.Tournament_ID) + ", of table: " + this.tableName + "-- Cannot be null");
                }

                if (this.StartsAT == null)
                {
                    result.success = false;
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(this.StartsAT) + ", of table: " + this.tableName + "-- Cannot be null");
                }

                if (this.Online == null)
                {
                    result.success = false;
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(this.Online) + ", of table: " + this.tableName + "-- Cannot be null");
                }

                if (string.IsNullOrEmpty(this.URL))
                {
                    result.success = false;
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(this.URL) + ", of table: " + this.tableName + "-- Cannot be null");
                }

                if (this.RegistrationOpen == null)
                {
                    result.success = false;
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(this.RegistrationOpen) + ", of table: " + this.tableName + "-- Cannot be null");
                }

                if (this.Game_ID == null)
                {
                    result.success = false;
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(this.Game_ID) + ", of table: " + this.tableName + "-- Cannot be null");
                }

                if (this.IsExpired == null)
                {
                    result.success = false;
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(this.IsExpired) + ", of table: " + this.tableName + "-- Cannot be null");
                }

                result.success = true;

            }
            catch (Exception ex)
            {
                result.success = false;
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return result;
        }
    }
}
