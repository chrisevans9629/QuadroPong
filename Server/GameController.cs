using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MyGame;

namespace Server
{
    [ApiController, Route("api/[Controller]")]
    public class GameController : Controller
    {
        public GameController()
        {
            
        }
        [HttpGet]
        public string Get()
        {
            return "Hello World!";
        }

        [HttpGet,Route("PlayerName")]
        public ActionResult<PlayerName?> GetPlayerName(string connection)
        {
            return Games.Players.FirstOrDefault(p => p.Connection == connection)?.PlayerName;
        }
    }
}