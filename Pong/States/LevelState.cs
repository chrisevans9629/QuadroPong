using System.Collections.Generic;

namespace MyGame.Levels
{
    public class LevelState
    {
        public List<PowerUpState> PowerUps { get; set; } = new List<PowerUpState>();
        public List<SpriteState> Boundaries { get; set; } = new List<SpriteState>();
        public List<SpriteState> Astroids { get; set; } = new List<SpriteState>();
        public GameMode GameMode { get; set; }
        public List<SpriteState> Balls { get; set; } = new List<SpriteState>();
        public List<PongPlayerState> PongPlayerStates { get; set; } = new List<PongPlayerState>();
    }
}