using MySql.Data.MySqlClient;
using System.Reflection;
using TourneyPal.Commons;
using TourneyPal.SQLManager.DataModels;

namespace TourneyPal.SQLManager
{
    public class SQLConnection
    {
        private string server;
        private string username;
        private string password;
        public string database { get; private set; }

        private MySqlConnection connection;

        public SQLConnection()
        {
            server = System.Configuration.ConfigurationManager.AppSettings["server"];
            database = System.Configuration.ConfigurationManager.AppSettings["database"];
            username = System.Configuration.ConfigurationManager.AppSettings["username"];
            password = System.Configuration.ConfigurationManager.AppSettings["password"];
        }

        private Result connect()
        {
            Result result = new Result();
            try
            {
                string connectionString = "SERVER=" + server + ";DATABASE=" + database + ";UID=" + username + ";password=" + password;
                connection = new MySqlConnection(connectionString);
                connection.Open();
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

        private Result disconnect()
        {
            Result result = new Result();
            try
            {
                connection.Close();
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

        

        public Model Load(SQLItem sql, Model model)
        {
            Result result = new Result();
            try
            {
                result = connect();
                if (!result.success)
                {
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: "Error connecting");
                    return null;
                }

                model = executeReadQuery(sql, model);
                if (model == null)
                {
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: "Error executing sql query: " + sql.query);
                    return null;
                }

                result = disconnect();
                if (!result.success)
                {
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: "Error Disconnecting");
                    return null;
                }

                return model;
            }
            catch (Exception ex)
            {
                result.success = false;
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            finally
            {
                if (!disconnect().success)
                {
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: "Error Disconnecting");
                }
            }
            return null;
        }

        private Model executeReadQuery(SQLItem sql, Model model)
        {
            Result result = new Result();
            try
            {
                MySqlCommand cmd = getMySqlCommand(sql);
                if (cmd == null)
                {
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: "Error getting sql command");
                    return null;
                }

                cmd.ExecuteNonQuery();

                MySqlDataReader reader = cmd.ExecuteReader();
                result = model.load(reader);
                if (!result.success)
                {
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: "Error loading "+ model?.GetType().Name);
                    return null;
                }

                result.success = true;

            }
            catch (Exception ex)
            {
                result.success = false;
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return model;
        }

        public Result SaveInsert(SQLItem sql, Model model)
        {
            Result result = new Result();
            try
            {
                result = model.save();
                if (!result.success)
                {
                    return result;
                }

                result = connect();
                if (!result.success)
                {
                    return result;
                }

                result = executeInsertQuery(sql, model);
                if (!result.success)
                {
                    return result;
                }

                result = disconnect();
                if (!result.success)
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.success = false;
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return result;
        }

        private Result executeInsertQuery(SQLItem sql, Model model)
        {
            Result result = new Result();
            try
            {
                MySqlCommand cmd = getMySqlCommand(sql);
                if (cmd == null)
                {
                    result.success = false;
                    return result;
                }

                cmd.ExecuteNonQuery();

                var lastinsertedID = (int)cmd.LastInsertedId;

                if (lastinsertedID > 0)
                {
                    foreach (var row in model.rows)
                    {
                        row.setNewID(lastinsertedID);
                        lastinsertedID++;
                    }
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

        public Result SaveUpdate(SQLItem sql, Model model)
        {
            Result result = new Result();
            try
            {
                result = model.save();
                if (!result.success)
                {
                    return result;
                }

                result = connect();
                if (!result.success)
                {
                    return result;
                }

                result = executeUpdateQuery(sql);
                if (!result.success)
                {
                    return result;
                }

                result = disconnect();
                if (!result.success)
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.success = false;
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return result;
        }

        private Result executeUpdateQuery(SQLItem sql)
        {
            Result result = new Result();
            try
            {
                MySqlCommand cmd = getMySqlCommand(sql);
                if (cmd == null)
                {
                    result.success = false;
                    return result;
                }

                cmd.ExecuteNonQuery();

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

        private MySqlCommand getMySqlCommand(SQLItem sql)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(sql.query, connection);

                if (sql.parameters != null && sql.parameters.Count > 0)
                {
                    foreach (var parameter in sql.parameters)
                    {
                        cmd.Parameters.Add(parameter);
                    }
                }

                return cmd;
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return null;
        }

    }



}

