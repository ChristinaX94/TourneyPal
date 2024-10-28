using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Extensions;
using DSharp​Plus.SlashCommands;
using System.Reflection;
using TourneyPal.Commons.DataObjects;

namespace TourneyPal.BotHandling
{
    public class BotCommands : ApplicationCommandModule
    {
        #region Commands
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

        [SlashCommand("post", "Posts all upcoming Tournaments up to one year")]
        public async Task Post(InteractionContext ctx)
        {
            try
            {
                List<DiscordEmbed> embeds = GetEmbeds(BotCommons.service.getNewTournaments());
                await setPages(ctx, embeds, ctx.InteractionId).ConfigureAwait(false);

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
                List<DiscordEmbed> embeds = GetEmbeds(BotCommons.service.getOldTournaments());
                await setPages(ctx, embeds, ctx.InteractionId).ConfigureAwait(false);
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
                List<DiscordEmbed> embeds = GetEmbeds(tournaments);

                var upcomingTournament = tournaments.FirstOrDefault(x => x.StartsAT >= Common.getDate());
                var upcomingTournamentPos = 0;
                if (upcomingTournament != null)
                {
                    upcomingTournamentPos = tournaments.IndexOf(upcomingTournament);
                }

                await setPages(ctx, embeds, ctx.InteractionId, upcomingTournamentPos).ConfigureAwait(false);
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

                List<DiscordEmbed> embeds = GetEmbeds(BotCommons.service.getNewTournamentsByCountryCode(country));

                await setPages(ctx, embeds, ctx.InteractionId).ConfigureAwait(false);
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
                var embeds = GetDataEmbeds(data);

                await setDataPages(ctx, embeds, ctx.InteractionId).ConfigureAwait(false);

                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder() { Content = "Respond with a number of choice to continue." });

                int choice = -1;
                var result = await ctx.Channel.GetNextMessageAsync(ctx.User);

                //if (!result.TimedOut) await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder() { Content = "Action confirmed." });
                if (!result.TimedOut)
                {
                    var ok = Int32.TryParse(result.Result.Content, out choice);
                    if (!ok || choice<=0 || choice>data.Count) { await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder() { Content = "Invalid input." }); return; }

                    
                    DiscordEmbed embedSelected = GetEmbeds(new List<TournamentData>(){ data[choice-1] }).FirstOrDefault();
                    var url = embedSelected.Fields.FirstOrDefault(x => x.Name.Contains("Site")).Value;
                    await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedSelected).AddComponents(getLinkButton(url)));
                }
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
                List<DiscordEmbed> embeds = GetEmbeds(new List<TournamentData>() { embed });
                await setPages(ctx, embeds, ctx.InteractionId).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.service.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }
        #endregion

        #endregion

        #region ActionHandlers
        private async Task setPages(InteractionContext ctx, List<DiscordEmbed> embeds, ulong interactionID = 0, int selectedPos = 0)
        {
            try
            {
                if (embeds.Count == 0)
                {
                    await ctx.CreateResponseAsync("No Data Found!").ConfigureAwait(false);
                    return;
                }

                var url = embeds[selectedPos].Fields.FirstOrDefault(x => x.Name.Contains("Site"));
                if (string.IsNullOrEmpty(url?.Value))
                {
                    await ctx.CreateResponseAsync("Error! No URL found!").ConfigureAwait(false);
                    return;
                }

                var buttons = embeds.Count > 1 ? getButtons(ctx.Client, url.Value) : getLinkButton(url.Value);

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent(ctx.Member.Mention)
                    .AddMention(mention: new UserMention(ctx.User))
                    .AddEmbed(embeds[selectedPos])
                    .AddComponents(buttons))
                    .ConfigureAwait(false);

                ctx.Client.ComponentInteractionCreated += async (s, e) =>
                {
                    if (interactionID != e.Message.Interaction.Id)
                    {
                        return;
                    }
                    await HandlePageChange(ctx, embeds, e).ConfigureAwait(false);
                };

            }
            catch (Exception ex)
            {
                BotCommons.service.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        private async Task setDataPages(InteractionContext ctx, List<DiscordEmbed> embeds, ulong interactionID = 0, int selectedPos = 0)
        {
            try
            {
                if (embeds.Count == 0)
                {
                    await ctx.CreateResponseAsync("No Data").ConfigureAwait(false);
                    return;
                }

                var component = new DiscordInteractionResponseBuilder()
                    .WithContent(ctx.Member.Mention)
                    .AddMention(mention: new UserMention(ctx.User))
                    .AddEmbed(embeds[selectedPos]);

                if (embeds.Count > 1)
                {
                    component.AddComponents(getButtons(ctx.Client));
                }

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, component).ConfigureAwait(false);

                ctx.Client.ComponentInteractionCreated += async (s, e) =>
                {
                    if (interactionID != e.Message.Interaction.Id)
                    {
                        return;
                    }
                    await HandlePageChange(ctx, embeds, e).ConfigureAwait(false);
                };
            }
            catch (Exception ex)
            {
                BotCommons.service.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        private async Task HandlePageChange(InteractionContext ctx,
                                            List<DiscordEmbed> embeds,
                                            ComponentInteractionCreateEventArgs e)
        {
            try
            {
                if (!await ValidatePageChangeAction(e).ConfigureAwait(false))
                {
                    return;
                }

                int pos = GetNextPagePosition(embeds, e);
                if (pos > -1)
                {
                    var url = embeds[pos].Fields.FirstOrDefault(x => x.Name.Contains("Site"))?.Value;
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder()
                    .AddEmbed(embeds[pos])
                    .AddComponents(getButtons(ctx.Client, url))).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                BotCommons.service.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }
        #endregion

        #region Helpers

        private static int GetNextPagePosition(List<DiscordEmbed> embeds, ComponentInteractionCreateEventArgs e)
        {
            try
            {
                var messageEmbed = e.Message.Embeds.FirstOrDefault();
                var selectedEmbed = embeds.FirstOrDefault(x => x.Fields.LastOrDefault().Value.Equals(messageEmbed.Fields.LastOrDefault().Value));

                if (selectedEmbed == null ||
                    embeds.IndexOf(selectedEmbed) == -1)
                {
                    return -1;
                }

                var pos = embeds.IndexOf(selectedEmbed);

                if (e.Id.Equals("Righty") && pos < embeds.Count - 1)
                {
                    pos++;
                }
                if (e.Id.Equals("Lefty") && pos > 0)
                {
                    pos--;
                }

                return pos;
            }
            catch (Exception ex)
            {
                BotCommons.service.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return -1;
        }

        private static async Task<bool> ValidatePageChangeAction(ComponentInteractionCreateEventArgs e)
        {
            try
            {
                if (e.Message?.Embeds == null)
                {
                    await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage,
                        new DiscordInteractionResponseBuilder().WithContent("Session Expired")).ConfigureAwait(false);
                    return false;
                }

                if (DateTimeOffset.Now.Subtract(e.Message.CreationTimestamp).TotalMinutes > Common.PageButtonTimeoutMinutes)
                {
                    if (e.Message.Embeds.FirstOrDefault() == null)
                    {
                        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage,
                        new DiscordInteractionResponseBuilder().WithContent("Session Expired")).ConfigureAwait(false);
                    }
                    else
                    {
                        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage,
                        new DiscordInteractionResponseBuilder().AddEmbed(e.Message.Embeds.FirstOrDefault())).ConfigureAwait(false);
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                BotCommons.service.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
                return false;
            }
            return true;
        }

        private List<DiscordComponent> getButtons(DiscordClient client, string? url = "")
        {
            try
            {
                var buttonlist = new List<DiscordComponent>(){
                        new DiscordButtonComponent(ButtonStyle.Secondary, "Lefty", string.Empty, emoji: new(DiscordEmoji.FromName(client, ":point_left:"))),
                        new DiscordButtonComponent(ButtonStyle.Secondary, "Righty", string.Empty, emoji: new(DiscordEmoji.FromName(client, ":point_right:"))) };

                if (!String.IsNullOrEmpty(url))
                {
                    buttonlist.Insert(1, new DiscordLinkButtonComponent(url, "Go!"));
                }
                return buttonlist;
            }
            catch (Exception ex)
            {
                BotCommons.service.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return new List<DiscordComponent>() { };
        }

        private List<DiscordComponent> getLinkButton(string url)
        {
            try
            {
                return new List<DiscordComponent>(){ new DiscordLinkButtonComponent(url, "Go!") };
            }
            catch (Exception ex)
            {
                BotCommons.service.Log(foundInItem: MethodBase.GetCurrentMethod(),
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
                    .AddField("Country Code: ", string.IsNullOrEmpty(tourney.CountryCode) ? " - " : tourney.CountryCode)
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
                BotCommons.service.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
                embeds = new List<DiscordEmbed>();
            }

            return embeds;
        }

        private List<DiscordEmbed> GetDataEmbeds(List<TournamentData> tourneysSelected)
        {
            var embeds = new List<DiscordEmbed>();
            try
            {
                var numberOfTourneys = tourneysSelected.Count;
                var pageNumber = Math.Ceiling((decimal)numberOfTourneys / Common.TourneyDataPageRows);
                for (var batchPage = 0; batchPage < pageNumber; batchPage++)
                {
                    DiscordEmbedBuilder embed = new DiscordEmbedBuilder
                    {
                        Title = tourneysSelected.First().Game,
                        Color = DiscordColor.Green
                    };

                    int numberOfItemsToGet = Common.TourneyDataPageRows;
                    if (pageNumber - batchPage == 1)
                    {
                        numberOfItemsToGet = numberOfTourneys % Common.TourneyDataPageRows;
                    }

                    foreach (var tourney in tourneysSelected.GetRange(batchPage*Common.TourneyDataPageRows, numberOfItemsToGet))
                    {
                        embed.AddField((tourneysSelected.IndexOf(tourney) + 1).ToString() + ".: " + tourney.Name,
                                tourney.StartsAT == null ? "Date: - " : "Date (dd/mm/yyyy): " + tourney.StartsAT.Value.Date.ToString("dd/MM/yyyy") + 
                                "\nLocation: " + tourney.CountryCode)
                             .WithFooter($"Page {1 + batchPage}/{pageNumber}");
                    }
                    embed.Build();
                    embeds.Add(embed);
                }
            }
            catch (Exception ex)
            {
                BotCommons.service.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return embeds;
        }
        #endregion
    }

}
