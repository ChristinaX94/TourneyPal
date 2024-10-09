using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.Commons;

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
