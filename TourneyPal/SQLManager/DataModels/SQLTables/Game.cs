using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;
using TourneyPal.SQLManager.DataModels;

namespace TourneyPal.SQLManager.DataModels.SQLTables
{
    public class Game : Model
    {
        public List<GameRow> rows { get; private set; }
        public class GameRow : ModelRow
        {
            public GameRow(string? tableName) : base(tableName)
            {
            }

            public string Title { get; private set; }

            public Result loadRow(MySqlDataReader reader)
            {
                Result result = new Result();
                try
                {
                    result = base.loadRow(reader);
                    if (!result.success)
                    {
                        return result;
                    }

                    if (reader[nameof(Title)] == null ||
                        string.IsNullOrEmpty(reader[nameof(Title)].ToString()))
                    {
                        result.success = false;
                        return result;
                    }
                    this.Title = reader[nameof(Title)].ToString();
                }
                catch (Exception ex)
                {
                    result.success = false;
                    result.message = ex.Message;
                }
                return result;
            }

        }

        public override Result load(MySqlDataReader reader)
        {
            Result result = new Result();
            try
            {
                while (reader.Read())
                {
                    GameRow row = new GameRow(GetType().Name);
                    result = row.loadRow(reader);
                    if (!result.success)
                    {
                        return result;
                    }

                    rows.Add(row);
                }
            }
            catch (Exception ex)
            {
                result.success = false;
                result.message = ex.Message;
            }
            return result;

        }

        public override Result delete(MySqlDataReader reader)
        {
            throw new NotImplementedException();
        }

        public override Result save(MySqlDataReader reader)
        {
            throw new NotImplementedException();
        }
    }


}
