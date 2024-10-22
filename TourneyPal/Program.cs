using Microsoft.Extensions.DependencyInjection;
using TourneyPal.Bot.BotHandling;
using TourneyPal.BotHandling;
using TourneyPal.Service;

namespace TourneyPal
{
    internal class Program
    {
        public static ServiceProvider serviceProvider = new ServiceCollection().AddSingleton<ITourneyPalService, TourneyPalService>().BuildServiceProvider();
        public static ITourneyPalService service = serviceProvider.GetService<ITourneyPalService>();
        public static async Task Main(string[] args)
        {
            try
            {
                service.initializeData();
                var tourneyPal = new BotHandling.Bot();
                BotCommons.service = service;
                await Task.WhenAll(tourneyPal.runAsync());
                //var apiHandler = new ApiHandler();
                //await Task.WhenAll(tourneyPal.runAsync(), apiHandler.runAsync());
            }
            catch (Exception ex)
            {
                //Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                //           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        public static void Initialize()
        {
            //TourneyPal.DataHandling.DataObjects.GeneralData.GeneralDataInitialize();
        }
    }
}
