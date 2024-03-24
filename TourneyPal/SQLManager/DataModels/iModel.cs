using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;

namespace TourneyPal.SQLManager.DataModels
{
    public interface iModel
    {
        public Result load(MySqlDataReader reader);

        public Result delete(MySqlDataReader reader);

        public Result save(MySqlDataReader reader);
    }
}
