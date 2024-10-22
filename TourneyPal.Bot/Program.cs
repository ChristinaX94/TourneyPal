using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TourneyPal.BotHandling;
using TourneyPal.Service;

namespace TourneyPal
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                ServiceProvider serviceProvider = new ServiceCollection().AddSingleton<ITourneyPalService, TourneyPalService>().BuildServiceProvider();
                BotCommons.service = serviceProvider.GetService<ITourneyPalService>()!;
                BotCommons.service.InitializeData();

                await Task.WhenAll(new Bot().runAsync(), BotCommons.service.RunApiHandler());
            }
            catch (Exception ex)
            {
                Console.WriteLine("FoundInItem: "+MethodBase.GetCurrentMethod() + ", ExceptionMessageItem: "+ex.Message + " -- " + ex.StackTrace);
            }
        }
    }
}
