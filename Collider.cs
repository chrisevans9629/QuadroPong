using System.Collections.Generic;
using Microsoft.Xna.Framework;

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

            return Direction.Bottom;
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
}