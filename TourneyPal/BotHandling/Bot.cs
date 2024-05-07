using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.EventHandling;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;

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
                MinimumLogLevel = Microsoft.Extensions.Logging.LogLevel.Debug
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
            await Task.Delay(-1);
        }

        private Task isClientReady(DiscordClient Client, ReadyEventArgs e)
        {
            Console.WriteLine("Tourney Pal is going online...");
            return Task.CompletedTask;
        }
    }
}
