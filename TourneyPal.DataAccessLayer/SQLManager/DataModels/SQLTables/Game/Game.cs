namespace TourneyPal.SQLManager.DataModels.SQLTables.Game
{
    public class Game : Model
    {
        public override GameRow initiateRow() => new GameRow(GetType().Name);
    }

}
