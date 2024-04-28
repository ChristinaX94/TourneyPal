using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;
using TourneyPal.SQLManager.DataModels;
using TourneyPal.SQLManager.DataModels.SQLTables.Game;
using static TourneyPal.DataHandling.StartGGHelper.StartGGJsonObject;

namespace TourneyPal.SQLManager
{
    public class SQLHandler
    {
        public Model loadModelData(Model model)
        {
            Result result = new Result();
            try
            {
                var connection = new SQLConnection();
                string query = "select * from " + connection.database + "." + model.GetType().Name;

                SQLItem sql = new SQLItem(query, null);
                return connection.Load(sql, model);
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }
            return null;
        }

        public Result saveData(string character, byte[] picture, string url)
        {
            Result result = new Result();
            try
            {
                var parameters = new List<MySqlParameter>();

                string query = "INSERT INTO aac.data (e7character,picture,url) VALUES(@character, @picture, @url);";

                parameters.Add(new MySqlParameter("@character", character));
                parameters.Add(new MySqlParameter("@url", url));

                var imageParameter = new MySqlParameter("@picture", MySqlDbType.LongBlob, picture.Length);
                imageParameter.Value = picture;

                parameters.Add(imageParameter);

                SQLItem sql = new SQLItem(query, parameters);


                //result = conn.cycleAppend(sql);
                if (!result.success)
                {
                    return result;
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
