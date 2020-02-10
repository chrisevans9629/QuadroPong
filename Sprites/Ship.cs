using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MyGame
{
    public enum ShipState
    {
        Dead,
        Coming,
        Ready,
    }
    public class Ship : Sprite
    {
        private readonly IParticleEngine _particleEngine;
        private readonly IRandomizer _randomizer;
        private List<Ball> shipBullets = new List<Ball>();
        public Ship(IParticleEngine particleEngine, IRandomizer randomizer)
        {
            _particleEngine = particleEngine;
            _randomizer = randomizer;
            Size = Vector2.Zero;
            AngularVelocity = 1f;

            EngineTimer.EveryNumOfSeconds = 0.5f;
            EngineTimer.Restart();

            ExplosionTimer.EveryNumOfSeconds = 0.3f;
            ExplosionTimer.Restart();
        }

        public List<Ball>? Bullets
        {
            get => ShipState == ShipState.Ready ? shipBullets : new List<Ball>();
            set => shipBullets = value ?? new List<Ball>();
        }

        public int Health { get; set; }
        public ShipState ShipState { get; set; } = ShipState.Dead;
        public GameTimer ExplosionTimer { get; set; } = new GameTimer();

        public GameTimer EngineTimer { get; set; } = new GameTimer();

        public SoundEffect Engines { get; set; }
        public SoundEffect Explosions { get; set; }

        public void Start()
        {
            ShipState = ShipState.Coming;
            Health = 10;
        }
        public void Update(
            GameTime gameTime,
            List<Ball> balls,
            int width,
            int height)
        {
            EngineTimer.Update(gameTime);
            Angle += AngularVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            switch (ShipState)
            {
                case ShipState.Coming:
                    UpdateComing(gameTime, width, height);
                    break;
                case ShipState.Dead:
                    UpdateDead(gameTime);
                    break;
                case ShipState.Ready:
                    UpdateReady(balls);
                    break;
            }
        }

        private void UpdateComing(GameTime gameTime, int width, int height)
        {
            if (EngineTimer.IsCompleted)
            {
                EngineTimer.Restart();
                Engines.Play(1f * Size.X, 0, 0);
            }
            var def = new Vector2(0.25f);

            if (Size.X < def.X)
            {
                Size += new Vector2(0.1f) * (float) gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                ShipState = ShipState.Ready;
                var i = 0f;
                var q = MathF.PI / 4;
                foreach (var shipBullet in shipBullets)
                {
                    shipBullet.Reset(width, height, Angle + i);
                    i += q;
                }
            }
        }

        private void UpdateReady(List<Ball> balls)
        {
            if (EngineTimer.IsCompleted)
            {
                EngineTimer.Restart();
                Engines.Play(1f * Size.X, 0, 0);
            }


            if (Health <= 0)
            {
                _particleEngine.AddParticles(Position - RelativeCenter);
                ShipState = ShipState.Dead;
            }

            foreach (var ball in balls.Union(shipBullets))
            {
                if (Collision(ball))
                {
                    if (ball.Timer.IsRunning)
                        continue;
                    Health--;
                    _particleEngine.AddParticles(ball.Position);
                    ball.Reflect(Center - ball.Center, 0);
                }
            }
        }

        private void UpdateDead(GameTime gameTime)
        {
            if (Size.X > 0)
            {
                Size -= new Vector2(0.1f) * (float) gameTime.ElapsedGameTime.TotalSeconds;

                var colors = new List<Color>() {Color.Red, Color.Yellow, Color.Orange};

                ExplosionTimer.Update(gameTime);


                if (ExplosionTimer.IsCompleted)
                {
                    ExplosionTimer.Restart();
                    Explosions.Play(0.5f, 0, 0);
                    var b = Bounds();
                    var x = _randomizer.Next(b.X, b.X + b.Width);
                    var y = _randomizer.Next(b.Y, b.Y + b.Height);
                    _particleEngine.AddParticles(new Vector2(x, y), colors);
                }
            }
        }

        public override RectangleF Bounds()
        {
            return new RectangleF(Position - RelativeCenter, Texture2D.Bounds.Size.ToVector2() * Size);
        }

        public Circle Circle()
        {
            var pos = Position;
            return new Circle(pos.X, pos.Y, Texture2D.Width / 2f * Size.X);
        }
        protected override bool IsColliding(Sprite sprite)
        {
            return Circle().Intersects(sprite.Bounds());
        }

        public Vector2 RelativeCenter => new Vector2(Texture2D.Width / 2f * Size.X, Texture2D.Height / 2f * Size.Y);
        public override void Draw(SpriteBatch batch)
        {
            var origin = new Vector2(Texture2D.Width / 2f, Texture2D.Height / 2f);

            batch.Draw(Texture2D, Position, null, Color, Angle, origin, Size, SpriteEffects.None, 0);
            if (ShipState != ShipState.Ready)
                return;
            foreach (var shipBullet in shipBullets)
            {
                shipBullet.Draw(batch);
            }
        }
    }
}