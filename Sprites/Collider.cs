using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using MonoGame.Extended;

namespace MyGame
{
    public abstract class Collider
    {
        public abstract RectangleF Bounds();

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

        public Direction OnRelativeSide(Sprite sprite)
        {
            var bounds = Bounds();
            if (BetweenX(sprite))
            {
                if (sprite.Position.Y >= bounds.Y + bounds.Height)
                {
                    return Direction.Bottom;
                }
                else
                {
                    return Direction.Top;
                }
            }
            else if (BetweenY(sprite))
            {
                if (sprite.Position.X + sprite.Texture2D.Width >= bounds.X + bounds.Width)
                {
                    return Direction.Right;
                }
                else
                {
                    return Direction.Left;
                }
            }

            return Direction.None;
        }

        protected virtual bool IsColliding(Sprite sprite)
        {
            var myBounds = Bounds();
            var spriteBounds = sprite.Bounds();

            var isColliding = myBounds.Intersects(spriteBounds);
            return isColliding;
        }
        public virtual bool Collision(Sprite sprite)
        {
            var isColliding = IsColliding(sprite);
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
}