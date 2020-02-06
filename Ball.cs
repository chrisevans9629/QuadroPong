﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace MyGame
{
    public class Ball : Sprite
    {
        private readonly Random _random;
        public GameTimer Timer { get; }

        public Ball(Random random, GameTimer gameTimer)
        {
            _random = random;
            Timer = gameTimer;
            Speed = 300;
        }

        public SoundEffect BounceSong { get; set; }
        private float RandomFloat() => (float)(_random.NextDouble() + 0.5f);
        private bool RandomBool() => _random.NextDouble() > 0.5f;
        public void Reset(int width, int height)
        {
            Timer.Restart();
            var x = RandomFloat();
            var y = RandomFloat();

            if (RandomBool())
            {
                x = -x;
            }

            if (RandomBool())
            {
                y = -y;
            }

            //x = -0.25f;
            //y = -1;
            Acceleration = new Vector2(x, y);
            Position = new Vector2(width / 2f, height / 2f) - new Vector2(0, Texture2D.Height / 2f);
        }

        public void Update(GameTime time, Vector2 viewportSize)
        {
            if (Timer.IsRunning)
                return;

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

        public void Reflect(Vector2 normal)
        {
            BounceSong.Play();

            var randomVariation = new Vector2(0, (float)_random.NextDouble() - 0.5f);

            var accel = Vector2.Clamp(Acceleration + randomVariation, -Vector2.One, Vector2.One);
            //var accel = Acceleration;
            Acceleration = Vector2.Reflect(accel, normal);
            Acceleration.Normalize();
        }
    }
}