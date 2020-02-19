using System.Collections.Generic;

namespace MyGame
{
    public class ShipState
    {
        public SpriteState SpriteState { get; set; } = new SpriteState();
        public List<SpriteState> Balls { get; set; } = new List<SpriteState>();
        public int Health { get; set; }
        public ShipStatus ShipStatus { get; set; } = ShipStatus.Dead;
        public int Score { get; set; }
    }
}