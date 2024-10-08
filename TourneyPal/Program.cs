using System.Reflection;
using TourneyPal.BotHandling;
using TourneyPal.Commons;
using TourneyPal.DataHandling;
using TourneyPal.DataHandling.DataObjects;
using TourneyPal.SQLManager;
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
                var tourneyPal = new Bot();
                var apiHandler = new ApiHandler();
                await Task.WhenAll(tourneyPal.runAsync(), apiHandler.runAsync());
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        public static void Initialize()
        {
            TourneyPal.DataHandling.DataObjects.GeneralData.GeneralDataInitialize();
        }
    }
}
