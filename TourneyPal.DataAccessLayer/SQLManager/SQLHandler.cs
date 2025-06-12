using MySql.Data.MySqlClient;
using Mysqlx.Crud;
using System.Reflection;
using System.Runtime.Serialization;
using TourneyPal.Commons;
using TourneyPal.SQLManager.DataModels;
using Type = System.Type;

namespace TourneyPal.SQLManager
{
    public static class SQLHandler
    {
        public static Model? loadModelData(Model model)
        {
            try
            {
                var connection = new SQLConnection();
                string query = "select * from " + connection.database + "." + model.GetType().Name;

                SQLItem sql = new SQLItem(query, null);
                return connection.Load(sql, model);
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(), 
                           messageItem: "Error loading model",
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return null;
        }

        public static Model saveData(Model model)
        {
            try
            {
                var connection = new SQLConnection();

                if (model?.Rows == null ||
                   model.Rows.Count == 0)
                {
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: "Nothing to save on " + model?.GetType().Name);
                    return null;
                }

                var tableType = model.GetType();

                var rowType = model.Rows.FirstOrDefault().GetType();
                var rowProperties = rowType.GetProperties().Where(pi => !Attribute.IsDefined(pi, typeof(IgnoreDataMemberAttribute))).Select(x => x.Name).ToList();

                var result = insertData(model, connection, tableType, rowProperties);
                if (!result)
                {
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: "Error inserting rows of " + tableType.Name);
                    return null;
                }

                result = updateData(model, connection, tableType, rowProperties);
                if (!result)
                {
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: "Error updating rows of " + tableType.Name);
                    return null;
                }

                return model;
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           messageItem: "Error saving model",
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return null;
        }

        private static bool insertData(Model model, SQLConnection connection, Type tableType, List<string> rowProperties)
        {
            var result = new bool();
            try
            {
                if(model.Rows.Where(x => x.ID == 0).Count() == 0)
                {
                    result = true;
                    return result;
                }

                //insert
                var sqlInsert = getInsertQuery(model.Rows.Where(x => x.ID == 0).ToArray(), connection, tableType, rowProperties);
                if (sqlInsert == null)
                {
                    result = false;
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: "Error creating insert save query of " + tableType.Name);
                    return result;
                }

                result = connection.SaveInsert(sqlInsert, model);
                if (!result)
                {
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: "Error inserting rows of " + tableType.Name);
                    return result;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return result;
        }

        private static SQLItem getInsertQuery(ModelRow[] inserts, SQLConnection connection, Type tableType, List<string> rowProperties)
        {
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
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return null;
            
        }

        private static bool updateData(Model model, SQLConnection connection, Type tableType, List<string> rowProperties)
        {
            var result = new bool();
            try
            {
                if (model.Rows.Where(x => x.ID > 0 && x.isModified).Count() == 0)
                {
                    result = true;
                    return result;
                }

                //update
                var sqlUpdate = getUpdateQuery(model.Rows.Where(x => x.ID > 0 && x.isModified).ToArray(), connection, tableType, rowProperties);
                if (sqlUpdate == null)
                {
                    result = false;
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: "Error creating update save query of " + tableType.Name);
                    return result;
                }

                result = connection.SaveUpdate(sqlUpdate, model);
                if (!result)
                {
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: "Error updating rows of " + tableType.Name);
                    return result;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return result;
        }
        private static SQLItem? getUpdateQuery(ModelRow[] updates, SQLConnection connection, Type tableType, List<string> rowProperties)
        {
            bool result = false;
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
                result = false;
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return null;

        }
    }
}
