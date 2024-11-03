using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TourneyPal.BotHandling;
using TourneyPal.Commons.DataObjects;

namespace TourneyPal.Bot.BotHandling
{
    public static class BotActions
    {
        public static Task CreateRole(DiscordClient Client, GuildCreateEventArgs server)
        {
            try
            {
                var role = server.Guild.Roles.Select(x => x.Value).FirstOrDefault(x => x.Name.Equals(Common.TourneyPalRole));
                if (role != null)
                {
                    return Task.CompletedTask;
                }

                server.Guild.CreateRoleAsync(name: Common.TourneyPalRole, mentionable: true);

                var admin = server.Guild.Members.FirstOrDefault(x => x.Value.Hierarchy == int.MaxValue).Value;
                if (admin == null)
                {
                    return Task.CompletedTask;
                }

                admin.SendMessageAsync("Thank you for inviting TourneyPal! " + Environment.NewLine +
                    "If it wasn't created already, TourneyPal created a new role - " + Common.TourneyPalRole + " - just for annoucements!")
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return Task.CompletedTask;
        }

        public static Task OnClientReady(DiscordClient Client, ReadyEventArgs e)
        {
            Console.WriteLine("Tourney Pal is going online...");
            return Task.CompletedTask;
        }

        public static async Task CheckUpdates(DiscordClient client)
        {
            try
            {
                var timer = new PeriodicTimer(TimeSpan.FromHours(1));
                while (await timer.WaitForNextTickAsync())
                {
                    if (Common.getDate().Hour != Common.TimeOfDayRefreshData)
                    {
                        continue;
                    }

                    var newlyAddedTournaments = BotCommons.DataService.getNewlyAddedTournaments(Common.Game.SoulCalibur6);
                    if (newlyAddedTournaments == null || newlyAddedTournaments.Count==0)
                    {
                        continue;
                    }

                    var servers = client.Guilds.ToList();
                    foreach (var server in servers.Select(x=>x.Value))
                    {
                        var role = server.Roles.Select(x => x.Value).FirstOrDefault(x => x.Name.Equals(Common.TourneyPalRole));
                        var channel = server.Channels.Values.FirstOrDefault(x => x.Type != ChannelType.Voice && x.Type != ChannelType.Category);
                        if(channel == null)
                        { 
                            continue; 
                        }
                        DiscordEmbed embed = BotCommons.GetDataEmbed(newlyAddedTournaments);
                        await BotCommons.SetMessage(embed, channel, role).ConfigureAwait(false);
                    }
                }
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }

        }

        
    }
}
