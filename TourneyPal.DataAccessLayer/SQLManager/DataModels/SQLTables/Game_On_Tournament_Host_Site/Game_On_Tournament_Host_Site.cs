namespace TourneyPal.SQLManager.DataModels.SQLTables.Game_On_Tournament_Host_Site
{
    public class Game_On_Tournament_Host_Site : Model
    {
        public override Game_On_Tournament_Host_SiteRow initiateRow() => new Game_On_Tournament_Host_SiteRow(GetType().Name);
    }
}
