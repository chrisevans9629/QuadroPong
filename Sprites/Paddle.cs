using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MyGame
{
    public class Paddle : Sprite
    {
        private readonly IPlayerController _player;

        public Paddle(IPlayerController player)
        {
            _player = player;
        }

        public Paddles Paddles { get; set; }
        public float Power { get; set; }
        public float ColorChange { get; set; }
        public int Score { get; set; }
        public SpriteFont SpriteFont { get; set; }

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
                ball.LastPosessor.Add(this);
                ball.Reflect(Center - ball.Center, Power);
                Power = 0;
            }

            var max = maxPort - EndPoint;

            if (!_player.UpdateAcceleration(this, ball).HasMoved)
            {
                return;
            }

            

            var newPos = Position +
                         Acceleration * (float)(Speed * time.ElapsedGameTime.TotalSeconds);

            Position = Vector2.Clamp(newPos, min, max);
        }

        public override void Draw(SpriteBatch batch)
        {
            batch.DrawString(SpriteFont,Score.ToString(), Position - new Vector2(10), Color.White);
            base.Draw(batch);
        }
    }
}