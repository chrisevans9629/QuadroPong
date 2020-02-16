using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MyGame
{
    public class Sprite : Collider, IDisposable
    {
        public Vector2 AngleToVector(float angle)
        {
            return new Vector2((float)Math.Sin(angle), -(float)Math.Cos(angle));
        }

        public float VectorToAngle(Vector2 vector)
        {
            return (float)Math.Atan2(vector.X, -vector.Y);
        }
        public Vector2 Position { get; set; }

        public Vector2 Center => Position + new Vector2(Source?.Width ?? Texture2D.Width / 2f * Size.X,Source?.Height ?? Texture2D.Height / 2f * Size.Y);

        public Vector2 EndPoint => new Vector2( Texture2D.Width * Size.X, Texture2D.Height * Size.Y);
        public Texture2D? Texture2D { get; set; }
        public Vector2 Size { get; set; } = new Vector2(1);
        public float Speed { get; set; } = 0;
        public Vector2 Acceleration { get; set; } = Vector2.Zero;
        public Color Color { get; set; } = Color.White;
        public float Angle { get; set; }
        public float AngularVelocity { get; set; }
        public float Layer { get; set; }
        public Rectangle? Source { get; set; } = null;
        public override RectangleF Bounds() => new RectangleF(Position, new Size2(Source?.Width ?? Texture2D.Width,Source?.Height ?? Texture2D.Height));
        public virtual void Draw(SpriteBatch batch)
        {
            batch.Draw(Texture2D, Position, Source, Color, Angle, Vector2.Zero, Size, SpriteEffects.None,0);
        }


        public virtual void Dispose()
        {
            Texture2D?.Dispose();
        }
    }
}