using MySql.Data.MySqlClient;
using System.Runtime.Serialization;
using TourneyPal.Commons;
using TourneyPal.SQLManager.DataModels;

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

        public Model saveData(Model model)
        {
            Result result = new Result();
            try
            {
                var connection = new SQLConnection();

                if (model?.rows == null ||
                   model.rows.Count == 0)
                {
                    result.success = true;
                    result.message = "Nothing to save on " + model?.GetType().Name;
                    return null;
                }

                var tableType = model.GetType();

                var rowType = model.rows.FirstOrDefault().GetType();
                var rowProperties = rowType.GetProperties().Where(pi => !Attribute.IsDefined(pi, typeof(IgnoreDataMemberAttribute))).Select(x => x.Name).ToList();

                result = insertData(model, connection, tableType, rowProperties);
                if (!result.success)
                {
                    result.message = "Error inserting rows of " + tableType.Name;
                    return null;
                }

                result = updateData(model, connection, tableType, rowProperties);
                if (!result.success)
                {
                    result.message = "Error updating rows of " + tableType.Name;
                    return null;
                }

                return model;
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }
            return null;
        }

        private Result insertData(Model model, SQLConnection connection, Type tableType, List<string> rowProperties)
        {
            var result = new Result();
            try
            {
                if(model.rows.Where(x => x.ID == 0).Count() == 0)
                {
                    result.success = true;
                    result.message = "Nothing to insert on " + model?.GetType().Name;
                    return result;
                }

                //insert
                var sqlInsert = getInsertQuery(model.rows.Where(x => x.ID == 0).ToArray(), connection, tableType, rowProperties);
                if (sqlInsert == null)
                {
                    result.success = false;
                    result.message = "Error creating insert save query of " + tableType.Name;
                    return result;
                }

                result = connection.SaveInsert(sqlInsert, model);
                if (!result.success)
                {
                    result.message = "Error inserting rows of " + tableType.Name;
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

        private SQLItem getInsertQuery(ModelRow[] inserts, SQLConnection connection, Type tableType, List<string> rowProperties)
        {
            Result result = new Result();
            string propertiesStr;
            string insertQuery;
            List<string> insertQueryStrs = new List<string>();
            List<MySqlParameter> parametersIns = new List<MySqlParameter>();

            try
            {
                propertiesStr = string.Join(",", rowProperties);
                insertQuery = "insert into " + connection.database + "." + tableType.Name + " (" + propertiesStr + ") values ";
                
                foreach (var row in inserts)
                {
                    var pos = Array.IndexOf(inserts, row);
                    var parametersStr = "@" + string.Join(pos + ", @", rowProperties) + pos;
                    string insertQueryRow = "(" + parametersStr + ")";
                    insertQueryStrs.Add(insertQueryRow);

                    foreach (var property in rowProperties)
                    {
                        parametersIns.Add(new MySqlParameter("@" + property + pos.ToString(), row.GetType().GetProperty(property).GetValue(row, null)));
                    }
                }

                insertQuery = insertQuery + string.Join(",", insertQueryStrs);

                return new SQLItem(insertQuery, parametersIns);
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }
            return null;
            
        }

        private Result updateData(Model model, SQLConnection connection, Type tableType, List<string> rowProperties)
        {
            var result = new Result();
            try
            {
                if (model.rows.Where(x => x.ID > 0 && x.isModified).Count() == 0)
                {
                    result.success = true;
                    result.message = "Nothing to update on " + model?.GetType().Name;
                    return result;
                }

                //update
                var sqlUpdate = getUpdateQuery(model.rows.Where(x => x.ID > 0 && x.isModified).ToArray(), connection, tableType, rowProperties);
                if (sqlUpdate == null)
                {
                    result.success = false;
                    result.message = "Error creating update save query of " + tableType.Name;
                    return result;
                }

                result = connection.SaveUpdate(sqlUpdate, model);
                if (!result.success)
                {
                    result.message = "Error updating rows of " + tableType.Name;
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
        private SQLItem getUpdateQuery(ModelRow[] updates, SQLConnection connection, Type tableType, List<string> rowProperties)
        {
            Result result = new Result();
            List<string> updateQueries = new List<string>(); ;
            List<MySqlParameter> parametersUpds = new List<MySqlParameter>();

            try
            {
                foreach (var row in updates)
                {
                    var updateQueryStrs = new List<string>();
                    var pos = Array.IndexOf(updates, row);
                    
                    foreach (var property in rowProperties)
                    {
                        string updateSetQueryRow = property + " = " + "@" + property + pos.ToString();
                        
                        updateQueryStrs.Add(updateSetQueryRow);

                        parametersUpds.Add(new MySqlParameter("@" + property + pos.ToString(), row.GetType().GetProperty(property).GetValue(row, null)));
                    }
                    updateQueries.Add("update " + connection.database + "." + tableType.Name + " set " + string.Join(",", updateQueryStrs) + " where ID= " + row.ID +";");
                }
                return new SQLItem(string.Join("\n", updateQueries), parametersUpds);
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }
            return null;

        }
    }
}
