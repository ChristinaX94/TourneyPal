using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;

namespace TourneyPal.SQLManager.DataModels.SQLTables.Related_Tournaments_Api_Call
{
    public class Related_Tournaments_Api_Call : Model
    {

        public override Result load(MySqlDataReader reader)
        {
            Result result = new Result();
            try
            {
                while (reader.Read())
                {
                    Related_Tournaments_Api_CallRow row = new Related_Tournaments_Api_CallRow(GetType().Name);
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

        public override Result delete(MySqlDataReader reader)
        {
            throw new NotImplementedException();
        }

    }
}
