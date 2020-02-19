using Microsoft.Xna.Framework;

namespace MyGame
{
    public class SpriteState
    {
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; } = Vector2.One;
        public float Speed { get; set; }
        public Vector2 Acceleration { get; set; }
        public Color Color { get; set; } = Color.White;
        public float Angle { get; set; }
        public float AngularVelocity { get; set; }
        public float Layer { get; set; }
        public Rectangle? Source { get; set; } = null;
    }
}