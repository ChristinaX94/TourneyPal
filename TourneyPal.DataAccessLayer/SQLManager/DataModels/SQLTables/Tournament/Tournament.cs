namespace TourneyPal.SQLManager.DataModels.SQLTables.Tournament
{
    public class Tournament : Model
    {
        public override TournamentRow initiateRow() => new TournamentRow(GetType().Name);
    }
}
