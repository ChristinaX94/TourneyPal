namespace TourneyPal.SQLManager.DataModels.SQLTables.Errorlogs
{
    public class Errorlogs : Model
    {
        public override ErrorlogsRow initiateRow() => new ErrorlogsRow(GetType().Name);
    }

}
