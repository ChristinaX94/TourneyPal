using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;
using static TourneyPal.SQLManager.DataModels.SQLTables.Game_On_Tournament_Host_Site.Game_On_Tournament_Host_Site;

namespace TourneyPal.SQLManager.DataModels.SQLTables.Game_On_Tournament_Host_Site
{
    public class Game_On_Tournament_Host_Site : Model
    {

        public override Result load(MySqlDataReader reader)
        {
            Result result = new Result();
            try
            {
                result = this.initializeRows();
                if (!result.success)
                {
                    return result;
                }

                while (reader.Read())
                {
                    Game_On_Tournament_Host_SiteRow row = new Game_On_Tournament_Host_SiteRow(GetType().Name);
                    result = row.loadRow(reader);
                    if (!result.success)
                    {
                        return result;
                    }
                    rows.Add(row);
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
