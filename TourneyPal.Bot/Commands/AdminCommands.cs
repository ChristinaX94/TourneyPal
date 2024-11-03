using DSharpPlus.SlashCommands;
using System.Reflection;
using TourneyPal.BotHandling;

namespace TourneyPal.Bot.Commands
{
    [SlashCommandGroup("admin", "List of admin commands")]
    public class AdminCommands : ApplicationCommandModule
    {
        [SlashCommand("registerChallongeTournament", "Searches a Challonge tournament based on URL and it gets registered.")]
        public async Task RegisterChallongeTournament(InteractionContext ctx, [Option("URL", "Url of Tournament")] string URL)
        {
            try
            {
                await BotCommons.CommandService.RegisterChallongeTournament(ctx, URL).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }
    }
}
