using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using System.Reflection;
using TourneyPal.Commons.DataObjects;

namespace TourneyPal.BotHandling
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }
        public SlashCommandsExtension Commands { get; private set; }

        public async Task runAsync()
        {
            var config = new DiscordConfiguration
            {
                Token = System.Configuration.ConfigurationManager.AppSettings["bottoken"],
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
            };

            this.Client = new DiscordClient(config);
            this.Client.Ready += isClientReady;

            this.Client.UseInteractivity(new InteractivityConfiguration()
            {
                PollBehaviour = PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromMinutes(5)
            });

            SlashCommandsConfiguration slashCommandsConfiguration = new SlashCommandsConfiguration();

            this.Commands = this.Client.UseSlashCommands(slashCommandsConfiguration);
            this.Commands.RegisterCommands<BotCommands>();


            await Client.ConnectAsync();
            await checkUpdates();
            await Task.Delay(-1);
        }

        private Task isClientReady(DiscordClient Client, ReadyEventArgs e)
        {
            Console.WriteLine("Tourney Pal is going online...");
            return Task.CompletedTask;
        }

        private async Task checkUpdates()
        {
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));
            while (await timer.WaitForNextTickAsync())
            {
                if (Common.getDate().Hour != Common.TimeOfDayRefreshData)
                {
                    continue;
                }

                var servers = this.Client.Guilds.ToList();
                foreach (var server in servers)
                {
                    var channels = server.Value.Channels.Values.Where(x => x.Type != ChannelType.Voice && x.Type != ChannelType.Category);
                    foreach (var channel in channels)
                    {
                        var role = server.Value.Roles.Select(x => x.Value).FirstOrDefault(x => x.Name.Equals("Test"));
                        DiscordEmbed embed = GetDataEmbed(BotCommons.service.getNewTournaments());
                        await setMessage(embed, channel, role).ConfigureAwait(false);
                    }
                }
            }
        }

        private DiscordEmbed GetDataEmbed(List<TournamentData> tourneysSelected)
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
                BotCommons.service.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
            return null;
        }


        private async Task setMessage(DiscordEmbed embed, DiscordChannel channel, DiscordRole role)
        {
            try
            {

                await channel.SendMessageAsync(new DiscordMessageBuilder()
                    .WithContent(role.Mention)
                    .AddMention(new RoleMention(role))
                    .AddEmbed(embed))
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                BotCommons.service.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }

    }
}
