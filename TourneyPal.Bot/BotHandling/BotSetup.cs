using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using System.Reflection;
using TourneyPal.Bot.BotHandling;
using TourneyPal.Bot.Commands;

namespace TourneyPal.BotHandling
{
    public class BotSetup
    {
        public DiscordClient Client { get; private set; }
        public SlashCommandsExtension Commands { get; private set; }

        public async Task runAsync()
        {
            try
            {
                var config = new DiscordConfiguration
                {
                    Token = System.Configuration.ConfigurationManager.AppSettings["bottoken"],
                    TokenType = TokenType.Bot,
                    AutoReconnect = true,
                    MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug,
                    Intents = DiscordIntents.All
                };

                this.Client = new DiscordClient(config);
                this.Client.Ready += BotActions.OnClientReady;
                this.Client.GuildCreated += BotActions.CreateRole;
                this.Client.MessageCreated += BotActions.OnMessageCreated;

                this.Client.UseInteractivity(new InteractivityConfiguration()
                {
                    PollBehaviour = PollBehaviour.KeepEmojis,
                    Timeout = TimeSpan.FromMinutes(5)
                });

                SlashCommandsConfiguration slashCommandsConfiguration = new SlashCommandsConfiguration();

                this.Commands = this.Client.UseSlashCommands(slashCommandsConfiguration);
                this.Commands.RegisterCommands<AdminCommands>();
                this.Commands.RegisterCommands<GeneralCommands>();

                DiscordActivity status = new("/help", ActivityType.ListeningTo);

                await Client.ConnectAsync(status, UserStatus.Online);
                await BotActions.CheckUpdates(this.Client);
                await Task.Delay(-1);
            }
            catch (Exception ex)
            {
                BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Message + " -- " + ex.StackTrace);
            }
        }
    }
}
