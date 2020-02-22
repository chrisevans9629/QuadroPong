using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using MyGame.Levels;
using Newtonsoft.Json;

namespace MyGame
{
    public class ServerClient
    {
        public const string SendState = nameof(SendState);
        public const string ReceiveState = nameof(ReceiveState);
        public const string ReceiveBallPosition = nameof(ReceiveBallPosition);
        public const string SendBallPosition = nameof(SendBallPosition);
        public const string ReceivePlayerPositions = nameof(ReceivePlayerPositions);
        public const string SendPlayerPositions = nameof(SendPlayerPositions);

        public const string ReceivePlayerJoined = nameof(ReceivePlayerJoined);
        public const string ReceivePlayerDisconnected = nameof(ReceivePlayerDisconnected);
        public const string PlayerJoinedGame = nameof(PlayerJoinedGame);
        private readonly HubConnection _connection;
        private readonly HttpClient _httpClient;
        const string Url = "https://localhost:44397/GameHub";
        public ServerClient(HttpMessageHandler? handler = null)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl(Url, o =>
                {
                    if (handler != null)
                        o.HttpMessageHandlerFactory = _ => handler;
                })
                //.AddMessagePackProtocol()
                .AddNewtonsoftJsonProtocol()
                .WithAutomaticReconnect()
                .Build();
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(Url, UriKind.RelativeOrAbsolute);
        }
        public async Task SendMove(LevelState state)
        {
            await _connection.InvokeAsync(SendState, state);
        }

        public async Task SendPlayers(List<VectorT> positions)
        {
            await _connection.InvokeAsync(SendPlayerPositions, positions);
        }
        public async Task SendBall(VectorT pos)
        {
            await _connection.InvokeAsync(SendBallPosition, pos);
        }

        public async Task<PlayerName> GetPlayerName()
        {
            var str =await _httpClient.GetStringAsync($"api/Game/PlayerName?connection={_connection.ConnectionId}");
            return JsonConvert.DeserializeObject<PlayerName>(str);
        }

        public LevelState? LevelStates { get; set; }
        public VectorT? BallPosition { get; set; }
        public List<VectorT>? PlayerPositions { get; set; }
        public bool PlayerJoined { get; set; }
        public PlayerName PlayerName { get; set; }
        public List<PlayerName> Players { get; set; } = new List<PlayerName>();
        public async Task Start()
        {
            
            _connection.On<LevelState>(
                ReceiveState,
                s => LevelStates = s);

            _connection.On<VectorT>(
                ReceiveBallPosition,
                s => BallPosition = s);

            _connection.On<List<VectorT>>(
                ReceivePlayerPositions,
                s => PlayerPositions = s);

            _connection.On<PlayerName>(ReceivePlayerJoined, p => Players.Add(p));
            _connection.On<PlayerName>(ReceivePlayerDisconnected, p => Players.Remove(p));

            await _connection.StartAsync();
            //await _connection.SendAsync(PlayerJoinedGame);
            PlayerName = await GetPlayerName();
        }
    }
}