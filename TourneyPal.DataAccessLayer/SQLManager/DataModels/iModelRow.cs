using MySql.Data.MySqlClient;

namespace TourneyPal.SQLManager.DataModels
{
    public interface iModelRow
    {
        public bool loadRow(MySqlDataReader reader);
        public bool validateRow();
        public int? convertToInt(string? name, string? value);
        public DateTime? convertToDateTime(string? name, string? value);
        public bool? convertToBool(string? name, string? value);
        public decimal? convertToDecimal(string? name, string? value);
    }
}
