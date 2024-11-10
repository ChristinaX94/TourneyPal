using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TourneyPal.Bot.Commands.CommandService;
using TourneyPal.BotHandling;
using TourneyPal.DataService;

namespace TourneyPal
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                ServiceProvider serviceProvider = new ServiceCollection()
                    .AddSingleton<ITourneyPalDataService, TourneyPalDataService>()
                    .AddSingleton<IBotCommandService, BotCommandService>()
                    .BuildServiceProvider();
                BotCommons.DataService = serviceProvider.GetService<ITourneyPalDataService>()!;
                BotCommons.CommandService = serviceProvider.GetService<IBotCommandService>()!;

                BotCommons.DataService.InitializeData();

                await Task.WhenAll(new BotConfiguration().runAsync(), BotCommons.DataService.RunApiHandler());
            }
            catch (Exception ex)
            {
                Console.WriteLine("FoundInItem: "+MethodBase.GetCurrentMethod() + ", ExceptionMessageItem: "+ex.Message + " -- " + ex.StackTrace);
            }
        }
    }
}
