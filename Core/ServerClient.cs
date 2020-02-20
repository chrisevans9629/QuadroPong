using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using MyGame.Levels;

namespace MyGame
{
    public class ServerClient
    {
        public const string SendState = nameof(SendState);
        public const string ReceiveState = nameof(ReceiveState);
        private readonly HubConnection _connection;

        public ServerClient(HttpMessageHandler? handler = null)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:44397/GameHub", o =>
                {
                    if (handler != null)
                        o.HttpMessageHandlerFactory = _ => handler;
                })
                //.AddMessagePackProtocol()
                .AddNewtonsoftJsonProtocol()
                .WithAutomaticReconnect()
                .Build();
        }
        public async Task SendMove(LevelState state)
        {
            await _connection.InvokeAsync(SendState, state);
        }

        public List<LevelState> LevelStates { get; set; } = new List<LevelState>();

        public async Task Start()
        {
            _connection.On<LevelState>(
                ReceiveState, 
                s => LevelStates.Add(s));
            await _connection.StartAsync();

        }
    }
}