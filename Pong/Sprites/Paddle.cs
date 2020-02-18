using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MyGame
{
    public class PaddleState
    {
        public SpriteState SpriteState { get; set; }
        public PlayerName PlayerName { get; set; }
        public Paddles Paddles { get; set; }
        public float Power { get; set; }
        public float ColorChange { get; set; }
        public int Score { get; set; }
        public bool IsStunned { get; set; }
        public bool HasHoldPaddle { get; set; }
        public Vector2 BallLaunchOffset { get; set; }
    }
    public class Paddle : Sprite
    {
        private readonly IPlayerController _player;
        private readonly IParticleEngine _particleEngine;
        private bool ballReady;
        private PaddleState _state;

        public PaddleState State
        {
            get => _state;
            set
            {
                _state = value;
                SpriteState = _state.SpriteState;
            }
        }

        public Paddle(IPlayerController player, IParticleEngine particleEngine)
        {
            _player = player;
            _particleEngine = particleEngine;
        }
        public PlayerName PlayerName { get => State.PlayerName; set => State.PlayerName = value; }

        public Paddles Paddles { get => State.Paddles; set => State.Paddles = value; }
        public float Power { get => State.Power; set => State.Power = value; }
        public float ColorChange { get => State.ColorChange; set => State.ColorChange = value; }
        public int Score { get => State.Score; set => State.Score = value; }
        public SpriteFont? SpriteFont { get; set; }
        public bool IsStunned { get => State.IsStunned; set => State.IsStunned = value; }
        public bool HasHoldPaddle { get => State.HasHoldPaddle; set => State.HasHoldPaddle = value; }
        public Vector2 BallLaunchOffset { get => State.BallLaunchOffset; set => State.BallLaunchOffset = value; }
        public override RectangleF Bounds() => new RectangleF(Position, new Size2(Texture2D.Width * Size.X, Texture2D.Height * Size.Y));

       

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
                    ball.Speed = Ball.DefaultSpeed + 100f;
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
                    ball.Reflect(Center + BallLaunchOffset - ball.Center, Power);
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
            var name = (int)PlayerName;

            batch.DrawString(SpriteFont, name.ToString(), Position - new Vector2(10), PlayerName.ToColor());
            base.Draw(batch);
        }
    }
}