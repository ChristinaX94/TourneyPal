namespace TourneyPal.SQLManager.DataModels.SQLTables.Tournament_Host_Sites
{
    public class Tournament_Host_Sites : Model
    {
        public override Tournament_Host_SitesRow initiateRow() => new Tournament_Host_SitesRow(GetType().Name);
    }
}
