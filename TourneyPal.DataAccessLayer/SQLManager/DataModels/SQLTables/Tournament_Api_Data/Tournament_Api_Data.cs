namespace TourneyPal.SQLManager.DataModels.SQLTables.Tournament_Api_Data
{
    public class Tournament_Api_Data : Model
    {
        public override Tournament_Api_DataRow initiateRow() => new Tournament_Api_DataRow(GetType().Name);
    }
}
