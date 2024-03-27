using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;

namespace TourneyPal.SQLManager.DataModels.SQLTables.Game_On_Tournament_Host_Site
{
    public class Game_On_Tournament_Host_SiteRow : ModelRow
    {
        public Game_On_Tournament_Host_SiteRow(string? tableName) : base(tableName)
        {
        }

        public int? Game_ID { get; private set; }
        public int? HostSite_ID { get; private set; }
        public int? SpecificHostGameID { get; private set; }

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

                //Game_ID
                if (reader[nameof(Game_ID)] != null)
                {
                    Game_ID = convertToInt(nameof(Game_ID), reader[nameof(Game_ID)]?.ToString());
                }

                //HostSite_ID
                if (reader[nameof(HostSite_ID)] == null)
                {
                    HostSite_ID = convertToInt(nameof(HostSite_ID), reader[nameof(HostSite_ID)]?.ToString());
                }

                //SpecificHostGameID
                if (reader[nameof(SpecificHostGameID)] == null)
                {
                    SpecificHostGameID = convertToInt(nameof(SpecificHostGameID), reader[nameof(SpecificHostGameID)]?.ToString());
                }
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

                if (this.Game_ID == null)
                {
                    result.success = false;
                    result.message = nameof(this.Game_ID) + ", of table: " + this.tableName + "-- Cannot be null";
                }

                if (this.HostSite_ID == null)
                {
                    result.success = false;
                    result.message = nameof(this.HostSite_ID) + ", of table: " + this.tableName + "-- Cannot be null";
                }

                if (this.SpecificHostGameID == null)
                {
                    result.success = false;
                    result.message = nameof(this.SpecificHostGameID) + ", of table: " + this.tableName + "-- Cannot be null";
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
