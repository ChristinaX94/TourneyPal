using MySql.Data.MySqlClient;
using System.Reflection;
using TourneyPal.Commons;

namespace TourneyPal.SQLManager.DataModels
{
    public abstract class Model : iModel
    {
        public List<ModelRow> rows { get; private set; }

        public Model()
        {
            this.initializeRows();
        }

        public abstract bool load(MySqlDataReader reader);

        public bool initializeRows()
        {
            bool result = false;
            try
            {
                this.rows = new List<ModelRow>();
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
                foreach (var row in rows)
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
