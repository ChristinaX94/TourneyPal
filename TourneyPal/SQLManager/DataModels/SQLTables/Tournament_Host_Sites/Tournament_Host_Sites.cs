using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;

namespace TourneyPal.SQLManager.DataModels.SQLTables.Tournament_Host_Sites
{
    public class Tournament_Host_Sites : Model
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
                    Tournament_Host_SitesRow row = new Tournament_Host_SitesRow(GetType().Name);
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
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return result;
        }
    }
}
