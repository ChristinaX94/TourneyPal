using TourneyPal.BotHandling;
using TourneyPal.DataHandling;
using TourneyPal.DataHandling.DataObjects;
using TourneyPal.SQLManager;
using TourneyPal.SQLManager.DataModels.SQLTables.Game;
using static System.Net.Mime.MediaTypeNames;

namespace TourneyPal
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                Initialize();
                //Test();
                var tourneyPal = new Bot();
                var apiHandler = new ApiHandler();
                await Task.WhenAll(tourneyPal.runAsync(), apiHandler.runAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex.Message);
            }
        }

        public static void Initialize()
        {
            TourneyPal.DataHandling.DataObjects.GeneralData.GeneralDataInitialize();
        }

        public static void Test()
        {
            SQLHandler sql = new SQLHandler();
            var item = (Game)sql.loadModelData(new Game());
        }
    }
}
