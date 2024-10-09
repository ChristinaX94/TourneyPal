using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;

namespace TourneyPal.SQLManager.DataModels.SQLTables.Stream
{
    public class StreamRow : ModelRow
    {
        public StreamRow(string? tableName) : base(tableName)
        {
        }

        public int? Tournament_ID { get; set; }
        public string? Title { get; set; }

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

                //Title
                if (reader[nameof(Title)] == null ||
                    string.IsNullOrEmpty(reader[nameof(Title)].ToString()))
                {
                    result = false;
                    return result;
                }
                Title = reader[nameof(Title)].ToString();

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
                result = base.validateRow();
                if (!result)
                {
                    return result;
                }

                if (this.Tournament_ID == null)
                {
                    result = false;
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(this.Tournament_ID) + ", of table: " + this.tableName + "-- Cannot be null");
                }

                if (string.IsNullOrEmpty(this.Title))
                {
                    result = false;
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(this.Title) + ", of table: " + this.tableName + "-- Cannot be null");
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
