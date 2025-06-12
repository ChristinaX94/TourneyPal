namespace TourneyPal.SQLManager.DataModels.SQLTables.Stream
{
    public class Stream : Model
    {
        public override StreamRow initiateRow() => new StreamRow(GetType().Name);
    }
}