using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MyGame;

namespace PongGame.Sprites
{
    public class Astroid : Sprite
    {
        private readonly IRandomizer _randomizer;

        public Astroid(IRandomizer randomizer)
        {
            _randomizer = randomizer;
        }
        public void Reset()
        {
            Position = new Vector2(0);
            Acceleration = new Vector2(_randomizer.Next(0,20), _randomizer.Next(0,20));
            //Acceleration = new Vector2(1,1);
            Speed = 100;
            Speed = (_randomizer.NextFloat() + 1) * 50;
            Size = new Vector2(_randomizer.NextFloat() + 0.3f);
            AngularVelocity = _randomizer.NextFloat() / 8f;
        }


        class T : Collider
        {
            public override RectangleF Bounds()
            {
                var state = Mouse.GetState();
                return new RectangleF(state.X, state.Y, 1, 1);
            }
        }
        public void Update(GameTime time, int width, int height, List<Ball> balls)
        {

            //var t = new T();
            //if (Collision(t))
            //{
            //    Console.WriteLine($"Mouse:{t.Bounds()},Astroid:{Bounds()}");
            //}
            foreach (var ball in balls)
            {
                if (Collision(ball))
                {
                    ball.Reflect((Position + RelativeCenter) - ball.Center, 0);
                }
            }

            var end = new Vector2(width + 100, height + 100);
            Acceleration = AngleToVector(this.VectorToAngle(Acceleration));

            var newPos = Position +
                         Acceleration * (float)(Speed * time.ElapsedGameTime.TotalSeconds);
            Angle += AngularVelocity;
            Position = Vector2.Clamp(newPos, Vector2.Zero, end);

            if (Position == end)
            {
                Reset();
            }
        }
        public Vector2 RelativeCenter => Source.Value.Width / 2f * Size;

        

        public Circle Circle()
        {
            var pos = Position;
            return new Circle(pos.X, pos.Y, Source.Value.Width / 2f * Size.X);
        }
        protected override bool IsColliding(Collider sprite)
        {
            return Circle().Intersects(sprite.Bounds());
        }

        public override void Draw(SpriteBatch batch)
        {
            batch.Draw(Texture2D, Position, Source, Color, Angle, new Vector2(50,50), Size, SpriteEffects.None, 0);
        }
    }
}