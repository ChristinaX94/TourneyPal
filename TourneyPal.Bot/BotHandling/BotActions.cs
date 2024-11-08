using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus;
using System.Reflection;
using TourneyPal.BotHandling;
using TourneyPal.Commons;
using TourneyPal.Bot.Commands;
using DSharpPlus.SlashCommands;
using EnumsNET;
using DSharpPlus.Interactivity.Extensions;

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

        public static async Task OnMessageCreated(DiscordClient Client, MessageCreateEventArgs e)
        {
            try
            {
                if (e?.Message?.Interaction?.Name == null)
                {
                    return;
                }
                if (e.Message.Interaction.Name.ToLower().Contains("registerservergames"))
                {
                    await RegisterServerGames(Client, e);
                    return;
                }
                if (e.Message.Interaction.Name.ToLower().Contains("removeservergames"))
                {
                    await RemoveServerGames(Client, e);
                    return;
                }
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            
        }

        public static async Task RegisterServerGames(DiscordClient Client, MessageCreateEventArgs e)
        {
            try
            {
                var commands = Client.GetSlashCommands();
                var messageCommands = await showCurrentCommandsAndSelection(e);
                foreach (var part in messageCommands)
                {
                    await commands.RefreshCommands();
                    if (!Int32.TryParse(part, out var key))
                    {
                        return;
                    }
                    if (!BotCommons.GameCommands.TryGetValue(key, out var value))
                    {
                        return;
                    }

                    switch (value)
                    {
                        case Common.Game.SoulCalibur2:
                            {
                                commands.RegisterCommands<SCIICommands>(e.Guild.Id);
                                continue;
                            }
                        case Common.Game.SoulCalibur6:
                            {
                                commands.RegisterCommands<SCVICommands>(e.Guild.Id);
                                continue;
                            }
                        case Common.Game.StreetFighter6:
                        case Common.Game.Tekken8:
                        default:
                            {
                                continue;
                            }
                    }
                }

                
                await e.Channel.SendMessageAsync("Commands Added! Refresh with CTRL+R!").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        public static async Task RemoveServerGames(DiscordClient Client, MessageCreateEventArgs e)
        {
            try
            {
                await e.Guild.BulkOverwriteApplicationCommandsAsync(new List<DiscordApplicationCommand>());

                await e.Channel.SendMessageAsync("Commands Removed! Refresh with CTRL+R!").ConfigureAwait(false);

                var commands = Client.GetSlashCommands();
                
                await commands.RefreshCommands();
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        private static async Task<List<string>> showCurrentCommandsAndSelection(MessageCreateEventArgs e)
        {
            try
            {
                var server = e.Guild;
                var serverCommands = server.GetApplicationCommandsAsync().Result;
                var currentCommands = serverCommands.Select(x =>  "- " + x.Name.ToUpper());
                await e.Channel.SendMessageAsync("Server contains the following sets of commands: " + Environment.NewLine + String.Join(Environment.NewLine, currentCommands));
                var listToShow = BotCommons.GameCommands.Select(x => x.Key + ". " + x.Value.AsString(EnumFormat.Description)).ToList();

                await e.Channel.SendMessageAsync("Commands available for: " + Environment.NewLine + String.Join(Environment.NewLine, listToShow) + Environment.NewLine + "Respond with the number of games, seperated by comma to continue.");

                var selection = await e.Channel.GetNextMessageAsync();
                var message = selection.Result?.Content?.ToString();
                if (String.IsNullOrEmpty(message))
                {
                    return new List<string>();
                }
                return message.Trim().Split(',').ToList();
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return new List<string>();
        }
    }
}

