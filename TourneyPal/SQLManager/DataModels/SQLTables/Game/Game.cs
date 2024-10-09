using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;

namespace TourneyPal.SQLManager.DataModels.SQLTables.Game
{
    public class Game : Model
    {
        public override bool load(MySqlDataReader reader)
        {
            bool result = false;
            try
            {
                result = this.initializeRows();
                if (!result)
                {
                    return result;
                }

                while (reader.Read())
                {
                    GameRow row = new GameRow(GetType().Name);
                    result = row.loadRow(reader);
                    if (!result)
                    {
                        return result;
                    }

                    rows.Add(row);
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
