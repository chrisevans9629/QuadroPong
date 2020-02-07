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
            Acceleration = new Vector2(x, y);
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

        public bool HasSound { get; set; } = true;
        public bool Debug { get; set; }
        public void Reflect(Direction position)
        {
            if (HasSound)
                BounceSong.Play();

            var randomVariation = (_random.NextFloat() - 0.5f) / 2f;


            var (x, y, normal) = position switch
            {
                Direction.Bottom => (0f, randomVariation, new Vector2(0, 1)),
                Direction.Top => (0f, -randomVariation, new Vector2(0, 1)),
                Direction.Left => (-randomVariation, 0f, new Vector2(1, 0)),
                Direction.Right => (randomVariation, 0f, new Vector2(1, 0)),
            };


            Acceleration = Vector2.Reflect(Acceleration + new Vector2(x, y), normal);
        }

        public float Normalize(float x, float min, float max, float from, float to)
        {
            return from + ((x - min) * (to - from)) / (max - min);
        }

        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);
            if (Debug)
                batch.DrawString(SpriteFont, Acceleration.ToString(), Position, Color.Red);
        }
    }
}