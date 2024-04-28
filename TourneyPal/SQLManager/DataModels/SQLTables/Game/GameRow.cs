using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;

namespace TourneyPal.SQLManager.DataModels.SQLTables.Game
{
    public class GameRow : ModelRow
    {
        public GameRow(string? tableName) : base(tableName)
        {
        }

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

                if (string.IsNullOrEmpty(this.Title))
                {
                    result.success = false;
                    result.message = nameof(this.Title) + ", of table: " + this.tableName + "-- Cannot be null";
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
