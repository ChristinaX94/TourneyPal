using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using System.Reflection;
using TourneyPal.BotHandling;
using TourneyPal.Commons.DataObjects;
using static TourneyPal.Commons.Common;
using TourneyPal.Bot.Commands.CommandExecution;

namespace TourneyPal.Bot.Commands.CommandService
{
    public class BotCommandService : IBotCommandService
    {
        public async Task Ping(InteractionContext ctx)
        {
            try
            {
                await ctx.CreateResponseAsync("Pong from Bot").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        public async Task PingService(InteractionContext ctx)
        {
            try
            {
                await ctx.CreateResponseAsync(BotCommons.DataService.Ping()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        public async Task Help(InteractionContext ctx)
        {
            try
            {
                var initialResponse = "Thank you for using TourneyPal!";
                var navigationHelpResponse = "Use /general getAvailableGames, /general getAvailableCommands, to navigate!";
                var issueResponse = "For any issues, please send an email to tourneyPal@gmail.com.";

                await ctx.CreateResponseAsync(initialResponse + Environment.NewLine 
                    + navigationHelpResponse + Environment.NewLine
                    + issueResponse + Environment.NewLine).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        public async Task Post(Game SelectedGame, InteractionContext ctx)
        {
            try
            {
                List<DiscordEmbed> embeds = BotCommandExecution.GetEmbeds(BotCommons.DataService.getNewTournaments(SelectedGame));
                await BotCommandExecution.setPages(ctx, embeds, ctx.InteractionId).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        public async Task PostOld(Game SelectedGame, InteractionContext ctx)
        {
            try
            {
                List<DiscordEmbed> embeds = BotCommandExecution.GetEmbeds(BotCommons.DataService.getOldTournaments(SelectedGame));
                await BotCommandExecution.setPages(ctx, embeds, ctx.InteractionId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        public async Task PostAll(Game SelectedGame, InteractionContext ctx)
        {
            try
            {
                List<TournamentData> tournaments = BotCommons.DataService.getAllTournaments(SelectedGame);
                List<DiscordEmbed> embeds = BotCommandExecution.GetEmbeds(tournaments);

                var upcomingTournament = tournaments.FirstOrDefault(x => x.StartsAT >= getDate());
                var upcomingTournamentPos = 0;
                if (upcomingTournament != null)
                {
                    upcomingTournamentPos = tournaments.IndexOf(upcomingTournament);
                }

                await BotCommandExecution.setPages(ctx, embeds, ctx.InteractionId, upcomingTournamentPos).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        public async Task PostTourneyIn(Game SelectedGame, InteractionContext ctx, string country)
        {
            try
            {
                if (string.IsNullOrEmpty(country) || country.ToArray().Length != 2)
                {
                    await ctx.CreateResponseAsync("Country must contain 2 characters!").ConfigureAwait(false);
                    return;
                }

                List<DiscordEmbed> embeds = BotCommandExecution.GetEmbeds(BotCommons.DataService.getNewTournamentsByCountryCode(SelectedGame, country));

                await BotCommandExecution.setPages(ctx, embeds, ctx.InteractionId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        public async Task SearchTournament(Game SelectedGame, InteractionContext ctx, string term)
        {
            try
            {
                if (string.IsNullOrEmpty(term))
                {
                    await ctx.CreateResponseAsync("Search term is empty!").ConfigureAwait(false);
                    return;
                }

                var data = BotCommons.DataService.searchTournaments(SelectedGame, term);
                if (data.Count == 0)
                {
                    await ctx.CreateResponseAsync("No Data").ConfigureAwait(false);
                    return;
                }

                if (data.Count == 1)
                {
                    List<DiscordEmbed> fullEmbeds = BotCommandExecution.GetEmbeds(data);
                    await BotCommandExecution.setPages(ctx, fullEmbeds);
                    return;
                }

                var embeds = BotCommandExecution.GetDataEmbeds(data);

                await BotCommandExecution.setDataPages(ctx, embeds, ctx.InteractionId).ConfigureAwait(false);

                await BotCommandExecution.SetMessageFollowUp(data, ctx).ConfigureAwait(false);


            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        public async Task RegisterChallongeTournament(InteractionContext ctx, string URL)
        {
            try
            {
                var canCall = await BotCommandExecution.ValidatePermissions(ctx, Permissions.Administrator);
                if (!canCall)
                {
                    return;
                }

                if (string.IsNullOrEmpty(URL))
                {
                    await ctx.CreateResponseAsync("Invalid URL!").ConfigureAwait(false);
                    return;
                }
                var embed = BotCommons.DataService.getChallongeTournamentByURL(URL).Result;
                if (embed == null)
                {
                    await ctx.CreateResponseAsync("No tournament found!").ConfigureAwait(false);
                    return;
                }
                List<DiscordEmbed> embeds = BotCommandExecution.GetEmbeds(new List<TournamentData>() { embed });
                await BotCommandExecution.setPages(ctx, embeds, ctx.InteractionId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        public async Task GetAvailableGames(InteractionContext ctx)
        {
            try
            {
                 await BotCommandExecution.GetAvailableGames(ctx).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        public async Task GetAvailableCommands(InteractionContext ctx)
        {
            try
            {
                await BotCommandExecution.GetAvailableCommands(ctx).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        public async Task RegisterServerGames(InteractionContext ctx)
        {
            try
            {
                var canCall = await BotCommandExecution.ValidatePermissions(ctx, Permissions.Administrator);
                if (!canCall)
                {
                    return;
                }

                await ctx.CreateResponseAsync("Set server games!").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        public async Task RemoveServerGames(InteractionContext ctx)
        {
            try
            {
                var canCall = await BotCommandExecution.ValidatePermissions(ctx, Permissions.Administrator);
                if (!canCall)
                {
                    return;
                }

                await ctx.CreateResponseAsync("Removing all server games!").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }
    }
}
