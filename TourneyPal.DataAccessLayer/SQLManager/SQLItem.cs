using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourneyPal.SQLManager
{
    public class SQLItem
    {
        public string query { get; private set; }
        public List<MySqlParameter> parameters { get; private set; }

        public SQLItem(string query, List<MySqlParameter> parameters)
        {
            this.query = query;
            this.parameters = parameters;
        }


    }
}
