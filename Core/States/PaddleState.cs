using Microsoft.Xna.Framework;

namespace MyGame
{
    public class PaddleState
    {
        public SpriteState SpriteState { get; set; } = new SpriteState();
        public PlayerName PlayerName { get; set; }
        public Paddles Paddles { get; set; }
        public float Power { get; set; }
        public float ColorChange { get; set; }
        public int Score { get; set; }
        public bool IsStunned { get; set; }
        public bool HasHoldPaddle { get; set; }
        public VectorT BallLaunchOffset { get; set; }
    }
}