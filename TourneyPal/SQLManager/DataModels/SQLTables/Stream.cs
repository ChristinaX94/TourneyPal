using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;

namespace TourneyPal.SQLManager.DataModels.SQLTables
{
    public class Stream : Model
    {
        public List<StreamRow> rows { get; private set; }
        public class StreamRow : ModelRow
        {
            public StreamRow(string? tableName) : base(tableName)
            {
            }

            public int Game_ID { get; private set; }
            public int HostSite_ID { get; private set; }
            public int SpecificHostGameID { get; private set; }

            public Result loadRow(MySqlDataReader reader)
            {
                Result result = new Result();
                try
                {
                    result = base.loadRow(reader);
                    if (!result.success)
                    {
                        return result;
                    }

                    //Game_ID
                    var game_ID = convertToInt(nameof(Game_ID), reader[nameof(Game_ID)]?.ToString());
                    if (game_ID == null)
                    {
                        result.success = false;
                        return result;
                    }
                    this.Game_ID = (int)game_ID;

                    //HostSite_ID
                    var hostSite_ID = convertToInt(nameof(HostSite_ID), reader[nameof(HostSite_ID)]?.ToString());
                    if (hostSite_ID == null)
                    {
                        result.success = false;
                        return result;
                    }
                    this.HostSite_ID = (int)hostSite_ID;

                    //SpecificHostGameID
                    var specificHostGameID = convertToInt(nameof(SpecificHostGameID), reader[nameof(SpecificHostGameID)]?.ToString());
                    if (specificHostGameID == null)
                    {
                        result.success = false;
                        return result;
                    }
                    this.SpecificHostGameID = (int)specificHostGameID;
                }
                catch (Exception ex)
                {
                    result.success = false;
                    result.message = ex.Message;
                }
                return result;
            }

        }

        public override Result load(MySqlDataReader reader)
        {
            Result result = new Result();
            try
            {
                while (reader.Read())
                {
                    StreamRow row = new StreamRow(GetType().Name);
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

        public override Result save(MySqlDataReader reader)
        {
            throw new NotImplementedException();
        }
    }
}