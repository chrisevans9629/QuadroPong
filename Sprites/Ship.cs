using System.Collections.Generic;
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

        public Ship(IParticleEngine particleEngine)
        {
            _particleEngine = particleEngine;
            Size = Vector2.Zero;
        }
        public int Health { get; set; }
        public ShipState ShipState { get; set; } = ShipState.Dead;
        public void Start()
        {
            ShipState = ShipState.Coming;
            Health = 10;
        }
        public void Update(GameTime gameTime, List<Ball> balls)
        {
            var def = new Vector2(0.25f);

            if (ShipState == ShipState.Coming)
            {
                if (Size.X < def.X)
                {
                    Size += new Vector2(0.1f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    ShipState = ShipState.Ready;
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

                foreach (var ball in balls)
                {
                    if (Collision(ball))
                    {
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
            batch.Draw(Texture2D, Position - RelativeCenter, null, Color, 0, Vector2.Zero, Size, SpriteEffects.None, 0);
        }
    }
}