using MySql.Data.MySqlClient;
using System;
using System.Collections;
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

        public Result saveData(Model model)
        {
            Result result = new Result();
            try
            {
                var connection = new SQLConnection();

                if (model.rows == null ||
                   model.rows.Count == 0)
                {
                    result.success = true;
                    result.message = "Nothing to save on " + model.GetType().Name;
                    return result;
                }

                var type = model.rows.FirstOrDefault().GetType();
                var rowProperties = type.GetProperties().Select(x => x.Name).ToList();
                rowProperties.Remove("ID");

                //example
                var properties = string.Join(",", rowProperties);
                var propertiesAii = string.Join(", @", rowProperties);
                string insertQuery = "insert into " + connection.database + "." + type.Name + " (" + properties + ") values ";
                string updateQuery = "update " + connection.database + "." + type.Name + " set " + properties[0] + "=" + propertiesAii[0] + "...;";
                //example

                var inserts = model.rows.Where(x => x.ID == 0).ToArray();
                var insertQueryStrs = new List<string>();
                foreach (var row in inserts)
                {
                    var pos = Array.IndexOf(inserts, row);
                    var parametersStr = string.Join(pos + ", @", rowProperties);
                    string insertQueryRow = "(" + parametersStr + ")";
                    insertQueryStrs.Add(insertQueryRow);

                    var parameters = new List<MySqlParameter>();
                    foreach(var property in rowProperties)
                    {
                        parameters.Add(new MySqlParameter("@"+property+pos.ToString(), row.GetType().GetProperty(property).GetValue(row,null)));
                    }
                    
                }
                //var parameters = new List<MySqlParameter>();

                //string query = "INSERT INTO aac.data (e7character,picture,url) VALUES(@character, @picture, @url);";


                //parameters.Add(new MySqlParameter("@character", character));
                //parameters.Add(new MySqlParameter("@url", url));

                //SQLItem sql = new SQLItem(query, parameters);


                return connection.Save(new SQLItem(insertQuery, null), model);

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
