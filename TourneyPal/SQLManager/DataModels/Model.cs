using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;
using Result = TourneyPal.Commons.Result;

namespace TourneyPal.SQLManager.DataModels
{
    public abstract class Model : iModel
    {
        public class ModelRow
        {
            public ModelRow(string? tableName)
            {
                this.tableName = tableName;
            }

            private string? tableName { get; set; }
            public int ID { get; private set; }
            public DateTime DateUpdated { get; private set; }
            public DateTime DateInserted { get; private set; }

            protected Result loadRow(MySqlDataReader reader)
            {
                Result result = new Result();
                try
                {
                    //ID
                    var id = convertToInt(nameof(this.ID), reader[nameof(this.ID)]?.ToString());
                    if (id == null)
                    {
                        result.success = false;
                        return result;
                    }
                    this.ID =  (int)id;

                    //DateUpdated
                    var dateUpdated = convertToDateTime(nameof(this.DateUpdated), reader[nameof(this.DateUpdated)]?.ToString());
                    if (dateUpdated == null)
                    {
                        result.success = false;
                        return result;
                    }
                    this.DateUpdated = (DateTime)dateUpdated;

                    //DateInserted
                    var dateInserted = convertToDateTime(nameof(this.DateInserted), reader[nameof(this.DateInserted)]?.ToString());
                    if (dateInserted == null)
                    {
                        result.success = false;
                        return result;
                    }
                    this.DateInserted = (DateTime)dateInserted;
                }
                catch (Exception ex)
                {
                    result.success = false;
                    result.message = ex.Message;
                }
                return result;
            }

            protected int? convertToInt(string? name, string? value)
            {
                try
                {
                    Int32.TryParse(value, out var intValue);
                    return intValue;
                }
                catch (Exception ex)
                {
                    Logger.log(messageItem: "Error converting " + name + ": " + value + ", of table: " + this.tableName, 
                               exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
                    return null;
                }
            }

            protected DateTime? convertToDateTime(string? name, string? value)
            {
                try
                {
                    DateTime.TryParse(value, out var dateValue);
                    return dateValue;
                }
                catch (Exception ex)
                {
                    Logger.log(messageItem: "Error converting " + name + ": " + value + ", of table: " + this.tableName,
                               exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
                    return null;
                }
            }

            protected bool? convertToBool(string? name, string? value)
            {
                try
                {
                    bool.TryParse(value, out var boolValue);
                    return boolValue;
                }
                catch (Exception ex)
                {
                    Logger.log(messageItem: "Error converting " + name + ": " + value + ", of table: " + this.tableName,
                               exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
                    return null;
                }
            }

            protected decimal? convertToDecimal(string? name, string? value)
            {
                try
                {
                    Decimal.TryParse(value, out var decimalValue);
                    return decimalValue;
                }
                catch (Exception ex)
                {
                    Logger.log(messageItem: "Error converting " + name + ": " + value + ", of table: " + this.tableName,
                               exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
                    return null;
                }
            }
        }

        public abstract Result load(MySqlDataReader reader);

        public abstract Result delete(MySqlDataReader reader);

        public abstract Result save(MySqlDataReader reader);

    }
}
