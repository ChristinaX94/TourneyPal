using MySql.Data.MySqlClient;
using System.Reflection;
using TourneyPal.Commons;

namespace TourneyPal.SQLManager.DataModels
{
    public abstract class Model : iModel
    {
        public List<ModelRow> Rows { get; private set; } = new List<ModelRow>();

        public abstract ModelRow initiateRow();

        public bool load(MySqlDataReader reader)
        {
            bool result = false;
            try
            {
                while (reader.Read())
                {
                    var row = initiateRow();

                    result = row.loadRow(reader);
                    if (!result)
                    {
                        return result;
                    }

                    Rows.Add(row);
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

        public bool save()
        {
            bool result = false;
            try
            {
                result = this.validate();
                if (!result)
                {
                    result = false;
                    return result;
                }

                return result;
            }
            catch (Exception ex)
            {
                result = false;
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return result;
        }

        public bool validate()
        {
            bool result = false;
            try
            {
                foreach (var row in Rows)
                {
                    result = row.validateRow();
                    if (!result)
                    {
                        result = false;
                        return result;
                    }
                }
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
