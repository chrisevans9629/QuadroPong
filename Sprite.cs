using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyGame
{
    public class Sprite
    {
        public Vector2 Position { get; set; }

        public Vector2 Center => Position - new Vector2(0, Texture2D.Height / 2f);

        public Vector2 EndPoint => new Vector2(Texture2D.Width, Texture2D.Height);
        public Texture2D Texture2D { get; set; }
        public Vector2 Size { get; set; } = new Vector2(1);
        public float Speed { get; set; } = 0;
        public Vector2 Acceleration { get; set; } = Vector2.Zero;
        public Color Color { get; set; } = Color.White;
        public void Draw(SpriteBatch batch)
        {
            batch.Draw(Texture2D, Position, null, Color, 0, Vector2.Zero, Size, SpriteEffects.None, 0);
        }
    }
}