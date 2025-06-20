﻿using DSharpPlus.SlashCommands;
using static TourneyPal.Commons.Common;

namespace TourneyPal.Bot.Commands.CommandService
{
    public interface IBotCommandService
    {
        public Task Ping(InteractionContext ctx);
        public Task PingService(InteractionContext ctx);
        public Task Help(InteractionContext ctx);
        public Task Post(Game SelectedGame, InteractionContext ctx);
        public Task PostOld(Game SelectedGame, InteractionContext ctx);
        public Task PostAll(Game SelectedGame, InteractionContext ctx);
        public Task PostUpdates(Game SelectedGame, InteractionContext ctx);
        public Task PostTourneyIn(Game SelectedGame, InteractionContext ctx, string country);
        public Task SearchTournament(Game SelectedGame, InteractionContext ctx, string term);
        public Task RegisterChallongeTournament(InteractionContext ctx, string URL);
        public Task GetAvailableGames(InteractionContext ctx);
        public Task GetAvailableCommands(InteractionContext ctx);
        public Task RegisterServerGames(InteractionContext ctx);
        public Task RemoveServerGames(InteractionContext ctx);
    }

}
