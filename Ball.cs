﻿using System;
using Microsoft.Xna.Framework;

namespace MyGame
{
    public class Ball : Sprite
    {
        public void Update(GameTime time, Vector2 viewportSize)
        {
            var min = Vector2.Zero;

            var max = viewportSize - EndPoint;

            if (Math.Abs(Position.X - min.X) < 1)
            {
                Acceleration = Vector2.Reflect(Acceleration, new Vector2(1, 0));
            }
            else if (Math.Abs(Position.X - max.X) < 1)
            {
                Acceleration = Vector2.Reflect(Acceleration, new Vector2(1, 0));
            }
            else if (Math.Abs(Position.Y - min.Y) < 1)
            {
                Acceleration = Vector2.Reflect(Acceleration, new Vector2(0, 1));
            }
            else if (Math.Abs(Position.Y - max.Y) < 1)
            {
                Acceleration = Vector2.Reflect(Acceleration, new Vector2(0, 1));
            }

            // TODO: Add your update logic here
            Acceleration.Normalize();

            var newPos = Position +
                         Acceleration * (float)(Speed * time.ElapsedGameTime.TotalSeconds);

            Position = Vector2.Clamp(newPos, min, max);
        }
    }
}