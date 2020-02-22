using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace MyGame
{
    public class Ball : Sprite
    {
        public BallState State
        {
            get => _state;
            set
            {
                _state = value;
                Timer.State = value.GameTimerState;
            }
        }

        public override SpriteState SpriteState { get=>State.SpriteState; set=>State.SpriteState=value; }

        private readonly IRandomizer _random;
        private BallState _state = new BallState();
        public GameTimer Timer { get; }
        public SpriteFont? SpriteFont { get; set; }
        public Ball(IRandomizer random, GameTimer gameTimer)
        {
            _random = random;
            Timer = gameTimer;
            Speed = DefaultSpeed;
            Timer.State = State.GameTimerState;
        }

        public override void Dispose()
        {
            PewSound?.Dispose();
            BounceSong?.Dispose();
            foreach (var paddle in this.LastPosessor)
            {
                paddle?.Dispose();
            }
            base.Dispose();
        }

        public const float DefaultSpeed = 500;

        public SoundEffect? PewSound { get; set; }
        public bool IsBallColliding { get=>State.IsBallColliding; set=>State.IsBallColliding = value; }
        public SoundEffect? BounceSong { get; private set; }
        private float RandomFloat() => (float)(_random.NextFloat() + 0.5f);
        private bool RandomBool() => _random.NextFloat() > 0.5f;
        public bool HasSound { get; set; } = true;
        public bool Debug { get; set; }
        public List<Paddle> LastPosessor { get; set; } = new List<Paddle>(100);
        public void Load(
            SoundEffect pew,
            SoundEffect blip,
            Texture2D ballTexture,
            SpriteFont font)
        {
            PewSound = pew;
            BounceSong = blip;
            Texture2D = ballTexture;
            Timer.Font = font;
            SpriteFont = font;
        }
        public void Reset(int width, int height, float? angle = null)
        {
            Speed = DefaultSpeed;
            Timer.Restart();
            LastPosessor.Clear();

            if (angle is float a)
            {
                Acceleration = AngleToVector(a);
            }
            else
            {
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

                //x = -1;
                //y = 0;
                Acceleration = new Vector2(x, y);
            }
            Position = new Vector2(width / 2f, height / 2f) - new Vector2(0, Texture2D?.Height ?? 1f / 2f);

        }

        public void Update(GameTime time, Vector2 viewportSize)
        {
            if (Timer.IsRunning)
                return;
            if (Timer.IsCompleted)
            {
                Timer.IsCompleted = false;
                if (HasSound)
                    PewSound?.Play(0.5f, 0, 0);
            }
            var min = Vector2.Zero;

            var max = viewportSize - EndPoint;

            if (Math.Abs(Position.X - min.X) < 1)
            {
                Acceleration = Vector2.Reflect(Acceleration, new Vector2(1, 0));
                OnCollision();
            }
            else if (Math.Abs(Position.X - max.X) < 1)
            {
                Acceleration = Vector2.Reflect(Acceleration, new Vector2(1, 0));
                OnCollision();
            }
            else if (Math.Abs(Position.Y - min.Y) < 1)
            {
                Acceleration = Vector2.Reflect(Acceleration, new Vector2(0, 1));
                OnCollision();
            }
            else if (Math.Abs(Position.Y - max.Y) < 1)
            {
                Acceleration = Vector2.Reflect(Acceleration, new Vector2(0, 1));
                OnCollision();
            }

            // TODO: Add your update logic here
            Acceleration = AngleToVector(this.VectorToAngle(Acceleration));

            var newPos = Position +
                         Acceleration * (float)(Speed * time.ElapsedGameTime.TotalSeconds);

            Position = Vector2.Clamp(newPos, min, max);

            var subColor = DefaultSpeed / Speed;

            Color = new Color(new Vector3(1, Math.Max(subColor, 0), Math.Max(subColor, 0)));
            IsBallColliding = false;
        }

       

        public void Reflect(Direction position)
        {
            OnCollision();
            var randomVariation = (_random.NextFloat() - 0.5f) / 2f;

            var (x, y, normal) = position switch
            {
                Direction.Bottom => (0f, randomVariation, new Vector2(0, 1)),
                Direction.Top => (0f, -randomVariation, new Vector2(0, 1)),
                Direction.Left => (-randomVariation, 0f, new Vector2(1, 0)),
                Direction.Right => (randomVariation, 0f, new Vector2(1, 0)),
                _ => throw new NotImplementedException(),
            };

            Acceleration = Vector2.Reflect(Acceleration + new Vector2(x, y), normal);
        }

        private void OnCollision()
        {
            IsBallColliding = true;
            if (HasSound)
                BounceSong?.Play();
        }

        public void Reflect(Vector2 vector2, float power)
        {
            OnCollision();
            Acceleration = -vector2;
            Speed += power;
        }

        public override void Draw(SpriteBatch batch)
        {
            base.Draw(batch);
            if (Debug)
                batch.DrawString(SpriteFont, Acceleration.ToString(), Position, Color.Red);
        }
    }
}