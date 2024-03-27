using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;

namespace TourneyPal.SQLManager.DataModels.SQLTables.Tournament_Host_Sites
{
    public class Tournament_Host_SitesRow : ModelRow
    {
        public Tournament_Host_SitesRow(string? tableName) : base(tableName)
        {
        }

        public string? Site { get; private set; }
        public string? Endpoint { get; private set; }
        public string? Token { get; private set; }
        public string? DataFormat { get; private set; }

        public override Result loadRow(MySqlDataReader reader)
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
                Site = reader[nameof(Site)].ToString();

                //Endpoint
                if (reader[nameof(Endpoint)] == null ||
                    string.IsNullOrEmpty(reader[nameof(Endpoint)].ToString()))
                {
                    result.success = false;
                    return result;
                }
                Endpoint = reader[nameof(Endpoint)].ToString();

                //Token
                if (reader[nameof(Token)] == null ||
                    string.IsNullOrEmpty(reader[nameof(Token)].ToString()))
                {
                    result.success = false;
                    return result;
                }
                Token = reader[nameof(Token)].ToString();

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

        public override Result validateRow()
        {
            Result result = new Result();
            try
            {
                result = base.validateRow();
                if (!result.success)
                {
                    return result;
                }

                if (string.IsNullOrEmpty(this.Site))
                {
                    result.success = false;
                    result.message = nameof(this.Site) + ", of table: " + this.tableName + "-- Cannot be null";
                }

                if (string.IsNullOrEmpty(this.Token))
                {
                    result.success = false;
                    result.message = nameof(this.Token) + ", of table: " + this.tableName + "-- Cannot be null";
                }

                if (string.IsNullOrEmpty(this.Endpoint))
                {
                    result.success = false;
                    result.message = nameof(this.Endpoint) + ", of table: " + this.tableName + "-- Cannot be null";
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
