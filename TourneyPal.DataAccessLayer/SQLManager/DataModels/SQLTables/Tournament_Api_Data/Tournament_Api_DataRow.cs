using MySql.Data.MySqlClient;
using System.Reflection;
using System.Security.Policy;
using TourneyPal.Commons;

namespace TourneyPal.SQLManager.DataModels.SQLTables.Tournament_Api_Data
{
    public class Tournament_Api_DataRow : ModelRow
    {
        public Tournament_Api_DataRow(string? tableName) : base(tableName)
        {
        }

        public int? TournamentHostSite_ID { get; set; }
        public string? RequestJSON { get; set; }
        public string? RequestContent { get; set; }
        public string? Response { get; set; }

        public override bool loadRow(MySqlDataReader reader)
        {
            bool result = false;
            try
            {
                result = base.loadRow(reader);
                if (!result)
                {
                    return result;
                }

                //TournamentHostSite_ID
                var tournamentHostSite_ID = convertToInt(nameof(TournamentHostSite_ID), reader[nameof(TournamentHostSite_ID)]?.ToString());
                if (tournamentHostSite_ID == null)
                {
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(TournamentHostSite_ID) + ", of table: " + tableName + "-- Could not be loaded");
                    result = false;
                    return result;
                }
                TournamentHostSite_ID = (int)tournamentHostSite_ID;

                //RequestJSON
                RequestJSON = reader[nameof(RequestJSON)].ToString();

                //RequestContent
                RequestContent = reader[nameof(RequestContent)].ToString();

                //Response
                if (reader[nameof(Response)] == null ||
                    string.IsNullOrEmpty(reader[nameof(Response)].ToString()))
                {
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(Response) + ", of table: " + tableName + "-- Could not be loaded");
                    result = false;
                    return result;
                }
                Response = reader[nameof(Response)].ToString();

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return result;
        }

        public override bool validateRow()
        {
            bool result = false;
            try
            {
                if (this.TournamentHostSite_ID == null)
                {
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(this.TournamentHostSite_ID) + ", of table: " + this.tableName + "-- Cannot be null");
                    result = false;
                    return result;
                }
                
                if (string.IsNullOrEmpty(this.Response))
                {
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(this.Response) + ", of table: " + this.tableName + "-- Cannot be null");
                    result = false;
                    return result;
                }

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return result;
        }

    }

}
