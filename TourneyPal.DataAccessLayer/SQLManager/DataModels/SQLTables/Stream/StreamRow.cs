using MySql.Data.MySqlClient;
using System.Reflection;
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
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(Tournament_ID) + ", of table: " + tableName + "-- Could not be loaded");
                    result = false;
                    return result;
                }
                Tournament_ID = (int)tournament_ID;

                //Title
                if (reader[nameof(Title)] == null ||
                    string.IsNullOrEmpty(reader[nameof(Title)].ToString()))
                {
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(Title) + ", of table: " + tableName + "-- Could not be loaded");
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
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(this.Tournament_ID) + ", of table: " + this.tableName + "-- Cannot be null");
                    result = false;
                    return result;
                }

                if (string.IsNullOrEmpty(this.Title))
                {
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(this.Title) + ", of table: " + this.tableName + "-- Cannot be null");
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
