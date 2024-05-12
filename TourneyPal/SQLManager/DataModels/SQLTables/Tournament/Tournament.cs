using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;
using static TourneyPal.SQLManager.DataModels.Model;

namespace TourneyPal.SQLManager.DataModels.SQLTables.Tournament
{
    public class Tournament : Model
    {
        public override Result validate()
        {
            throw new NotImplementedException();
        }

        public override Result load(MySqlDataReader reader)
        {
            Result result = new Result();
            try
            {
                while (reader.Read())
                {
                    TournamentRow row = new TournamentRow(GetType().Name);
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
