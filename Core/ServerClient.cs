using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
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
                .Build();

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };
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