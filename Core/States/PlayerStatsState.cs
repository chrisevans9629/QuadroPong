using Microsoft.Xna.Framework;

namespace MyGame
{
    public class PlayerStatsState
    {
        public VectorT Position { get; set; }
        public int Score { get; set; }
        public int Health { get; set; }
        public PlayerName PlayerName { get; set; }
    }
}