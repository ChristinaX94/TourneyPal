using DSharpPlus;
using DSharpPlus.Entities;
using DSharp​Plus.SlashCommands;
using System.Reflection;
using TourneyPal.Commons.DataObjects;

namespace TourneyPal.BotHandling
{
    public class BotCommands : ApplicationCommandModule
    {
        [SlashCommand("ping", "pings bot")]
        public async Task Ping(InteractionContext ctx)
        {
            var server = ctx.Guild;
            var member = ctx.Member;

            Console.WriteLine("server: " + server);
            Console.WriteLine("member: " + member);

            await ctx.CreateResponseAsync("Pong from Bot").ConfigureAwait(false);
        }

        [SlashCommand("pingService", "pings bot")]
        public async Task PingService(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(BotCommons.service.Ping()).ConfigureAwait(false);
        }

        [SlashCommand("help", "Get a list of all commands")]
        public async Task Help(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync("/post").ConfigureAwait(false);
        }

        [SlashCommand("post", "Posts all upcoming Tournaments up to one year")]
        public async Task Post(InteractionContext ctx)
        {
            try
            {
                List<DiscordEmbed> embeds = BotCommons.GetEmbeds(BotCommons.service.getNewTournaments());
                await BotCommons.setPages(ctx, embeds, ctx.InteractionId).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                BotCommons.service.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        [SlashCommand("postOld", "Posts all past Tournaments")]
        public async Task PostOld(InteractionContext ctx)
        {
            try
            {
                List<DiscordEmbed> embeds = BotCommons.GetEmbeds(BotCommons.service.getOldTournaments());
                await BotCommons.setPages(ctx, embeds, ctx.InteractionId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.service.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        [SlashCommand("postAll", "Posts all registered Tournaments in DB")]
        public async Task PostAll(InteractionContext ctx)
        {
            try
            {
                List<TournamentData> tournaments = BotCommons.service.getAllTournaments();
                List<DiscordEmbed> embeds = BotCommons.GetEmbeds(tournaments);

                var upcomingTournament = tournaments.FirstOrDefault(x => x.StartsAT >= Common.getDate());
                var upcomingTournamentPos = 0;
                if (upcomingTournament != null)
                {
                    upcomingTournamentPos = tournaments.IndexOf(upcomingTournament);
                }

                await BotCommons.setPages(ctx, embeds, ctx.InteractionId, upcomingTournamentPos).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.service.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        [SlashCommand("tourneyIn", "Posts all registered Tournaments in Specific Country")]
        public async Task PostTourneyIn(InteractionContext ctx, [Option("country", "2 character Country Code")] string country)
        {
            try
            {
                if (string.IsNullOrEmpty(country) || country.ToArray().Length != 2)
                {
                    await ctx.CreateResponseAsync("Country must contain 2 characters!").ConfigureAwait(false);
                    return;
                }

                List<DiscordEmbed> embeds = BotCommons.GetEmbeds(BotCommons.service.getNewTournamentsByCountryCode(country));

                await BotCommons.setPages(ctx, embeds, ctx.InteractionId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.service.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        [SlashCommand("searchTournament", "Searches a tournament based on title and shows results that are a match")]
        public async Task SearchTournament(InteractionContext ctx, [Option("SearchTerm", "Use a search term")] string term)
        {
            try
            {
                if (string.IsNullOrEmpty(term))
                {
                    await ctx.CreateResponseAsync("Search term is empty!").ConfigureAwait(false);
                    return;
                }

                var data = BotCommons.service.searchTournaments(term);
                if (data.Count == 0)
                {
                    await ctx.CreateResponseAsync("No Data").ConfigureAwait(false);
                    return;
                }

                if (data.Count == 1)
                {
                    List<DiscordEmbed> fullEmbeds = BotCommons.GetEmbeds(data);
                    await BotCommons.setPages(ctx, fullEmbeds);
                    return;
                }

                var embeds = BotCommons.GetDataEmbeds(data);

                await BotCommons.setDataPages(ctx, embeds, ctx.InteractionId).ConfigureAwait(false);

                await BotCommons.setMessageFollowUp(data, ctx).ConfigureAwait(false);

                
            }
            catch (Exception ex)
            {
                BotCommons.service.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        #region RestrictedCommands
        [SlashCommand("registerChallongeTournament", "Searches a Challonge tournament based on URL and it gets registered.")]
        public async Task RegisterChallongeTournament(InteractionContext ctx, [Option("URL", "Url of Tournament")] string URL)
        {
            try
            {
                Permissions userPermissions = ctx.Member.Permissions;

                if (!userPermissions.HasPermission(Permissions.Administrator))
                {
                    await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("You don't have the required permissions"));
                    return;
                }

                if (string.IsNullOrEmpty(URL))
                {
                    await ctx.CreateResponseAsync("Invalid URL!").ConfigureAwait(false);
                    return;
                }
                var embed = BotCommons.service.getChallongeTournamentByURL(URL).Result;
                if (embed == null)
                {
                    await ctx.CreateResponseAsync("No tournament found!").ConfigureAwait(false);
                    return;
                }
                List<DiscordEmbed> embeds = BotCommons.GetEmbeds(new List<TournamentData>() { embed });
                await BotCommons.setPages(ctx, embeds, ctx.InteractionId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.service.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }
        #endregion
    }

}
