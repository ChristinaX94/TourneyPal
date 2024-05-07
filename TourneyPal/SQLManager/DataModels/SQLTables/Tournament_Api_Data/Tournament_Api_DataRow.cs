using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;

namespace TourneyPal.SQLManager.DataModels.SQLTables.Tournament_Api_Data
{
    public class Tournament_Api_DataRow : ModelRow
    {
        public Tournament_Api_DataRow(string? tableName) : base(tableName)
        {
        }

        public int? RelatedTournamentsApiCall_ID { get; private set; }
        public int? TournamentHostSite_ID { get; private set; }
        public string? Request { get; private set; }
        public string? Response { get; private set; }

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

                //RelatedTournamentsApiCall_ID
                var relatedTournamentsApiCall_ID = convertToInt(nameof(RelatedTournamentsApiCall_ID), reader[nameof(RelatedTournamentsApiCall_ID)]?.ToString());
                if (relatedTournamentsApiCall_ID == null)
                {
                    result.success = false;
                    return result;
                }
                RelatedTournamentsApiCall_ID = (int)relatedTournamentsApiCall_ID;

                //TournamentHostSite_ID
                var tournamentHostSite_ID = convertToInt(nameof(TournamentHostSite_ID), reader[nameof(TournamentHostSite_ID)]?.ToString());
                if (tournamentHostSite_ID == null)
                {
                    result.success = false;
                    return result;
                }
                TournamentHostSite_ID = (int)tournamentHostSite_ID;

                //Request
                if (reader[nameof(Request)] == null ||
                    string.IsNullOrEmpty(reader[nameof(Request)].ToString()))
                {
                    result.success = false;
                    return result;
                }
                Request = reader[nameof(Request)].ToString();

                //Response
                if (reader[nameof(Response)] == null ||
                    string.IsNullOrEmpty(reader[nameof(Response)].ToString()))
                {
                    result.success = false;
                    return result;
                }
                Response = reader[nameof(Response)].ToString();

                result.success = true;
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }
            return result;
        }

        public override Result validateRow()
        {
            Result result = new Result();
            try
            {
                if (this.RelatedTournamentsApiCall_ID == null)
                {
                    result.success = false;
                    result.message = nameof(this.RelatedTournamentsApiCall_ID) + ", of table: " + this.tableName + "-- Cannot be null";
                }
                if (this.TournamentHostSite_ID == null)
                {
                    result.success = false;
                    result.message = nameof(this.TournamentHostSite_ID) + ", of table: " + this.tableName + "-- Cannot be null";
                }
                if (string.IsNullOrEmpty(this.Request))
                {
                    result.success = false;
                    result.message = nameof(this.Request) + ", of table: " + this.tableName + "-- Cannot be null";
                }
                if (string.IsNullOrEmpty(this.Response))
                {
                    result.success = false;
                    result.message = nameof(this.Response) + ", of table: " + this.tableName + "-- Cannot be null";
                }
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }
            return result;
        }

    }

}
