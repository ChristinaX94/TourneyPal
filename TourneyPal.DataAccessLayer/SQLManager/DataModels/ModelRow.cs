using MySql.Data.MySqlClient;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Policy;
using TourneyPal.Commons;

namespace TourneyPal.SQLManager.DataModels
{
    public abstract class ModelRow: iModelRow
    {
        public ModelRow(string? tableName)
        {
            this.tableName = tableName;
            this.isModified = false;
        }

        [IgnoreDataMember]
        protected string? tableName { get; set; }

        [IgnoreDataMember]
        public int ID { get; private set; }

        [IgnoreDataMember]
        public bool isModified { get; set; }

        public DateTime? DateUpdated { get; private set; }
        public DateTime? DateInserted { get; private set; }


        public virtual bool loadRow(MySqlDataReader reader)
        {
            bool result = false;
            try
            {
                //ID
                var id = convertToInt(nameof(this.ID), reader[nameof(this.ID)]?.ToString());
                if (id == null)
                {
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(ID) + ", of table: " + tableName + "-- Could not be loaded");
                    result = false;
                    return result;
                }
                this.ID = (int)id;

                //DateUpdated
                var dateUpdated = convertToDateTime(nameof(this.DateUpdated), reader[nameof(this.DateUpdated)]?.ToString());
                if (dateUpdated == null)
                {
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(DateUpdated) + ", of table: " + tableName + "-- Could not be loaded");
                    result = false;
                    return result;
                }
                this.DateUpdated = (DateTime)dateUpdated;

                //DateInserted
                var dateInserted = convertToDateTime(nameof(this.DateInserted), reader[nameof(this.DateInserted)]?.ToString());
                if (dateInserted == null)
                {
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(), messageItem: nameof(DateInserted) + ", of table: " + tableName + "-- Could not be loaded");
                    result = false;
                    return result;
                }
                this.DateInserted = (DateTime)dateInserted;

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

        public virtual bool validateRow()
        {
            bool result = false;
            try
            {
                if(this.DateUpdated == null)
                {
                    result = false;
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                               messageItem: nameof(this.DateUpdated) + ", of table: " + this.tableName + "-- Cannot be null");
                }

                if (this.DateInserted == null)
                {
                    result = false;
                    Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                               messageItem: nameof(this.DateInserted) + ", of table: " + this.tableName + "-- Cannot be null");
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

        public bool insertNewRowData()
        {
            bool result = false;
            try
            {
                this.DateInserted = Common.getDate();
                this.DateUpdated = Common.getDate();
            }
            catch (Exception ex)
            {
                result = false;
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return result;
        }

        public bool updateRowData()
        {
            bool result = false;
            try
            {
                this.DateUpdated = Common.getDate();
            }
            catch (Exception ex)
            {
                result = false;
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
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
                           foundInItem: MethodBase.GetCurrentMethod(),
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
                           foundInItem: MethodBase.GetCurrentMethod(),
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
                           foundInItem: MethodBase.GetCurrentMethod(),
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
                           foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
                return null;
            }
        }

        public void setNewID(int newID)
        {
            this.ID = newID;
        }
    }
}
