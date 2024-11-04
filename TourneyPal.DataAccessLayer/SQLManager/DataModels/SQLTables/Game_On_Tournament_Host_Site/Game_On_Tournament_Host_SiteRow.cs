using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public override bool loadRow(MySqlDataReader reader)
        {
            bool result = false;
            try
            {
                result = base.loadRow(reader);
                if (!result)
                {
                    return result;
                }

                //Game_ID
                var game_ID = convertToInt(nameof(Game_ID), reader[nameof(Game_ID)]?.ToString());
                if (game_ID == null)
                {
                    result = false;
                    return result;
                }
                Game_ID = game_ID;

                //HostSite_ID
                var hostSite_ID = convertToInt(nameof(HostSite_ID), reader[nameof(HostSite_ID)]?.ToString());
                if (hostSite_ID == null)
                {
                    result = false;
                    return result;
                }
                HostSite_ID = hostSite_ID;

                //SpecificHostGameID
                var specificHostGameID = convertToInt(nameof(SpecificHostGameID), reader[nameof(SpecificHostGameID)]?.ToString());
                if (specificHostGameID == null)
                {
                    result = false;
                    return result;
                }
                SpecificHostGameID = specificHostGameID;

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

        public override bool validateRow()
        {
            bool result = false;
            try
            {
                result = base.validateRow();
                if (!result)
                {
                    return result;
                }

                if (this.Game_ID == null)
                {
                    result = false;
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(this.Game_ID) + ", of table: " + this.tableName + "-- Cannot be null");
                }

                if (this.HostSite_ID == null)
                {
                    result = false;
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(this.HostSite_ID) + ", of table: " + this.tableName + "-- Cannot be null");
                }

                if (this.SpecificHostGameID == null)
                {
                    result = false;
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(this.SpecificHostGameID) + ", of table: " + this.tableName + "-- Cannot be null");
                }

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

    }
}
