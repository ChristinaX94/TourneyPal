using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        private bool connect()
        {
            bool result = false;
            try
            {
                string connectionString = "SERVER=" + server + ";DATABASE=" + database + ";UID=" + username + ";password=" + password;
                connection = new MySqlConnection(connectionString);
                connection.Open();
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

        private bool disconnect()
        {
            bool result = false;
            try
            {
                connection.Close();
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(), exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return result;
        }

        

        public Model Load(SQLItem sql, Model model)
        {
            bool result = false;
            try
            {
                result = connect();
                if (!result)
                {
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           messageItem: "Error connecting");
                    return null;
                }

                model = executeReadQuery(sql, model);
                if (model == null)
                {
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           messageItem: "Error executing sql query: " + sql.query);
                    return null;
                }

                result = disconnect();
                if (!result)
                {
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: "Error Disconnecting");
                    return null;
                }

                return model;
            }
            catch (Exception ex)
            {
                result = false;
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            finally
            {
                if (!disconnect())
                {
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: "Error Disconnecting");
                }
            }
            return null;
        }

        private Model executeReadQuery(SQLItem sql, Model model)
        {
            bool result = false;
            try
            {
                MySqlCommand cmd = getMySqlCommand(sql);
                if (cmd == null)
                {
                    result = false;
                    return null;
                }

                cmd.ExecuteNonQuery();

                MySqlDataReader reader = cmd.ExecuteReader();
                result = model.load(reader);
                if (!result)
                {
                    return null;
                }

                result = true;

            }
            catch (Exception ex)
            {
                result = false;
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return model;
        }

        public bool SaveInsert(SQLItem sql, Model model)
        {
            bool result = false;
            try
            {
                result = model.save();
                if (!result)
                {
                    return result;
                }

                result = connect();
                if (!result)
                {
                    return result;
                }

                result = executeInsertQuery(sql, model);
                if (!result)
                {
                    return result;
                }

                result = disconnect();
                if (!result)
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(), exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return result;
        }

        private bool executeInsertQuery(SQLItem sql, Model model)
        {
            bool result = false;
            try
            {
                MySqlCommand cmd = getMySqlCommand(sql);
                if (cmd == null)
                {
                    result = false;
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

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(), exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return result;
        }

        public bool SaveUpdate(SQLItem sql, Model model)
        {
            bool result = false;
            try
            {
                result = model.save();
                if (!result)
                {
                    return result;
                }

                result = connect();
                if (!result)
                {
                    return result;
                }

                result = executeUpdateQuery(sql);
                if (!result)
                {
                    return result;
                }

                result = disconnect();
                if (!result)
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                result = false;
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(), exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return result;
        }

        private bool executeUpdateQuery(SQLItem sql)
        {
            bool result = false;
            try
            {
                MySqlCommand cmd = getMySqlCommand(sql);
                if (cmd == null)
                {
                    result = false;
                    return result;
                }

                cmd.ExecuteNonQuery();

                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(), exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
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

