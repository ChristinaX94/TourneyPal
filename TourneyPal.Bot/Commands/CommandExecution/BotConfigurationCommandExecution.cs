using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus;
using System.Reflection;
using TourneyPal.BotHandling;
using TourneyPal.Commons;
using DSharpPlus.SlashCommands;
using EnumsNET;
using DSharpPlus.Interactivity.Extensions;
using TourneyPal.Bot.Commands.GameCommands;

namespace TourneyPal.Bot.Commands.CommandExecution
{
    public static class BotConfigurationCommandExecution
    {
        public static Task CreateRole(DiscordClient Client, GuildCreateEventArgs server)
        {
            try
            {
                var role = server.Guild.Roles.Select(x => x.Value).FirstOrDefault(x => x.Name.Equals(BotCommons.TourneyPalRole));
                if (role != null)
                {
                    return Task.CompletedTask;
                }

                server.Guild.CreateRoleAsync(name: BotCommons.TourneyPalRole, mentionable: true);

                var admin = server.Guild.Members.FirstOrDefault(x => x.Value.Hierarchy == int.MaxValue).Value;
                if (admin == null)
                {
                    return Task.CompletedTask;
                }

                admin.SendMessageAsync("Thank you for inviting TourneyPal! " + Environment.NewLine +
                    "If it wasn't created already, TourneyPal created a new role - " + BotCommons.TourneyPalRole + " - just for annoucements!")
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
                    if (newlyAddedTournaments == null || newlyAddedTournaments.Count == 0)
                    {
                        continue;
                    }

                    var servers = client.Guilds.ToList();
                    foreach (var server in servers.Select(x => x.Value))
                    {
                        var role = server.Roles.Select(x => x.Value).FirstOrDefault(x => x.Name.Equals(BotCommons.TourneyPalRole));
                        var channel = server.Channels.Values.FirstOrDefault(x => x.Type != ChannelType.Voice && x.Type != ChannelType.Category);
                        if (channel == null)
                        {
                            continue;
                        }
                        DiscordEmbed embed = BotCommandExecution.GetDataEmbed(newlyAddedTournaments);
                        await BotCommandExecution.SetMessage(embed, channel, role).ConfigureAwait(false);
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

                var dictionaryOfAvailableGameCommands = await ShowCurrentCommands(e);
                if (dictionaryOfAvailableGameCommands == null ||
                    dictionaryOfAvailableGameCommands.Count == 0)
                {
                    await e.Channel.SendMessageAsync("All Commands Available!").ConfigureAwait(false);
                    return;
                }

                var selectedGameCommands = await SelectCommands(dictionaryOfAvailableGameCommands, e);

                await RegisterNewCommands(Client, e, dictionaryOfAvailableGameCommands, selectedGameCommands).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

        private static async Task RegisterNewCommands(DiscordClient Client, MessageCreateEventArgs e, Dictionary<int, Common.Game> dictionaryOfAvailableGameCommands, List<string> selectedGameCommands)
        {
            try
            {
                var commands = Client.GetSlashCommands();
                foreach (var part in selectedGameCommands)
                {

                    if (!int.TryParse(part, out var key))
                    {
                        return;
                    }
                    if (!dictionaryOfAvailableGameCommands.TryGetValue(key, out var value))
                    {
                        return;
                    }

                    switch (value)
                    {
                        case Common.Game.SoulCalibur2:
                            {
                                commands.RegisterCommands<SCIICommands>(e.Guild.Id);
                                await commands.RefreshCommands();
                                continue;
                            }
                        case Common.Game.SoulCalibur6:
                            {
                                commands.RegisterCommands<SCVICommands>(e.Guild.Id);
                                await commands.RefreshCommands();
                                continue;
                            }
                        case Common.Game.StreetFighter6:
                        case Common.Game.Tekken8:
                        default:
                            {
                                await commands.RefreshCommands();
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

        private static async Task<Dictionary<int, Common.Game>> ShowCurrentCommands(MessageCreateEventArgs e)
        {
            try
            {
                var server = e.Guild;
                var serverCommands = server.GetApplicationCommandsAsync().Result;
                await e.Channel.SendMessageAsync("Server contains the following sets of commands: " + Environment.NewLine + string.Join(Environment.NewLine, serverCommands.Select(x => "- " + x.Name.ToUpper())));

                Dictionary<int, string> gameCommands = new Dictionary<int, string>();
                var currentCommandsNames = serverCommands.Select(x => x.Name.ToUpper()).ToList();
                var currentDictionary = BotCommons.GameDescriptions.Where(x => !currentCommandsNames.Any(y => y.Equals(x.Key))).ToDictionary();
                if (currentDictionary == null)
                {
                    return new Dictionary<int, Common.Game>();
                }

                var dictionaryOfAvailableCommands = new Dictionary<int, Common.Game>();
                var enumerator = 1;
                foreach (var currentGame in currentDictionary.Values)
                {
                    dictionaryOfAvailableCommands.Add(enumerator, currentGame);
                    enumerator++;
                }
                return dictionaryOfAvailableCommands;
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return new Dictionary<int, Common.Game>();
        }

        private static async Task<List<string>> SelectCommands(Dictionary<int, Common.Game> dictionaryOfAvailableGameCommands, MessageCreateEventArgs e)
        {
            try
            {
                var listToShow = dictionaryOfAvailableGameCommands.Select(x => x.Key + ". " + x.Value.AsString(EnumFormat.Description)).ToList();

                await e.Channel.SendMessageAsync("Commands available for: " + Environment.NewLine + string.Join(Environment.NewLine, listToShow) + Environment.NewLine + "Respond with the number of games, seperated by comma to continue.");

                var selection = await e.Channel.GetNextMessageAsync();
                var message = selection.Result?.Content?.ToString();
                if (string.IsNullOrEmpty(message))
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

