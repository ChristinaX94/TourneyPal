using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using System.Reflection;
using TourneyPal.Bot.Commands.AllGuildsCommands;
using TourneyPal.Bot.Commands.CommandExecution;

namespace TourneyPal.BotHandling
{
    public class BotConfiguration
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
                this.Client.Ready += BotConfigurationCommandExecution.OnClientReady;
                this.Client.GuildCreated += BotConfigurationCommandExecution.CreateRole;
                this.Client.MessageCreated += BotConfigurationCommandExecution.OnMessageCreated;

                this.Client.UseInteractivity(new InteractivityConfiguration()
                {
                    PollBehaviour = PollBehaviour.KeepEmojis,
                    Timeout = TimeSpan.FromMinutes(5)
                });

                SlashCommandsConfiguration slashCommandsConfiguration = new SlashCommandsConfiguration();

                this.Commands = this.Client.UseSlashCommands(slashCommandsConfiguration);
                this.Commands.RegisterCommands<AdminCommands>();
                this.Commands.RegisterCommands<GeneralCommands>();

                this.Commands.SlashCommandErrored += (cnext, ex) =>
                {
                    Console.WriteLine($"ERROR: {ex.Exception}");
                    BotCommons.DataService.Log(foundInItem: MethodBase.GetCurrentMethod(),
                           exceptionMessageItem: ex.Context.CommandName + " -- " + ex.Exception);
                    return Task.CompletedTask;
                };

                DiscordActivity status = new("/help", ActivityType.ListeningTo);

                await Client.ConnectAsync(status, UserStatus.Online);
                await BotConfigurationCommandExecution.CheckUpdates(this.Client);
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
