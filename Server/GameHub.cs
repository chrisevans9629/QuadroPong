using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Xna.Framework;
using MyGame;
using MyGame.Levels;


namespace Server
{
    public static class Games
    {
        public static List<HubPlayer> Players { get; set; } = new List<HubPlayer>();
    }


    public class GameHub : Hub
    {
        public async Task SendPlayerPositions(List<VectorT> players)
        {
            await Clients.Others.SendAsync(ServerClient.ReceivePlayerPositions, players);
        }
        //public async Task PlayerJoinedGame()
        //{
        //    await Clients.Others.SendAsync(ServerClient.ReceivePlayerJoined);
        //}

        public async Task SendPlayerState(PongPlayerState pongPlayerState)
        {
            await Clients.Others.SendAsync(ServerClient.ReceivePlayerState, pongPlayerState);
        }

        public async Task SendState(LevelState state)
        {
            await Clients.Others.SendAsync(ServerClient.ReceiveState, state);
        }

        public async Task SendBallPosition(VectorT pos)
        {
            await Clients.Others.SendAsync(ServerClient.ReceiveBallPosition, pos);
        }

        public override async Task OnConnectedAsync()
        {
            var player = new HubPlayer()
                {PlayerName = (PlayerName) Games.Players.Count + 1, Connection = this.Context.ConnectionId};
            Games.Players.Add(player);
            await Clients.Others.SendAsync(ServerClient.ReceivePlayerJoined, player.PlayerName);
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var t = Games.Players.First(p => p.Connection == Context.ConnectionId);
            Games.Players.Remove(t);
            await Clients.Others.SendAsync(ServerClient.ReceivePlayerDisconnected, t.PlayerName);
            await base.OnDisconnectedAsync(exception);
        }

        //public async Task Moved(string player,float x, float y)
        //{
        //    await Clients.Others.SendAsync("Receive", player, x, y);
        //}
    }
}