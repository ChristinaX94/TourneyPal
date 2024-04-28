using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;
using TourneyPal.GeneralData;

namespace TourneyPal.SQLManager.DataModels
{
    public abstract class ModelRow: iModelRow
    {
        public ModelRow(string? tableName)
        {
            this.tableName = tableName;
        }

        protected string? tableName { get; set; }
        public int ID { get; private set; }
        public DateTime? DateUpdated { get; private set; }
        public DateTime? DateInserted { get; private set; }

        public virtual Result loadRow(MySqlDataReader reader)
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
                this.ID = (int)id;

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

                result.success = true;
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }
            return result;
        }

        public virtual Result validateRow()
        {
            Result result = new Result();
            try
            {
                if(this.DateUpdated == null)
                {
                    result.success = false;
                    result.message = nameof(this.DateUpdated) + ", of table: " + this.tableName + "-- Cannot be null";
                }

                if (this.DateInserted == null)
                {
                    result.success = false;
                    result.message = nameof(this.DateInserted) + ", of table: " + this.tableName + "-- Cannot be null";
                }
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }
            return result;
        }

        public Result insertNewRowData()
        {
            Result result = new Result();
            try
            {
                this.DateInserted = General.getDate();
                this.DateUpdated = General.getDate();
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }
            return result;
        }

        public Result insertRowData()
        {
            Result result = new Result();
            try
            {
                this.DateUpdated = General.getDate();
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }
            return result;
        }

        public int? convertToInt(string? name, string? value)
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

        public DateTime? convertToDateTime(string? name, string? value)
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

        public bool? convertToBool(string? name, string? value)
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

        public decimal? convertToDecimal(string? name, string? value)
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
}
