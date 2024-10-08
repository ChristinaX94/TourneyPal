using MySql.Data.MySqlClient;
using System.Reflection;
using TourneyPal.Commons;
using Result = TourneyPal.Commons.Result;

namespace TourneyPal.SQLManager.DataModels
{
    public abstract class Model : iModel
    {
        public List<ModelRow> rows { get; private set; }

        public Model()
        {
            this.initializeRows();
        }

        public abstract Result load(MySqlDataReader reader);

        public Result initializeRows()
        {
            Result result = new Result();
            try
            {
                this.rows = new List<ModelRow>();
                result.success = true;
            }
            catch (Exception ex)
            {
                result.success = false;
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return result;
        }

        public Result save()
        {
            Result result = new Result();
            try
            {

                result = this.validate();
                if (!result.success)
                {
                    result.success = false;
                    return result;
                }

                return result;
            }
            catch (Exception ex)
            {
                result.success = false;
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return result;
        }

        public Result validate()
        {
            Result result = new Result();
            try
            {
                foreach (var row in rows)
                {

                    result = row.validateRow();
                    if (!result.success)
                    {
                        result.success = false;
                        return result;
                    }
                }

            }
            catch (Exception ex)
            {
                result.success = false;
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return result;
        }

    }
}
