using System.Collections.Generic;

namespace MyGame.Levels
{
    public class LevelState
    {
        public List<SpriteState> Balls { get; set; } = new List<SpriteState>();
        public List<PongPlayerState> PongPlayerStates { get; set; } = new List<PongPlayerState>();
    }
}