using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Xna.Framework;
using MyGame;
using MyGame.Levels;


namespace Server
{
    public class GameHub : Hub
    {
        public async Task SendPlayerPositions(List<VectorT> players)
        {
            await Clients.Others.SendAsync(ServerClient.ReceivePlayerPositions, players);
        }
        public async Task PlayerJoinedGame()
        {
            await Clients.Others.SendAsync(ServerClient.ReceivePlayerJoined);
        }
        public async Task SendState(LevelState state)
        {
            await Clients.Others.SendAsync(ServerClient.ReceiveState, state);
        }

        public async Task SendBallPosition(VectorT pos)
        {
            await Clients.Others.SendAsync(ServerClient.ReceiveBallPosition, pos);
        }
        //public async Task Moved(string player,float x, float y)
        //{
        //    await Clients.Others.SendAsync("Receive", player, x, y);
        //}
    }
}