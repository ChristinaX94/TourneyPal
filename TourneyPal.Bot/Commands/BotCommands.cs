using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharp​Plus.SlashCommands;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TourneyPal.Bot.BotHandling;
using TourneyPal.Service;

namespace TourneyPal
{
    public class BotCommands : ApplicationCommandModule
    {
        
        private ITourneyPalService _service = BotCommons.service;

        #region Commands
        [SlashCommand("ping", "pings bot")]
        public async Task Ping(InteractionContext ctx)
        {
            var server = ctx.Guild;
            var member = ctx.Member;

            Console.WriteLine("server: " + server);
            Console.WriteLine("member: " + member);

            await ctx.CreateResponseAsync("Pong").ConfigureAwait(false);
        }

        [SlashCommand("pingServer", "pings bot")]
        public async Task PingServer(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(_service.ping()).ConfigureAwait(false);
        }

        //[SlashCommand("post", "Posts all upcoming Tournaments up to one year")]
        //public async Task Post(InteractionContext ctx)
        //{
        //    try
        //    {
        //        List<DiscordEmbed> embeds = GetEmbeds(GeneralData.TournamentsData.Where(x=> x.StartsAT >= Common.getDate()).ToList());
        //        await setPages(ctx, embeds, ctx.InteractionId).ConfigureAwait(false);

        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
        //                   exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
        //    }
        //}

        //[SlashCommand("postOld", "Posts all past Tournaments")]
        //public async Task PostOld(InteractionContext ctx)
        //{
        //    try
        //    {
        //        List<DiscordEmbed> embeds = GetEmbeds(GeneralData.TournamentsData.Where(x => x.StartsAT < Common.getDate()).ToList());
        //        await setPages(ctx, embeds, ctx.InteractionId).ConfigureAwait(false);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
        //                   exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
        //    }
        //}

        //[SlashCommand("postAll", "Posts all registered Tournaments in DB")]
        //public async Task PostAll(InteractionContext ctx)
        //{
        //    try
        //    {
        //        List<DiscordEmbed> embeds = GetEmbeds(GeneralData.TournamentsData);

        //        var upcomingTournament = GeneralData.TournamentsData.FirstOrDefault(x => x.StartsAT >= Common.getDate());
        //        var upcomingTournamentPos = 0;
        //        if (upcomingTournament != null)
        //        {
        //            upcomingTournamentPos = GeneralData.TournamentsData.IndexOf(upcomingTournament);
        //        }

        //        await setPages(ctx, embeds, ctx.InteractionId, upcomingTournamentPos).ConfigureAwait(false);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
        //                   exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
        //    }
        //}

        //[SlashCommand("tourneyIn", "Posts all registered Tournaments in Specific Country")]
        //public async Task PostTourneyIn(InteractionContext ctx, [Option("country", "2 character Country Code")] string country)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(country) || country.ToArray().Length!=2)
        //        {
        //            await ctx.CreateResponseAsync("Country must contain 2 characters!").ConfigureAwait(false);
        //        }

        //        List<DiscordEmbed> embeds = GetEmbeds(GeneralData.TournamentsData
        //                                            .Where(x => x.CountryCode.Equals(country) && x.StartsAT >= Common.getDate()).ToList());

        //        await setPages(ctx, embeds, ctx.InteractionId).ConfigureAwait(false);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
        //                   exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
        //    }
        //}

        //[SlashCommand("search", "Searches a tournament based on URL. If it is located in Challonge, it gets registered.")]
        //public async Task SearchTourneyIn(InteractionContext ctx, [Option("URL", "Url of Tournament")] string URL)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(URL))
        //        {
        //            await ctx.CreateResponseAsync("Invalid URL!").ConfigureAwait(false);
        //        }
        //        var embed = GeneralData.TournamentsData.FirstOrDefault(x => x.URL.Equals(URL));
        //        if (embed==null)
        //        {
        //            //ApiHandler.examineSingleChallongeRequest(URL);
        //        }
        //        List <DiscordEmbed> embeds = GetEmbeds( new List<TournamentData>() { embed } );
        //        await setPages(ctx, embeds, ctx.InteractionId).ConfigureAwait(false);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
        //                   exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
        //    }
        //}

        #endregion

        //#region ActionHandlers
        //private async Task setPages(InteractionContext ctx, List<DiscordEmbed> embeds, ulong interactionID = 0, int selectedPos = 0)
        //{
        //    try
        //    {
        //        if (embeds.Count == 0)
        //        {
        //            await ctx.CreateResponseAsync("No Data").ConfigureAwait(false);
        //        }

        //        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
        //            .WithContent(ctx.Member.Mention)
        //            .AddMention(mention: new UserMention(ctx.User))
        //            .AddEmbed(embeds[selectedPos])
        //            .AddComponents(getButtons(ctx.Client, embeds[selectedPos].Fields[0].Value)))
        //            .ConfigureAwait(false);

        //        ctx.Client.ComponentInteractionCreated += async (s, e) =>
        //        {
        //            if (interactionID != e.Message.Interaction.Id)
        //            {
        //                return;
        //            }
        //            await HandlePageChange(ctx, embeds, e).ConfigureAwait(false);
        //        };

        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
        //                   exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
        //    }
        //}

        //private async Task HandlePageChange(InteractionContext ctx,
        //                                    List<DiscordEmbed> embeds,
        //                                    ComponentInteractionCreateEventArgs e)
        //{
        //    await ValidatePageChangeAction(e).ConfigureAwait(false);
        //    int pos = GetNextPagePosition(embeds, e);
        //    if (pos > -1)
        //    {
        //        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage, new DiscordInteractionResponseBuilder()
        //        .AddEmbed(embeds[pos])
        //        .AddComponents(getButtons(ctx.Client, embeds[pos].Fields[0].Value))).ConfigureAwait(false);
        //    }
        //    await Task.CompletedTask;
        //}
        //#endregion

        //#region Helpers

        //private static int GetNextPagePosition(List<DiscordEmbed> embeds, ComponentInteractionCreateEventArgs e)
        //{
        //    var selectedEmbed = embeds.FirstOrDefault(x => x.Fields[x.Fields.Count - 1].Value.Equals(e.Message.Embeds.FirstOrDefault().Fields[x.Fields.Count - 1].Value));

        //    if (selectedEmbed == null ||
        //        embeds.IndexOf(selectedEmbed) == -1)
        //    {
        //        return -1;
        //    }

        //    var pos = embeds.IndexOf(selectedEmbed);

        //    if (e.Id.Equals("Righty") && pos < embeds.Count - 1)
        //    {
        //        pos++;
        //    }
        //    if (e.Id.Equals("Lefty") && pos > 0)
        //    {
        //        pos--;
        //    }

        //    return pos;
        //}

        //private static async Task ValidatePageChangeAction(ComponentInteractionCreateEventArgs e)
        //{
        //    if (e.Message?.Embeds == null)
        //    {
        //        await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage,
        //            new DiscordInteractionResponseBuilder().WithContent("Session Expired")).ConfigureAwait(false);
        //    }

        //    if (DateTimeOffset.Now.Subtract(e.Message.CreationTimestamp).TotalMinutes > Common.PageButtonTimeoutMinutes)
        //    {
        //        if (e.Message.Embeds.FirstOrDefault() == null)
        //        {
        //            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage,
        //            new DiscordInteractionResponseBuilder().WithContent("Session Expired")).ConfigureAwait(false);
        //        }
        //        else
        //        {
        //            await e.Interaction.CreateResponseAsync(InteractionResponseType.UpdateMessage,
        //            new DiscordInteractionResponseBuilder().AddEmbed(e.Message.Embeds.FirstOrDefault())).ConfigureAwait(false);
        //        }
        //    }
        //}

        //private List<DiscordComponent> getButtons(DiscordClient client, string url)
        //{
        //    try
        //    {
        //        return new List<DiscordComponent>(){
        //                new DiscordButtonComponent(ButtonStyle.Secondary, "Lefty", string.Empty, emoji: new(DiscordEmoji.FromName(client, ":point_left:"))),
        //                new DiscordLinkButtonComponent(url, "Go!"),
        //                new DiscordButtonComponent(ButtonStyle.Secondary, "Righty", string.Empty, emoji: new(DiscordEmoji.FromName(client, ":point_right:"))) };
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
        //                   exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
        //    }
        //    return new List<DiscordComponent>() { };
        //}

        //private List<DiscordEmbed> GetEmbeds(List<TournamentData> tourneysSelected)
        //{
        //    List<DiscordEmbed> embeds = new List<DiscordEmbed>();
        //    try
        //    {
        //        foreach (var tourney in tourneysSelected)
        //        {
        //            DiscordEmbed embed = new DiscordEmbedBuilder
        //            {
        //                Title = tourney.Game,
        //                Description = tourney.Name,
        //                Color = DiscordColor.Purple,

        //            }
        //            .AddField("Site: ", tourney.HostSite + tourney.URL)
        //            .AddField("Online: ", tourney.Online == null ? " - " : tourney.Online == true ? "Yes" : "No")
        //            .AddField("Country Code: ", string.IsNullOrEmpty(tourney.CountryCode)? " - ":tourney.CountryCode)
        //            .AddField("Location: ", tourney.VenueAddress + ", " + tourney.City + ", " + tourney.AddrState + ", " + tourney.CountryCode)
        //            .AddField("Date (dd/mm/yyyy): ", tourney.StartsAT == null ? " - " : tourney.StartsAT.Value.Date.ToString("dd/MM/yyyy"))
        //            .AddField("Streams: ", tourney.Streams == null || tourney.Streams.Count == 0 ? " - " : string.Join("\n", tourney.Streams))
        //            .AddField("ID: ", tourney.ID.ToString())
        //            .WithFooter($"Tournament {1 + tourneysSelected.IndexOf(tourney)}/{tourneysSelected.Count}")
        //            .Build();

        //            embeds.Add(embed);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.log(foundInItem: MethodBase.GetCurrentMethod(),
        //                   exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
        //        embeds = new List<DiscordEmbed>();
        //    }

        //    return embeds;
        //}
        //#endregion
    }

}
