using System.Collections.Generic;

namespace MyGame.Levels
{
    public class LevelState
    {
        public ShipState? ShipState { get; set; }
        public List<PowerUpState> PowerUps { get; set; } = new List<PowerUpState>();
        public List<SpriteState> Boundaries { get; set; } = new List<SpriteState>();
        public List<SpriteState> Astroids { get; set; } = new List<SpriteState>();
        public GameMode GameMode { get; set; }
        public List<BallState> Balls { get; set; } = new List<BallState>();
        public List<PongPlayerState> PongPlayerStates { get; set; } = new List<PongPlayerState>();
    }
}