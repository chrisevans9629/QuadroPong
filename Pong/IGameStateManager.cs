using MyGame.Levels;

namespace PongGame.States
{
    public interface IGameStateManager
    {
        bool HasSavedGame();
        void SaveGame(LevelState state);
        LevelState LoadGame();
    }
}