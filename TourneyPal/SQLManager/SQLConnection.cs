using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;
using TourneyPal.SQLManager;
using TourneyPal.SQLManager.DataModels;
using static Mysqlx.Expect.Open.Types.Condition.Types;

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
                result.message = ex.Message;
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
                result.message = ex.Message;
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
                    result.message = "Error connecting";
                    return null;
                }

                model = executeReadQuery(sql, model);
                if (model == null)
                {
                    result.message = "Error executing sql query: " + sql.query;
                    return null;
                }

                result = disconnect();
                if (!result.success)
                {
                    result.message = "Error Disconnecting";
                    return null;
                }

                return model;
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }
            finally
            {
                if (!result.success)
                {
                    Logger.log(result.message);

                    if (!disconnect().success)
                    {
                        Logger.log("Error Disconnecting");
                    }
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
                    result.success = false;
                    return null;
                }

                cmd.ExecuteNonQuery();

                MySqlDataReader reader = cmd.ExecuteReader();
                result = model.load(reader);
                if (!result.success)
                {
                    return null;
                }

                result.success = true;

            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }
            finally
            {
                if (!result.success)
                {
                    Logger.log(result.message);
                }
            }
            return model;
        }

        public Result Save(SQLItem sql, Model model)
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

                result = executeQuery(sql);
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
                result.message = ex.Message;
            }
            return result;
        }

        private Result executeQuery(SQLItem sql)
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
                result.message = ex.Message;
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
            }
            return null;
        }

    }



}

