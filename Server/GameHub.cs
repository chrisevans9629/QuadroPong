using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MyGame;
using MyGame.Levels;

namespace Server
{
    public class GameHub : Hub
    {
        public async Task SendState(LevelState state)
        {
            await Clients.Others.SendAsync(ServerClient.ReceiveState, state);
        }
        //public async Task Moved(string player,float x, float y)
        //{
        //    await Clients.Others.SendAsync("Receive", player, x, y);
        //}
    }
}