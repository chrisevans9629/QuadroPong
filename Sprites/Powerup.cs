using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace MyGame
{
    public enum PowerUpType
    {
        FastBall,
        BiggerPaddle
    }



    public class PowerUp : Sprite
    {
        private readonly IRandomizer _randomizer;

        public PowerUp(IRandomizer randomizer)
        {
            _randomizer = randomizer;
            Color = Color.Blue;
        }
        public void Reset(Rectangle area)
        {
            var x = _randomizer.Next(area.X, area.Width);
            var y = _randomizer.Next(area.Y, area.Height);

            var types = Enum.GetNames(typeof(PowerUpType));

            PowerUpType = Enum.Parse<PowerUpType>(types[_randomizer.Next(types.Length)]);

            Position = new Vector2(x, y);
        }

        public PowerUpType PowerUpType { get; set; }
        public float Power { get; set; } = 200f;
        public SoundEffect SoundEffect { get; set; }
        public void Update(Ball ball, Rectangle area, bool isSoundOn)
        {
            if (Collision(ball))
            {
                if (ball.LastPosessor.Any() != true)
                    return;
                if (isSoundOn)
                    SoundEffect.Play(0.3f, 0, 0);
                if (PowerUpType == PowerUpType.FastBall)
                {
                    ball.LastPosessor.Last().Power += Power;
                    Reset(area);
                }
                else if(PowerUpType == PowerUpType.BiggerPaddle)
                {
                    var last = ball.LastPosessor.Last();

                    if (last.Paddles == Paddles.Left || last.Paddles == Paddles.Right)
                    {
                        last.Size = new Vector2(1,2);
                    }
                    else
                    {
                        last.Size = new Vector2(2,1);
                    }
                    last.AddTimedPowerup(PowerUpType, 10, () => last.Size = new Vector2(1));
                    Reset(area);
                }
            }
        }
    }
}