using DSharpPlus.SlashCommands;
using TourneyPal.BotHandling;

namespace TourneyPal.Bot.Commands.AllGuildsCommands
{
    [SlashCommandGroup("general", "List of general commands")]
    public class GeneralCommands : ApplicationCommandModule
    {
        [SlashCommand("ping", "pings bot")]
        public async Task Ping(InteractionContext ctx)
        {
            await BotCommons.CommandService.Ping(ctx).ConfigureAwait(false);
        }

        [SlashCommand("pingService", "pings bot")]
        public async Task PingService(InteractionContext ctx)
        {
            await BotCommons.CommandService.PingService(ctx).ConfigureAwait(false);
        }

        [SlashCommand("getAvailableGames", "Get a list of all game commands")]
        public async Task GetAvailableGames(InteractionContext ctx)
        {
            await BotCommons.CommandService.GetAvailableGames(ctx).ConfigureAwait(false);
        }

        [SlashCommand("getAvailableCommands", "Get a list of all commands of the server")]
        public async Task GetAvailableCommands(InteractionContext ctx)
        {
            await BotCommons.CommandService.GetAvailableCommands(ctx).ConfigureAwait(false);
        }

        [SlashCommand("help", "Get a list of all commands")]
        public async Task Help(InteractionContext ctx)
        {
            await BotCommons.CommandService.Help(ctx).ConfigureAwait(false);
        }
    }
}
