using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
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

                //Tournament_ID
                var tournament_ID = convertToInt(nameof(Tournament_ID), reader[nameof(Tournament_ID)]?.ToString());
                if (tournament_ID == null)
                {
                    result.success = false;
                    return result;
                }
                Tournament_ID = (int)tournament_ID;

                //TournamentApiData_ID
                var tournamentApiData_ID = convertToInt(nameof(TournamentApiData_ID), reader[nameof(TournamentApiData_ID)]?.ToString());
                if (tournamentApiData_ID == null)
                {
                    result.success = false;
                    return result;
                }
                TournamentApiData_ID = (int)tournamentApiData_ID;

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
                if (this.Tournament_ID == null)
                {
                    result.success = false;
                    result.message = nameof(this.Tournament_ID) + ", of table: " + this.tableName + "-- Cannot be null";
                }

                result.success = true;
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
