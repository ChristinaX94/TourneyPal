using MySql.Data.MySqlClient;

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
