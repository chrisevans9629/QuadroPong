using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MyGame
{
    public class Paddle : Sprite
    {
        private readonly IPlayerController _player;
        private readonly IParticleEngine _particleEngine;

        public Paddle(IPlayerController player, IParticleEngine particleEngine)
        {
            _player = player;
            _particleEngine = particleEngine;
        }

        public Paddles Paddles { get; set; }
        public float Power { get; set; }
        public float ColorChange { get; set; }
        public int Score { get; set; }
        public SpriteFont SpriteFont { get; set; }
        public bool IsStunned { get; set; }
        public bool HasHoldPaddle { get; set; }

        public override RectangleF Bounds() => new RectangleF(Position, new Size2(Texture2D.Width * Size.X, Texture2D.Height * Size.Y));

        private bool ballReady;


        public void Update(GameTime time, Vector2 min, Vector2 maxPort, Ball ball, float width, float height)
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

            if (ballReady)
            {
                var direct = _player.GetDirectional(Center - new Vector2(-width / 2f, height / 2f));
                if (_player.TriggerPressed().Value)
                {
                    ball.Reflect(new Vector2(-direct.Value.X, direct.Value.Y), Power);
                    Power = 0;
                    ball.Speed = 300f;
                    HasHoldPaddle = false;
                    ballReady = false;
                }
                return;
            }
            

            if (Collision(ball))
            {
                ball.LastPosessor.Add(this);

                if (HasHoldPaddle)
                {
                    ballReady = true;
                    ball.Speed = 0;
                    ball.Acceleration = Vector2.Zero;
                    return;
                }
                else
                {
                    ball.Reflect(Center - ball.Center, Power);
                    Power = 0;
                }
            }

            var max = maxPort - EndPoint;

            if (IsStunned)
            {
                _particleEngine.AddParticles(Center, new List<Color>() { Color.Yellow, Color.Orange });
                return;
            }
            var accel = _player.UpdateAcceleration(this, ball);

            if (!accel.HasMoved)
            {
                return;
            }



            var newPos = Position +
                         Acceleration * (float)(Speed * time.ElapsedGameTime.TotalSeconds);

            Position = Vector2.Clamp(newPos, min, max);
        }

        public override void Draw(SpriteBatch batch)
        {
            batch.DrawString(SpriteFont, Score.ToString(), Position - new Vector2(10), Color.White);
            base.Draw(batch);
        }
    }
}