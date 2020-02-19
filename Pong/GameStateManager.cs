using System.IO;
using MyGame.Levels;
using Newtonsoft.Json;

namespace PongGame.States
{
    public class GameStateManager : IGameStateManager
    {
        private const string file = "game.json";
        public bool HasSavedGame()
        {
            return File.Exists(file);
        }

        public void SaveGame(LevelState state)
        {
            File.WriteAllText(file,JsonConvert.SerializeObject(state));
        }

        public LevelState LoadGame()
        {
            return JsonConvert.DeserializeObject<LevelState>(File.ReadAllText(file));
        }
    }
}