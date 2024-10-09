using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
