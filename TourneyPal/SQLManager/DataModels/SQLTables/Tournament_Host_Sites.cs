using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;

namespace TourneyPal.SQLManager.DataModels.SQLTables
{
    public class Tournament_Host_Sites : Model
    {
        public List<TTournament_Host_SitesRow> rows { get; private set; }
        public class TTournament_Host_SitesRow : ModelRow
        {
            public TTournament_Host_SitesRow(string? tableName) : base(tableName)
            {
            }

            public string Site { get; private set; }
            public string Endpoint { get; private set; }
            public string Token { get; private set; }
            public string? DataFormat { get; private set; }

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

                    //Site
                    if (reader[nameof(Site)] == null ||
                        string.IsNullOrEmpty(reader[nameof(Site)].ToString()))
                    {
                        result.success = false;
                        return result;
                    }
                    this.Site = reader[nameof(Site)].ToString();

                    //Endpoint
                    if (reader[nameof(Endpoint)] == null ||
                        string.IsNullOrEmpty(reader[nameof(Endpoint)].ToString()))
                    {
                        result.success = false;
                        return result;
                    }
                    this.Endpoint = reader[nameof(Endpoint)].ToString();

                    //Token
                    if (reader[nameof(Token)] == null ||
                        string.IsNullOrEmpty(reader[nameof(Token)].ToString()))
                    {
                        result.success = false;
                        return result;
                    }
                    this.Token = reader[nameof(Token)].ToString();

                    //DataFormat
                    DataFormat = reader[nameof(DataFormat)]?.ToString();
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
                    TTournament_Host_SitesRow row = new TTournament_Host_SitesRow(GetType().Name);
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
