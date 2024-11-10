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
        public static readonly int PageButtonTimeoutMinutes = 5;

        public static readonly int TourneyDataPageRows = 5;

        public static readonly string TourneyPalRole = "TourneyPalAnnouncements";

        public static ITourneyPalDataService DataService { get; set; } = default!;

        public static IBotCommandService CommandService { get; set; } = default!;

        public static Dictionary<string, Common.Game> GameDescriptions =
            new Dictionary<string, Common.Game>()
            {
                { "SCII", Common.Game.SoulCalibur2 },
                { "SCVI", Common.Game.SoulCalibur6}
            };
    }
}
