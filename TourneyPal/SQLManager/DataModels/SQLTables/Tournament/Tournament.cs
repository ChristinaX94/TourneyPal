using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;
using static TourneyPal.SQLManager.DataModels.Model;

namespace TourneyPal.SQLManager.DataModels.SQLTables.Tournament
{
    public class Tournament : Model
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
                    TournamentRow row = new TournamentRow(GetType().Name);
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
