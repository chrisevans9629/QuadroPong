using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
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
        private List<Ball> shipBullets = new List<Ball>();
        public Ship(IParticleEngine particleEngine)
        {
            _particleEngine = particleEngine;
            Size = Vector2.Zero;
            AngularVelocity = 1f;
        }

        public List<Ball>? Bullets
        {
            get => ShipState == ShipState.Ready ? shipBullets : new List<Ball>();
            set => shipBullets = value ?? new List<Ball>();
        }

        public int Health { get; set; }
        public ShipState ShipState { get; set; } = ShipState.Dead;

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
            var def = new Vector2(0.25f);
            Angle += AngularVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (ShipState == ShipState.Coming)
            {
                if (Size.X < def.X)
                {
                    Size += new Vector2(0.1f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
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
            else if (ShipState == ShipState.Dead)
            {
                if (Size.X > 0)
                {
                    Size -= new Vector2(0.1f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    _particleEngine.AddParticles(Position - RelativeCenter);
                    _particleEngine.AddParticles(Bounds().TopLeft);
                    _particleEngine.AddParticles(Bounds().BottomRight);
                }
            }
            else if (ShipState == ShipState.Ready)
            {
                if (Health <= 0)
                {
                    _particleEngine.AddParticles(Position - RelativeCenter);
                    ShipState = ShipState.Dead;
                }
                
                foreach (var ball in balls.Union(shipBullets))
                {
                    if (Collision(ball))
                    {
                        if(ball.Timer.IsRunning)
                            continue;
                        Health--;
                        _particleEngine.AddParticles(ball.Position);
                        ball.Reflect(Center - ball.Center, 0);
                    }
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
            if(ShipState != ShipState.Ready)
                return;
            foreach (var shipBullet in shipBullets)
            {
                shipBullet.Draw(batch);
            }
        }
    }
}