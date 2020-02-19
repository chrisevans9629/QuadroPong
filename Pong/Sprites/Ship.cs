using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace MyGame
{
    public enum ShipStatus
    {
        Dead,
        Coming,
        Ready,
    }

    public class ShipState
    {
        public SpriteState SpriteState { get; set; } = new SpriteState();
        public List<SpriteState> Balls { get; set; } = new List<SpriteState>();
        public int Health { get; set; }
        public ShipStatus ShipStatus { get; set; } = ShipStatus.Dead;
        public int Score { get; set; }
    }

    public class Ship : Sprite
    {
        private readonly IParticleEngine _particleEngine;
        private readonly IRandomizer _randomizer;
        private List<Ball> shipBullets = new List<Ball>();
        public Ship(
            IParticleEngine particleEngine, 
            IRandomizer randomizer, 
            ShipState? state = null)
        {
            _particleEngine = particleEngine ?? throw new ArgumentNullException(nameof(particleEngine));
            _randomizer = randomizer ?? throw new ArgumentNullException(nameof(randomizer));
           // Size = Vector2.Zero;
            //AngularVelocity = 1f;

            EngineTimer.EveryNumOfSeconds = 0.5f;
            EngineTimer.Restart();

            ExplosionTimer.EveryNumOfSeconds = 0.3f;
            ExplosionTimer.Restart();
            if (state != null)
            {
                foreach (var spriteState in state.Balls)
                {
                    shipBullets.Add(new Ball(_randomizer, new GameTimer()){SpriteState = spriteState});
                }
            }
            else
            {
                state = new ShipState();
                state.SpriteState.Size = Vector2.Zero;
                state.SpriteState.AngularVelocity = 1f;
                for (int i = 0; i < 4; i++)
                {
                    var ball = new Ball(_randomizer, new GameTimer());
                    shipBullets.Add(ball);
                    state.Balls.Add(ball.SpriteState);
                }
            }
            State = state;
            SpriteState = state.SpriteState;
        }

        public override void Dispose()
        {
            foreach (var bullet in this.shipBullets)
            {
                bullet?.Dispose();
            }
            this.Engines?.Dispose();
            this.Explosions?.Dispose();
            base.Dispose();
        }

        public void Reset()
        {
            ShipStatus = ShipStatus.Dead;
            Score = 0;
        }

        public void Load(
            Texture2D MEAT, 
            Point window, 
            SoundEffect explosions, 
            SoundEffect engineSound,
            SoundEffect pew,
            SoundEffect blip,
            Texture2D ballTexture2D,
            SpriteFont font)
        {
            Texture2D = MEAT;
            Position = new Vector2(window.X / 2f, window.Y / 2f);
            Explosions = explosions;
            Engines = engineSound;
            foreach (var shipBullet in shipBullets)
            {
                shipBullet.Load(pew,blip,ballTexture2D,font);
            }
        }
        public List<Ball>? Bullets => ShipStatus == ShipStatus.Ready ? shipBullets : new List<Ball>();
        public GameTimer ExplosionTimer { get; set; } = new GameTimer();
        public Vector2 RelativeCenter => new Vector2(Texture2D.Width / 2f * Size.X, Texture2D.Height / 2f * Size.Y);
        public GameTimer EngineTimer { get; set; } = new GameTimer();

        public ShipState State { get; }
        public int Health { get=>State.Health; set=>State.Health=value; }
        public ShipStatus ShipStatus { get=>State.ShipStatus; set=>State.ShipStatus=value; }
        public int Score { get=>State.Score; set=>State.Score=value; }

        public SoundEffect? Engines { get; set; }
        public SoundEffect? Explosions { get; set; }

        public void Start()
        {
            Size = Vector2.Zero;
            ShipStatus = ShipStatus.Coming;
            Health = 10;
        }
        public void Update(
            GameTime gameTime,
            List<Ball> balls,
            int width,
            int height,
            bool isSoundOn)
        {
            EngineTimer.Update(gameTime);
            Angle += AngularVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            switch (ShipStatus)
            {
                case ShipStatus.Coming:
                    UpdateComing(gameTime, width, height, isSoundOn);
                    break;
                case ShipStatus.Dead:
                    UpdateDead(gameTime, isSoundOn);
                    break;
                case ShipStatus.Ready:
                    UpdateReady(balls, isSoundOn);
                    break;
            }
        }

        private void UpdateComing(GameTime gameTime, int width, int height, bool isSoundOn)
        {
            if (EngineTimer.IsCompleted)
            {
                EngineTimer.Restart();
                if (isSoundOn)
                    Engines?.Play(1f * Size.X, 0, 0);
            }
            var def = new Vector2(0.25f);

            if (Size.X < def.X)
            {
                Size += new Vector2(0.1f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                ShipStatus = ShipStatus.Ready;
                var i = 0f;
                var q = MathF.PI / 4;
                foreach (var shipBullet in shipBullets)
                {
                    shipBullet.Reset(width, height, Angle + i);
                    i += q;
                }
            }
        }

        private void UpdateReady(List<Ball> balls, bool isSoundOn)
        {
            if (EngineTimer.IsCompleted)
            {
                EngineTimer.Restart();
                if (isSoundOn)
                    Engines?.Play(1f * Size.X, 0, 0);
            }


            if (Health <= 0)
            {
                _particleEngine.AddParticles(Position - RelativeCenter);
                ShipStatus = ShipStatus.Dead;
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

        private void UpdateDead(GameTime gameTime, bool isSoundOn)
        {
            if (Size.X > 0)
            {
                Size -= new Vector2(0.05f) * (float)gameTime.ElapsedGameTime.TotalSeconds;

                var colors = new List<Color>() { Color.Red, Color.Yellow, Color.Orange };

                ExplosionTimer.Update(gameTime);


                if (ExplosionTimer.IsCompleted)
                {
                    ExplosionTimer.Restart();
                    if (isSoundOn)
                        Explosions?.Play(0.5f, 0, 0);
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
        protected override bool IsColliding(Collider sprite)
        {
            return Circle().Intersects(sprite.Bounds());
        }



        public override void Draw(SpriteBatch batch)
        {
            var origin = new Vector2(Texture2D.Width / 2f, Texture2D.Height / 2f);

            batch.Draw(Texture2D, Position, null, Color, Angle, origin, Size, SpriteEffects.None, 0);
            if (ShipStatus != ShipStatus.Ready)
                return;
            foreach (var shipBullet in shipBullets)
            {
                shipBullet.Draw(batch);
            }
        }
    }
}