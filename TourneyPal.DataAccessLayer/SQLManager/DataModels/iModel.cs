using MySql.Data.MySqlClient;

namespace TourneyPal.SQLManager.DataModels
{
    public interface iModel
    {
        public ModelRow initiateRow();

        public bool load(MySqlDataReader reader);
        
        public bool save();

        public bool validate();
    }
}
