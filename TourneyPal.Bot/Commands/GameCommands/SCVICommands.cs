using DSharp​Plus.SlashCommands;
using System.Reflection;
using TourneyPal.BotHandling;
using TourneyPal.Commons;

namespace TourneyPal.Bot.Commands.GameCommands
{
    [SlashCommandGroup("SCVI", "List of commands for SoulCalibur VI")]
    public class SCVICommands : ApplicationCommandModule
    {
        public readonly Common.Game SelectedGame = Common.Game.SoulCalibur6;

        [SlashCommand("post", "Posts all upcoming Tournaments up to one year")]
        public async Task Post(InteractionContext ctx)
        {
            try
            {
                await BotCommons.CommandService.Post(SelectedGame, ctx).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        [SlashCommand("postOld", "Posts all past Tournaments")]
        public async Task PostOld(InteractionContext ctx)
        {
            try
            {
                await BotCommons.CommandService.PostOld(SelectedGame, ctx).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        [SlashCommand("postAll", "Posts all registered Tournaments in DB")]
        public async Task PostAll(InteractionContext ctx)
        {
            try
            {
                await BotCommons.CommandService.PostAll(SelectedGame, ctx).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        [SlashCommand("tourneyIn", "Posts all registered Tournaments in Specific Country")]
        public async Task PostTourneyIn(InteractionContext ctx, [Option("country", "2 character Country Code")] string country)
        {
            try
            {
                await BotCommons.CommandService.PostTourneyIn(SelectedGame, ctx, country).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        [SlashCommand("searchTournament", "Searches a tournament based on title and shows results that are a match")]
        public async Task SearchTournament(InteractionContext ctx, [Option("SearchTerm", "Use a search term")] string term)
        {
            try
            {
                await BotCommons.CommandService.SearchTournament(SelectedGame, ctx, term).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }
    }

}
