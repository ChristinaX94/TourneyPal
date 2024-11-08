using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using System.Reflection;
using TourneyPal.Bot.Commands.CommandService;
using TourneyPal.Commons;
using TourneyPal.Commons.DataObjects;
using TourneyPal.DataService;

namespace TourneyPal.BotHandling
{
    public static class BotCommons
    {
        public static ITourneyPalDataService DataService { get; set; } = default!;
        public static IBotCommandService CommandService { get; set; } = default!;

        public static Dictionary<int, Common.Game> GameCommands = 
            new Dictionary<int, Common.Game>() 
            {
                { 1, Common.Game.SoulCalibur2  },
                { 2, Common.Game.SoulCalibur6  }
            };

        #region ActionHandlers
        public static async Task setPages(InteractionContext ctx, List<DiscordEmbed> embeds, ulong interactionID = 0, int selectedPos = 0)
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
                    await HandlePageChange(interactionID, ctx, embeds, e).ConfigureAwait(false);
                };

            }
            catch (Exception ex)
            {
                DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        public static async Task setDataPages(InteractionContext ctx, List<DiscordEmbed> embeds, ulong interactionID = 0, int selectedPos = 0)
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
                    await HandlePageChange(interactionID, ctx, embeds, e).ConfigureAwait(false);
                };
            }
            catch (Exception ex)
            {
                DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        public static async Task HandlePageChange(ulong interactionID,
                                            InteractionContext ctx,
                                            List<DiscordEmbed> embeds,
                                            ComponentInteractionCreateEventArgs e)
        {
            try
            {
                if (interactionID != e.Message.Interaction.Id)
                {
                    return;
                }

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
                DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }
        #endregion

        #region Helpers

        public static int GetNextPagePosition(List<DiscordEmbed> embeds, ComponentInteractionCreateEventArgs e)
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
                DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return -1;
        }

        public static async Task<bool> ValidatePageChangeAction(ComponentInteractionCreateEventArgs e)
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
                DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
                return false;
            }
            return true;
        }

        public static async Task<bool> ValidatePermissions(InteractionContext ctx, Permissions requiredPermissions)
        {
            try
            {
                Permissions userPermissions = ctx.Member.Permissions;

                if (userPermissions.HasPermission(requiredPermissions))
                {
                    return true;
                }
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent("You don't have the required permissions"));
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return false;
        }

        public static List<DiscordComponent> getButtons(DiscordClient client, string? url = "")
        {
            try
            {
                var buttonlist = new List<DiscordComponent>()
                {
                    new DiscordButtonComponent(ButtonStyle.Secondary, "Lefty", string.Empty, emoji: new(DiscordEmoji.FromName(client, ":point_left:"))),
                    new DiscordButtonComponent(ButtonStyle.Secondary, "Righty", string.Empty, emoji: new(DiscordEmoji.FromName(client, ":point_right:"))) 
                };

                if (!String.IsNullOrEmpty(url))
                {
                    buttonlist.Insert(1, new DiscordLinkButtonComponent(url, "Go!"));
                }

                return buttonlist;
            }
            catch (Exception ex)
            {
                DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return new List<DiscordComponent>() { };
        }

        public static List<DiscordComponent> getLinkButton(string url)
        {
            try
            {
                return new List<DiscordComponent>() { new DiscordLinkButtonComponent(url, "Go!") };
            }
            catch (Exception ex)
            {
                DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return new List<DiscordComponent>() { };
        }

        public static List<DiscordEmbed> GetEmbeds(List<TournamentData> tourneysSelected)
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
                DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
                embeds = new List<DiscordEmbed>();
            }

            return embeds;
        }

        public static List<DiscordEmbed> GetDataEmbeds(List<TournamentData> tourneysSelected)
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

                    foreach (var tourney in tourneysSelected.GetRange(batchPage * Common.TourneyDataPageRows, numberOfItemsToGet))
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
                DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return embeds;
        }
        #endregion


        public static DiscordEmbed GetDataEmbed(List<TournamentData> tourneysSelected)
        {
            try
            {
                var numberOfTourneys = tourneysSelected.Count;

                DiscordEmbedBuilder embedTobuild = new DiscordEmbedBuilder
                {
                    Title = tourneysSelected.First().Game,
                    Color = DiscordColor.Cyan
                };


                foreach (var tourney in tourneysSelected)
                {
                    embedTobuild.AddField((tourneysSelected.IndexOf(tourney) + 1).ToString() + ".: " + tourney.Name,
                        tourney.StartsAT == null ? "Date: - " : "Date (dd/mm/yyyy): " + tourney.StartsAT.Value.Date.ToString("dd/MM/yyyy") +
                        "\nLocation: " + tourney.CountryCode)
                        .WithFooter($"Total: " + numberOfTourneys + " Tournaments");
                }

                return embedTobuild.Build();

            }
            catch (Exception ex)
            {
                DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return null;
        }

        public static async Task SetMessage(DiscordEmbed embed, DiscordChannel channel, DiscordRole? role)
        {
            try
            {
                if (role == null)
                {
                    await channel.SendMessageAsync(new DiscordMessageBuilder().AddEmbed(embed)).ConfigureAwait(false);
                    return;
                }

                await channel.SendMessageAsync(new DiscordMessageBuilder()
                    .WithContent(role.Mention)
                    .AddMention(new RoleMention(role))
                    .AddEmbed(embed))
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        internal static async Task setMessageFollowUp(List<TournamentData> data, InteractionContext ctx)
        {
            try
            {
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder() { Content = "Respond with a number of choice to continue." });

                var result = await ctx.Channel.GetNextMessageAsync(ctx.User);

                if (result.TimedOut)
                {
                    return;
                }

                var ok = Int32.TryParse(result.Result.Content, out var choice);
                if (!ok || choice <= 0 || choice > data.Count)
                {
                    await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder() { Content = "Invalid input." });
                    return;
                }

                DiscordEmbed embedSelected = BotCommons.GetEmbeds(new List<TournamentData>() { data[choice - 1] }).FirstOrDefault();
                var url = embedSelected.Fields.FirstOrDefault(x => x.Name.Contains("Site"))?.Value;
                await ctx.FollowUpAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedSelected).AddComponents(BotCommons.getLinkButton(url)));
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
