using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace MyGame
{
    public class Ball : Sprite
    {
        private readonly IRandomizer _random;
        public GameTimer Timer { get; }
        public SpriteFont SpriteFont { get; set; }
        public Ball(IRandomizer random, GameTimer gameTimer)
        {
            _random = random;
            Timer = gameTimer;
            Speed = 300;
        }

        public SoundEffect BounceSong { get; set; }
        private float RandomFloat() => (float)(_random.NextFloat() + 0.5f);
        private bool RandomBool() => _random.NextFloat() > 0.5f;
        public void Reset(int width, int height)
        {
            Timer.Restart();
            var x = RandomFloat();
            var y = RandomFloat();

            if (RandomBool())
            {
                x = -x;
            }

            if (RandomBool())
            {
                y = -y;
            }

            //x = -0.25f;
            //y = -1;
            Acceleration = new Vector2(-0.5f, -0.39999998f);
            Position = new Vector2(width / 2f, height / 2f) - new Vector2(0, Texture2D.Height / 2f);
        }

        public void Update(GameTime time, Vector2 viewportSize)
        {
            if (Timer.IsRunning)
                return;

            var min = Vector2.Zero;

            var max = viewportSize - EndPoint;

            if (Math.Abs(Position.X - min.X) < 1)
            {
                Acceleration = Vector2.Reflect(Acceleration, new Vector2(1, 0));
            }
            else if (Math.Abs(Position.X - max.X) < 1)
            {
                Acceleration = Vector2.Reflect(Acceleration, new Vector2(1, 0));
            }
            else if (Math.Abs(Position.Y - min.Y) < 1)
            {
                Acceleration = Vector2.Reflect(Acceleration, new Vector2(0, 1));
            }
            else if (Math.Abs(Position.Y - max.Y) < 1)
            {
                Acceleration = Vector2.Reflect(Acceleration, new Vector2(0, 1));
            }

            // TODO: Add your update logic here
            Acceleration.Normalize();

            var newPos = Position +
                         Acceleration * (float)(Speed * time.ElapsedGameTime.TotalSeconds);

            Position = Vector2.Clamp(newPos, min, max);
        }
        
        public void Reflect(Vector2 normal, float minAngle, float maxAngle)
        {
            BounceSong.Play();

            var randomVariation = _random.NextFloat() - 0.5f;

            var angle = Math.Min(Math.Min(VectorToAngle(Acceleration), minAngle), maxAngle);

            var accel = AngleToVector(angle + randomVariation);// * randomVariation;

            //if (RandomBool())
            //{
            //    accel.X = Math.Min(Acceleration.X * randomVariation.X,0.25f);
            //    accel.Y = Math.Min(Acceleration.Y * randomVariation.Y, 0.25f);
            //}
            //else
            //{
            //    accel = Acceleration / randomVariation;
            //}
            

            //if (Math.Abs(accel.X) > 1)
            //{
            //    accel.X /= randomVariation.X;
            //}

            //if (Math.Abs(accel.Y) > 1)
            //{
            //    accel.Y /= randomVariation.Y;
            //}

            //if (Math.Abs(accel.X - randomVariation.X) > 0)
            //{
            //    accel.X += randomVariation.X;
            //}
            //if (Math.Abs(accel.Y - randomVariation.Y) > 0)
            //{
            //    accel.Y += randomVariation.Y;
            //}
            //var accel = Acceleration;
            Acceleration = Vector2.Reflect(accel, normal);
        }

        public float Normalize(float x, float min, float max, float from, float to)
        {
            return from + ((x - min)*(to-from)) / (max - min);
        }

        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);
            batch.DrawString(SpriteFont,Acceleration.ToString(), Position, Color.Red);
        }
    }
}