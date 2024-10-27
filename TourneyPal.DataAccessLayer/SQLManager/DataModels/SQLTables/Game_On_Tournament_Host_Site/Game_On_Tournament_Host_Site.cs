using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;
using static TourneyPal.SQLManager.DataModels.SQLTables.Game_On_Tournament_Host_Site.Game_On_Tournament_Host_Site;

namespace TourneyPal.SQLManager.DataModels.SQLTables.Game_On_Tournament_Host_Site
{
    public class Game_On_Tournament_Host_Site : Model
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
                    Game_On_Tournament_Host_SiteRow row = new Game_On_Tournament_Host_SiteRow(GetType().Name);
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
