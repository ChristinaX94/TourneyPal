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

        [SlashCommand("registerServerGames", "Set prefered games - for commands and announcements.")]
        public async Task RegisterServerGames(InteractionContext ctx)
        {
            try
            {
                await BotCommons.CommandService.RegisterServerGames(ctx).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        [SlashCommand("removeservergames", "Removes commands and announcements of all available games.")]
        public async Task RemoveServerGames(InteractionContext ctx)
        {
            try
            {
                await BotCommons.CommandService.RemoveServerGames(ctx).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }
        
    }
}
