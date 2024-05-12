using MySql.Data.MySqlClient;
using Result = TourneyPal.Commons.Result;

namespace TourneyPal.SQLManager.DataModels
{
    public abstract class Model : iModel
    {
        public List<ModelRow> rows { get; private set; }

        public Model()
        {
            this.rows = new List<ModelRow>();
        }

        public abstract Result load(MySqlDataReader reader);

        public virtual Result save()
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
                result.message = ex.Message;
            }
            return result;
        }

        public virtual Result validate()
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
                result.message = ex.Message;
            }
            return result;
        }

    }
}
