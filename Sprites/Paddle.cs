using System;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace MyGame
{
    public class Paddle : Sprite
    {
        private readonly IPlayer _player;

        public Paddle(IPlayer player)
        {
            _player = player;
        }

        public float Power { get; set; }
        public float ColorChange { get; set; }
        public void Update(GameTime time, Vector2 min, Vector2 maxPort, Ball ball)
        {
            if (Power > 0)
            {
                var r = (float)(Math.Sin(ColorChange) / 2f) + 0.5f;
                ColorChange += (float)time.ElapsedGameTime.TotalSeconds * 2f;
                var pulseColor = new Vector3(1, r, r);
                Color = new Color(pulseColor);
            }
            else
            {
                Color = Color.White;
            }

            if (ColorChange > 10)
            {
                ColorChange = 0;
            }
            

            if (Collision(ball))
            {
                ball.LastPosessor = this;
                ball.Reflect(Center - ball.Center, Power);
                Power = 0;
            }

            var max = maxPort - EndPoint;

            if (!_player.UpdateAcceleration(this, ball))
            {
                return;
            }

            

            var newPos = Position +
                         Acceleration * (float)(Speed * time.ElapsedGameTime.TotalSeconds);

            Position = Vector2.Clamp(newPos, min, max);
        }

        
    }
}