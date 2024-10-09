using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;

namespace TourneyPal.SQLManager.DataModels.SQLTables.Related_Tournaments_Api_Call
{
    public class Related_Tournaments_Api_CallRow : ModelRow
    {
        public Related_Tournaments_Api_CallRow(string? tableName) : base(tableName)
        {
        }

        public int? Tournament_ID { get; set; }

        public int? TournamentApiData_ID { get; set; }

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

                //Tournament_ID
                var tournament_ID = convertToInt(nameof(Tournament_ID), reader[nameof(Tournament_ID)]?.ToString());
                if (tournament_ID == null)
                {
                    result = false;
                    return result;
                }
                Tournament_ID = (int)tournament_ID;

                //TournamentApiData_ID
                var tournamentApiData_ID = convertToInt(nameof(TournamentApiData_ID), reader[nameof(TournamentApiData_ID)]?.ToString());
                if (tournamentApiData_ID == null)
                {
                    result = false;
                    return result;
                }
                TournamentApiData_ID = (int)tournamentApiData_ID;

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
                if (this.Tournament_ID == null)
                {
                    result = false;
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(this.Tournament_ID) + ", of table: " + this.tableName + "-- Cannot be null");
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
