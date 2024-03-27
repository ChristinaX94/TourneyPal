using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;

namespace TourneyPal.SQLManager.DataModels.SQLTables.Tournament_Api_Data
{
    public class Tournament_Api_Data : Model
    {
        
        public override Result load(MySqlDataReader reader)
        {
            Result result = new Result();
            try
            {
                while (reader.Read())
                {
                    Tournament_Api_DataRow row = new Tournament_Api_DataRow(GetType().Name);
                    result = row.loadRow(reader);
                    if (!result.success)
                    {
                        return result;
                    }

                    rows.Add(row);
                }
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
