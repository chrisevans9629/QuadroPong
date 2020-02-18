using Microsoft.Xna.Framework;

namespace MyGame
{
    public class PlayerStatsState
    {
        public Vector2 Position { get; set; }
        public int Score { get; set; }
        public int Health { get; set; }
        public PlayerName PlayerName { get; set; }
    }
}