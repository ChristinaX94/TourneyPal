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
        public bool load(MySqlDataReader reader);
        
        public bool save();

        public bool validate();

        public bool initializeRows();

    }
}
