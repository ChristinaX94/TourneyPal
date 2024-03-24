using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;

namespace TourneyPal.SQLManager.DataModels.SQLTables
{
    public class Tournament_Api_Data: Model
    {
        public List<Tournament_Api_DataRow> rows { get; private set; }
        public class Tournament_Api_DataRow : ModelRow
        {
            public Tournament_Api_DataRow(string? tableName) : base(tableName)
            {
            }

            public int Tournament_ID { get; private set; }
            public int HostSite_ID { get; private set; }
            public int TournamentHostSite_ID { get; private set; }
            public string Request { get; private set; }
            public string Response { get; private set; }

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

                    //Tournament_ID
                    var tournament_ID = convertToInt(nameof(Tournament_ID), reader[nameof(Tournament_ID)]?.ToString());
                    if (tournament_ID == null)
                    {
                        result.success = false;
                        return result;
                    }
                    this.Tournament_ID = (int)tournament_ID;

                    //HostSite_ID
                    var hostSite_ID = convertToInt(nameof(HostSite_ID), reader[nameof(HostSite_ID)]?.ToString());
                    if (hostSite_ID == null)
                    {
                        result.success = false;
                        return result;
                    }
                    this.HostSite_ID = (int)hostSite_ID;

                    //TournamentHostSite_ID
                    var tournamentHostSite_ID = convertToInt(nameof(TournamentHostSite_ID), reader[nameof(TournamentHostSite_ID)]?.ToString());
                    if (tournamentHostSite_ID == null)
                    {
                        result.success = false;
                        return result;
                    }
                    this.TournamentHostSite_ID = (int)tournamentHostSite_ID;

                    //Request
                    if (reader[nameof(Request)] == null ||
                        string.IsNullOrEmpty(reader[nameof(Request)].ToString()))
                    {
                        result.success = false;
                        return result;
                    }
                    this.Request = reader[nameof(Request)].ToString();

                    //Response
                    if (reader[nameof(Response)] == null ||
                        string.IsNullOrEmpty(reader[nameof(Response)].ToString()))
                    {
                        result.success = false;
                        return result;
                    }
                    this.Response = reader[nameof(Response)].ToString();
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

        public override Result save(MySqlDataReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
