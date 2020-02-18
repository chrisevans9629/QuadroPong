using Microsoft.Xna.Framework;

namespace MyGame
{
    public class SpriteState
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; } = Vector2.One;
        public float Speed { get; set; }
        public Vector2 Acceleration { get; set; }

    }
}