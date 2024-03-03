
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharp​Plus.SlashCommands;
using TourneyPal.DataHandling.DataObjects;

namespace TourneyPal
{
    public class BotCommands : ApplicationCommandModule
    {
        public BotCommands()
        {
        }

        [SlashCommand("ping", "pings bot")]
        public async Task Ping(InteractionContext ctx)
        {
            var server = ctx.Guild;
            var member = ctx.Member;

            Console.WriteLine("server: " + server);
            Console.WriteLine("member: " + member);

            await ctx.CreateResponseAsync("Pong").ConfigureAwait(false);
            //await ctx.Channel.SendMessageAsync("Pong").ConfigureAwait(false);
        }

        [SlashCommand("post", "Posts all available Tournaments up to one year")]
        public async Task Post(InteractionContext ctx)
        {
            List<DiscordEmbed> embeds = new List<DiscordEmbed>();
            var server = ctx.Guild;
            var member = ctx.Member;

            Console.WriteLine("server: " + server);
            Console.WriteLine("member: " + member);

            if (GeneralData.Tournaments.Count==0)
            {
                await ctx.CreateResponseAsync("No Data").ConfigureAwait(false);
            }

            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(". . .")).ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync(member.Mention).ConfigureAwait(false);
            foreach (var tourney in GeneralData.Tournaments)
            {
                DiscordEmbed embed = new DiscordEmbedBuilder
                {
                    Title = tourney.Game,
                    Description = tourney.Name,
                    Color = DiscordColor.Purple,

                }
                .AddField("Site: ", tourney.TournamentHostSite+tourney.URL)
                .AddField("Online: ", tourney.Online == null ? " - " : tourney.Online == true ? "Yes" : "No")
                .AddField("Location: ", tourney.VenueAddress + ", " + tourney.City + ", "+ tourney.AddrState + ", " + tourney.CountryCode)
                .AddField("Date (dd/mm/yyyy): ", tourney.StartsAT == null ? " - " : tourney.StartsAT.Value.Date.ToString("dd/MM/yyyy"))
                .AddField("Streams: ", tourney.Streams==null? " - " : string.Join("\n", tourney.Streams))
                .AddField("ID: ", tourney.ID.ToString())
                .Build();

                embeds.Add(embed);
            }

            var pages = GeneratePagesInEmbed(embeds);
            
            await ctx.Channel.SendPaginatedMessageAsync(ctx.Member, pages, deletion: ButtonPaginationBehavior.DeleteButtons).ConfigureAwait(false);
            
        }

        private IEnumerable<Page> GeneratePagesInEmbed(List<DiscordEmbed> embeds)
        {
            List<Page> list = new List<Page>();
            try
            {
                var count = 1;
                foreach(var embed in embeds)
                {
                    list.Add(new Page("", new DiscordEmbedBuilder(embed).WithFooter($"Tournament {count}/{embeds.Count}")));
                    count++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION: " + ex.Message);
                list = new List<Page>();
            }
            return list;
        }
    }

}
