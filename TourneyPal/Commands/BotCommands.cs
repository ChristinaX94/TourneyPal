using DSharpPlus;
using DSharpPlus.Entities;
using DSharp​Plus.SlashCommands;
using System.Reflection;
using TourneyPal.Commons;
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
        }

        
        [SlashCommand("post", "Posts all upcoming Tournaments up to one year")]
        public async Task Post(InteractionContext ctx)
        {
            try
            {
                List<DiscordEmbed> embeds = GetEmbeds(GeneralData.TournamentsData.Where(x => DateOnly.FromDateTime((DateTime)x.StartsAT) >= DateOnly.FromDateTime(DateTime.Now)).OrderBy(x => x.StartsAT).ThenBy(x => x.ID).ToList());
                await setPages(ctx, embeds, ctx.InteractionId).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        [SlashCommand("postOld", "Posts all past Tournaments")]
        public async Task PostOld(InteractionContext ctx)
        {
            try
            {
                List<DiscordEmbed> embeds = GetEmbeds(GeneralData.TournamentsData.Where(x => DateOnly.FromDateTime((DateTime)x.StartsAT) < DateOnly.FromDateTime(DateTime.Now)).OrderBy(x => x.StartsAT).ThenBy(x => x.ID).ToList());
                await setPages(ctx, embeds, ctx.InteractionId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        [SlashCommand("postAll", "Posts all registered Tournaments in DB")]
        public async Task PostAll(InteractionContext ctx)
        {
            try
            {
                List<DiscordEmbed> embeds = GetEmbeds(GeneralData.TournamentsData.OrderBy(x => DateOnly.FromDateTime((DateTime)x.StartsAT)).ThenBy(x => x.ID).ToList());

                var upcomingTournament = GeneralData.TournamentsData.OrderBy(x => DateOnly.FromDateTime((DateTime)x.StartsAT)).ThenBy(x => x.ID).FirstOrDefault(x => DateOnly.FromDateTime((DateTime)x.StartsAT) >= DateOnly.FromDateTime(DateTime.Now));
                var upcomingTournamentPos = 0;
                if (upcomingTournament != null)
                {
                    upcomingTournamentPos = GeneralData.TournamentsData.IndexOf(upcomingTournament);
                }

                await setPages(ctx, embeds, ctx.InteractionId, upcomingTournamentPos).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        [SlashCommand("tourneyIn", "Posts all registered Tournaments in Specific Country")]
        public async Task PostTourneyIn(InteractionContext ctx, [Option("country", "2 character Country Code")] string country)
        {
            try
            {
                if (string.IsNullOrEmpty(country) || country.ToArray().Length!=2)
                {
                    await ctx.CreateResponseAsync("Country must contain 2 characters!").ConfigureAwait(false);
                }
                
                List<DiscordEmbed> embeds = GetEmbeds(GeneralData.TournamentsData.Where(x => x.CountryCode.Equals(country) && DateOnly.FromDateTime((DateTime)x.StartsAT) >= DateOnly.FromDateTime(DateTime.Now)).OrderBy(x => x.StartsAT).ThenBy(x => x.ID).ToList());
                await setPages(ctx, embeds, ctx.InteractionId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }
        

        private async Task setPages(InteractionContext ctx, List<DiscordEmbed> embeds, ulong interactionID = 0, int selectedPos=0)
        {
            try
            {
                var pos = selectedPos;
                if (embeds.Count == 0)
                {
                    await ctx.CreateResponseAsync("No Data").ConfigureAwait(false);
                }

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent(ctx.Member.Mention)
                    .AddMention(mention: new UserMention(ctx.User))
                    .AddEmbed(embeds[pos])
                    .AddComponents(getButtons(ctx.Client, embeds[pos].Fields[0].Value)))
                    .ConfigureAwait(false);

                ctx.Client.ComponentInteractionCreated += async (s, e) =>
                {
                    if(e.Message?.Embeds == null)
                    {
                        return;
                    }

                    if (interactionID != e.Message.Interaction.Id)
                    {
                        return;
                    }

                    var selectedEmbed = embeds.FirstOrDefault(x => x.Fields[x.Fields.Count - 1].Value.Equals(e.Message.Embeds.FirstOrDefault().Fields[x.Fields.Count - 1].Value));
                    pos = embeds.IndexOf(selectedEmbed);
                    if (pos==-1)
                    {
                        return;
                    }

                    if (e.Id.Equals("Righty") && pos < embeds.Count - 1)
                    {
                        pos++;
                    }
                    if (e.Id.Equals("Lefty") && pos > 0)
                    {
                        pos--;
                    }

                    await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder()
                        .AddEmbed(embeds[pos])
                        .AddComponents(getButtons(ctx.Client, embeds[pos].Fields[0].Value)));
                };

            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        private List<DiscordComponent> getButtons(DiscordClient client, string url)
        {
            try
            {
                return new List<DiscordComponent>(){
                        new DiscordButtonComponent(ButtonStyle.Secondary, "Lefty", string.Empty, emoji: new(DiscordEmoji.FromName(client, ":point_left:"))),
                        new DiscordLinkButtonComponent(url, "Go!"),
                        new DiscordButtonComponent(ButtonStyle.Secondary, "Righty", string.Empty, emoji: new(DiscordEmoji.FromName(client, ":point_right:"))) };
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return new List<DiscordComponent>() { };
        }

        private List<DiscordEmbed> GetEmbeds(List<TournamentData> tourneysSelected)
        {
            List<DiscordEmbed> embeds = new List<DiscordEmbed>();
            try
            {
                foreach (var tourney in tourneysSelected)
                {
                    DiscordEmbed embed = new DiscordEmbedBuilder
                    {
                        Title = tourney.Game,
                        Description = tourney.Name,
                        Color = DiscordColor.Purple,

                    }
                    .AddField("Site: ", tourney.HostSite + tourney.URL)
                    .AddField("Online: ", tourney.Online == null ? " - " : tourney.Online == true ? "Yes" : "No")
                    .AddField("Country Code: ", string.IsNullOrEmpty(tourney.CountryCode)? " - ":tourney.CountryCode)
                    .AddField("Location: ", tourney.VenueAddress + ", " + tourney.City + ", " + tourney.AddrState + ", " + tourney.CountryCode)
                    .AddField("Date (dd/mm/yyyy): ", tourney.StartsAT == null ? " - " : tourney.StartsAT.Value.Date.ToString("dd/MM/yyyy"))
                    .AddField("Streams: ", tourney.Streams == null || tourney.Streams.Count == 0 ? " - " : string.Join("\n", tourney.Streams))
                    .AddField("ID: ", tourney.ID.ToString())
                    .WithFooter($"Tournament {1 + tourneysSelected.IndexOf(tourney)}/{tourneysSelected.Count}")
                    .Build();

                    embeds.Add(embed);
                }
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
                embeds = new List<DiscordEmbed>();
            }
            
            return embeds;
        }
    }

}
