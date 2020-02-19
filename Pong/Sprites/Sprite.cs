using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MyGame
{
    public class Sprite : Collider, IDisposable
    {
        private SpriteState _spriteState = new SpriteState();

        public Sprite()
        {
            SpriteState.Size = Vector2.One;
        }

        public SpriteState SpriteState
        {
            get => _spriteState;
            set => _spriteState = value ?? throw new InvalidOperationException();
        }

        public Vector2 AngleToVector(float angle)
        {
            return new Vector2((float)Math.Sin(angle), -(float)Math.Cos(angle));
        }

        public float VectorToAngle(Vector2 vector)
        {
            return (float)Math.Atan2(vector.X, -vector.Y);
        }
        public Vector2 Position { get => SpriteState.Position; set => SpriteState.Position = value; }

        public Vector2 Center => Position + new Vector2(Source?.Width ?? Texture2D.Width / 2f * Size.X,Source?.Height ?? Texture2D.Height / 2f * Size.Y);

        public Vector2 EndPoint => new Vector2( Texture2D.Width * Size.X, Texture2D.Height * Size.Y);
        public Texture2D? Texture2D { get; set; }
        public Vector2 Size { get => SpriteState.Size; set => SpriteState.Size = value; } 
        public float Speed { get => SpriteState.Speed; set => SpriteState.Speed = value; }
        public Vector2 Acceleration { get => SpriteState.Acceleration; set => SpriteState.Acceleration = value; }
        public Color Color { get=>SpriteState.Color; set=>SpriteState.Color =value; } 
        public float Angle { get=>SpriteState.Angle; set=>SpriteState.Angle=value; }
        public float AngularVelocity { get=>SpriteState.AngularVelocity; set=>SpriteState.AngularVelocity=value; }
        public float Layer { get=>SpriteState.Layer; set=>SpriteState.Layer=value; }
        public Rectangle? Source { get=>SpriteState.Source; set=>SpriteState.Source=value; } 
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