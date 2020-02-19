using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace MyGame
{
    public class ServerClient
    {
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
        public async Task SendMove(string player, float x, float y)
        {
            await connection.InvokeAsync("Moved", player, x, y);
        }

        public string Output { get; set; } = "";
        public async Task Start()
        {
            connection.On<string, float, float>("Receive", (s, f, arg3) => { Output += $"{s} {f},{arg3}"; });
            await connection.StartAsync();

        }
    }
}