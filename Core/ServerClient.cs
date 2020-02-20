using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using MyGame.Levels;

namespace MyGame
{
    public class ServerClient
    {
        public const string SendState = nameof(SendState);
        public const string ReceiveState = nameof(ReceiveState);
        private HubConnection connection;

        public ServerClient()
        {
            connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:44397/GameHub")
                .WithAutomaticReconnect()
                .Build();
        }
        public async Task SendMove(LevelState state)
        {
            await connection.InvokeAsync(SendState, state);
        }

        public List<LevelState> LevelStates { get; set; } = new List<LevelState>();

        public async Task Start()
        {
            connection.On<LevelState>(
                ReceiveState, 
                s => LevelStates.Add(s));
            await connection.StartAsync();

        }
    }
}