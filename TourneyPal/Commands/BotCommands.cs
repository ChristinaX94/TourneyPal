using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.EventHandling;
using DSharpPlus.Interactivity.Extensions;
using DSharp​Plus.SlashCommands;
using System.Reflection;
using TourneyPal.Commons;

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

        [SlashCommand("postOld", "To be deleted")]
        public async Task PostOld(InteractionContext ctx)
        {
            try
            {
                List<DiscordEmbed> embeds = new List<DiscordEmbed>();
                var server = ctx.Guild;
                var member = ctx.Member;

                if (TourneyPal.DataHandling.DataObjects.GeneralData.TournamentsData.Count==0)
                {
                    await ctx.CreateResponseAsync("No Data").ConfigureAwait(false);
                }

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent(member.Mention).AddMention(mention: new UserMention(ctx.User))).ConfigureAwait(false);
                foreach (var tourney in TourneyPal.DataHandling.DataObjects.GeneralData.TournamentsData)
                {
                    DiscordEmbed embed = new DiscordEmbedBuilder
                    {
                        Title = tourney.Game,
                        Description = tourney.Name,
                        Color = DiscordColor.Purple,

                    }
                    .AddField("Site: ", tourney.HostSite + tourney.URL)
                    .AddField("Online: ", tourney.Online == null ? " - " : tourney.Online == true ? "Yes" : "No")
                    .AddField("Location: ", tourney.VenueAddress + ", " + tourney.City + ", "+ tourney.AddrState + ", " + tourney.CountryCode)
                    .AddField("Date (dd/mm/yyyy): ", tourney.StartsAT == null ? " - " : tourney.StartsAT.Value.Date.ToString("dd/MM/yyyy"))
                    .AddField("Streams: ", tourney.Streams==null|| tourney.Streams.Count==0 ? " - " : string.Join("\n", tourney.Streams))
                    .AddField("ID: ", tourney.ID.ToString())
                    .Build();

                    embeds.Add(embed);
                }

                var buttons = new PaginationButtons()
                {
                    SkipLeft = new(ButtonStyle.Secondary, "leftskip", "1", false),
                    Left = new(ButtonStyle.Primary, "left", null, false, new(DiscordEmoji.FromName(ctx.Client, ":point_left:"))),
                    Stop = new(ButtonStyle.Secondary, "stop", ".", true),
                    Right = new(ButtonStyle.Primary, "right", null, false, new(DiscordEmoji.FromName(ctx.Client, ":point_right:"))),
                    SkipRight = new(ButtonStyle.Secondary, "rightskip", embeds.Count.ToString(), false)
                };

                var pages = GeneratePagesInEmbed(embeds);
                await ctx.Channel.SendPaginatedMessageAsync(user:ctx.Member, 
                                                            pages: pages, 
                                                            buttons: buttons, 
                                                            behaviour: PaginationBehaviour.WrapAround, 
                                                            deletion: ButtonPaginationBehavior.DeleteButtons).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
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
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
                list = new List<Page>();
            }
            return list;
        }


        [SlashCommand("post", "Posts all available Tournaments up to one year")]
        public async Task Post(InteractionContext ctx)
        {
            try
            {
                var pos = 0;
                List<DiscordEmbed> embeds = new List<DiscordEmbed>();
                var server = ctx.Guild;
                var member = ctx.Member;

                if (TourneyPal.DataHandling.DataObjects.GeneralData.TournamentsData.Count == 0)
                {
                    await ctx.CreateResponseAsync("No Data").ConfigureAwait(false);
                }

                
                foreach (var tourney in TourneyPal.DataHandling.DataObjects.GeneralData.TournamentsData)
                {
                    DiscordEmbed embed = new DiscordEmbedBuilder
                    {
                        Title = tourney.Game,
                        Description = tourney.Name,
                        Color = DiscordColor.Purple,

                    }
                    .AddField("Site: ", tourney.HostSite + tourney.URL)
                    .AddField("Online: ", tourney.Online == null ? " - " : tourney.Online == true ? "Yes" : "No")
                    .AddField("Location: ", tourney.VenueAddress + ", " + tourney.City + ", " + tourney.AddrState + ", " + tourney.CountryCode)
                    .AddField("Date (dd/mm/yyyy): ", tourney.StartsAT == null ? " - " : tourney.StartsAT.Value.Date.ToString("dd/MM/yyyy"))
                    .AddField("Streams: ", tourney.Streams == null || tourney.Streams.Count == 0 ? " - " : string.Join("\n", tourney.Streams))
                    .AddField("ID: ", tourney.ID.ToString())
                    .WithFooter($"Tournament {1+TourneyPal.DataHandling.DataObjects.GeneralData.TournamentsData.IndexOf(tourney)}/{TourneyPal.DataHandling.DataObjects.GeneralData.TournamentsData.Count}")
                    .Build();

                    embeds.Add(embed);
                }

                var leftButton = new DiscordButtonComponent(ButtonStyle.Secondary, "Lefty", string.Empty, emoji: new(DiscordEmoji.FromName(ctx.Client, ":point_left:")));
                var goButton = new DiscordLinkButtonComponent(embeds[pos].Fields[0].Value, "Go!");
                var rightButton = new DiscordButtonComponent(ButtonStyle.Secondary, "Righty", string.Empty, emoji: new(DiscordEmoji.FromName(ctx.Client, ":point_right:")));

                var buttons = new DiscordComponent[] { leftButton, goButton, rightButton };

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent(member.Mention)
                    .AddMention(mention: new UserMention(ctx.User))
                    .AddEmbed(embeds[pos])
                    .AddComponents(buttons))
                    .ConfigureAwait(false);

                ctx.Client.ComponentInteractionCreated += async (s, e) =>
                {
                    if (e.Id.Equals("Righty") && pos < embeds.Count-1)
                    {
                        pos++;
                    }
                    if (e.Id.Equals("Lefty") && pos > 0)
                    {
                        pos--;
                    }
                    buttons[1]= new DiscordLinkButtonComponent(embeds[pos].Fields[0].Value, "Go!");

                    await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage,new DiscordInteractionResponseBuilder().AddEmbed(embeds[pos]).AddComponents(buttons));
                };
            }
            catch (Exception ex)
            {
                Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }


    }

}
