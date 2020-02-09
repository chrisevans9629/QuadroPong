using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
        public ShipState ShipState { get; set; }
        public void Start()
        {
            ShipState = ShipState.Coming;
            Health = 10;
        }
        public void Update(GameTime gameTime, List<Ball> balls)
        {
            var def = new Vector2(1);

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
                if(Size.X > 0)
                {
                    Size -= new Vector2(0.1f) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    _particleEngine.AddParticles(Center);
                }
            }
            else if (ShipState == ShipState.Ready)
            {
                if (Health <= 0)
                {
                    _particleEngine.AddParticles(Center);
                    ShipState = ShipState.Dead;
                }

                foreach (var ball in balls)
                {
                    if (Collision(ball))
                    {
                        Health--;
                        _particleEngine.AddParticles(ball.Position);
                    }
                }
            }
        }

        public override void Draw(SpriteBatch batch)
        {
            batch.Draw(Texture2D, Center, null,Color,0,Vector2.Zero, Size, SpriteEffects.None,0);
        }
    }
}