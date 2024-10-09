using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                    result = false;
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(this.TournamentHostSite_ID) + ", of table: " + this.tableName + "-- Cannot be null");
                }
                
                if (string.IsNullOrEmpty(this.Response))
                {
                    result = false;
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(this.Response) + ", of table: " + this.tableName + "-- Cannot be null");
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
