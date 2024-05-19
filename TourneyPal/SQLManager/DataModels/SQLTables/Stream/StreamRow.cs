using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public int? Tournament_ID { get; private set; }
        public string? Title { get; private set; }

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

                //Title
                if (reader[nameof(Title)] == null ||
                    string.IsNullOrEmpty(reader[nameof(Title)].ToString()))
                {
                    result.success = false;
                    return result;
                }
                Title = reader[nameof(Title)].ToString();

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
                result = base.validateRow();
                if (!result.success)
                {
                    return result;
                }

                if (this.Tournament_ID == null)
                {
                    result.success = false;
                    result.message = nameof(this.Tournament_ID) + ", of table: " + this.tableName + "-- Cannot be null";
                }

                if (string.IsNullOrEmpty(this.Title))
                {
                    result.success = false;
                    result.message = nameof(this.Title) + ", of table: " + this.tableName + "-- Cannot be null";
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
