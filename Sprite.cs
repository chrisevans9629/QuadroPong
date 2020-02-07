using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyGame
{
    public abstract class Collider
    {
        public abstract Rectangle Bounds();

        private  List<Sprite> lastCollisions = new List<Sprite>();


        public bool BetweenX(Sprite sprite)
        {
            var bounds = Bounds();
           return sprite.Position.X >= bounds.X && sprite.Position.X + sprite.EndPoint.X <= bounds.X + bounds.Width;
        }

        public bool BetweenY(Sprite sprite)
        {
            var bounds = Bounds();
            return sprite.Position.Y >= bounds.Y && sprite.Position.Y + sprite.EndPoint.Y <= bounds.Y + bounds.Height;
        }


        public bool Collision(Sprite sprite)
        {
            // Texture2D.Bounds.Intersects(sprite.Texture2D.Bounds);
            var isColliding = Bounds().Intersects(sprite.Bounds());
            if (isColliding)
            {
                if (lastCollisions.Contains(sprite))
                {
                    return false;
                }

                lastCollisions.Add(sprite);
            }
            else
            {
                lastCollisions.Remove(sprite);
            }

            return isColliding;
        }
    }
    public class Sprite : Collider
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

        public Vector2 Center => Position - new Vector2(0, Texture2D.Height / 2f);

        public Vector2 EndPoint => new Vector2( Texture2D.Width, Texture2D.Height);
        public Texture2D Texture2D { get; set; }
        public Vector2 Size { get; set; } = new Vector2(1);
        public float Speed { get; set; } = 0;
        public Vector2 Acceleration { get; set; } = Vector2.Zero;
        public Color Color { get; set; } = Color.White;
        public override Rectangle Bounds() => new Rectangle(Position.ToPoint(), new Point(Texture2D.Width, Texture2D.Height));
        public virtual void Draw(SpriteBatch batch)
        {
            batch.Draw(Texture2D, Position, null, Color, 0, Vector2.Zero, Size, SpriteEffects.None, 0);
        }

        

    }
}