using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;

namespace TourneyPal.SQLManager.DataModels.SQLTables.Errorlogs
{
    public class ErrorlogsRow : ModelRow
    {
        public ErrorlogsRow(string? tableName) : base(tableName)
        {
        }

        public string? Message { get; set; }
        public string? ExceptionMessage { get; set; }
        public string? FoundIn { get; set; }

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

                //Message
                Message = reader[nameof(Message)].ToString();

                //ExceptionMessage
                ExceptionMessage = reader[nameof(ExceptionMessage)]?.ToString();

                //FoundIn
                if (reader[nameof(FoundIn)] == null ||
                    string.IsNullOrEmpty(reader[nameof(FoundIn)].ToString()))
                {
                    result = false;
                    return result;
                }
                FoundIn = reader[nameof(FoundIn)].ToString();

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

                if (string.IsNullOrEmpty(this.FoundIn))
                {
                    result = false;
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(this.FoundIn) + ", of table: " + this.tableName + "-- Cannot be null");
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
