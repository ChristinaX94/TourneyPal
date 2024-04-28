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

        public abstract Result delete(MySqlDataReader reader);

        public virtual Result save(MySqlDataReader reader)
        {
            Result result = new Result();
            try
            {
                if (this.rows == null ||
                   this.rows.Count == 0)
                {
                    result.success = true;
                    result.message = "Nothing to save on " + GetType().Name;
                    return result;
                }

                foreach (var row in rows)
                {
                    result = row.ID == 0 ? row.insertNewRowData() : row.insertRowData();
                    if (!result.success)
                    {
                        result.success = false;
                        return result;
                    }

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
