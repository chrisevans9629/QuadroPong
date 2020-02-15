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
        FastPaddle,
        BiggerPaddle,
        SmallerPaddle,
        HoldPaddle,
        StunPaddle,
        BiggerBall,
        SmallerBall,
    }

    public class PowerUp : Sprite
    {
        private readonly IRandomizer _randomizer;
        private readonly IPowerupManager _powerupManager;

        public PowerUp(IRandomizer randomizer, IPowerupManager powerupManager)
        {
            _randomizer = randomizer;
            _powerupManager = powerupManager;
            Color = Color.Blue;
        }

        public override void Dispose()
        {
            this.SoundEffect?.Dispose();
            base.Dispose();
        }

        public void Reset(Rectangle area)
        {
            var x = _randomizer.Next(area.X, area.Width);
            var y = _randomizer.Next(area.Y, area.Height);

            var types = Enum.GetNames(typeof(PowerUpType));

            PowerUpType = Enum.Parse<PowerUpType>(types[_randomizer.Next(types.Length)]);
            Color = Getcolor();
            Position = new Vector2(x, y);
        }

        Color Getcolor()
        {
            return PowerUpType switch 
                {
                    PowerUpType.BiggerPaddle => Color.Green,
                    PowerUpType.FastBall => Color.Blue,
                    PowerUpType.FastPaddle => Color.LightBlue,
                    PowerUpType.SmallerPaddle => Color.Red,
                    PowerUpType.BiggerBall => Color.Purple,
                    PowerUpType.SmallerBall => Color.MediumPurple,
                    PowerUpType.StunPaddle => Color.Yellow,
                    PowerUpType.HoldPaddle => Color.Orange,
                _ => throw new NotImplementedException()
                };
        }


        public PowerUpType PowerUpType { get; set; }
        public float Power { get; set; } = 200f;
        public SoundEffect SoundEffect { get; set; }
        public void Update(Ball ball, Rectangle area, bool isSoundOn)
        {
            if (Collision(ball))
            {
                if (PowerUpType == PowerUpType.BiggerBall)
                {
                    ball.Size = new Vector2(2);
                    _powerupManager.AddTimedPowerup(ball, PowerUpType, 10, () => ball.Size = new Vector2(1));
                    Reset(area);
                }
                else if (PowerUpType == PowerUpType.SmallerBall)
                {
                    ball.Size = new Vector2(0.5f);
                    _powerupManager.AddTimedPowerup(ball, PowerUpType, 10, () => ball.Size = new Vector2(1));
                    Reset(area);
                }
                else
                {
                    if (!UpdatePaddlePowerups(ball, area)) 
                        return;
                }
              
                if (isSoundOn)
                    SoundEffect.Play(0.3f, 0, 0);
            }
        }

        private bool UpdatePaddlePowerups(Ball ball, Rectangle area)
        {
            if (ball.LastPosessor.Any() != true)
                return false;
            var last = ball.LastPosessor.Last();

            if (PowerUpType == PowerUpType.FastBall)
            {
                last.Power += Power;
            }
            else if (PowerUpType == PowerUpType.HoldPaddle)
            {
                last.HasHoldPaddle = true;
            }
            else if (PowerUpType == PowerUpType.StunPaddle)
            {
                last.IsStunned = true;
                _powerupManager.AddTimedPowerup(last, PowerUpType, 3, () => last.IsStunned = false);
            }
            else if (PowerUpType == PowerUpType.FastPaddle)
            {
                last.Speed += 100;
                _powerupManager.AddTimedPowerup(last, PowerUpType, 10, () => last.Speed -= 100);
            }
            else if (PowerUpType == PowerUpType.BiggerPaddle)
            {
                if (last.Paddles == Paddles.Left || last.Paddles == Paddles.Right)
                {
                    last.Size = new Vector2(1, 2);
                }
                else
                {
                    last.Size = new Vector2(2, 1);
                }

                _powerupManager.AddTimedPowerup(last, PowerUpType, 10, () => last.Size = new Vector2(1));
            }
            else if (PowerUpType == PowerUpType.SmallerPaddle)
            {
                if (last.Paddles == Paddles.Left || last.Paddles == Paddles.Right)
                {
                    last.Size = new Vector2(1, 0.5f);
                }
                else
                {
                    last.Size = new Vector2(0.5f, 1);
                }
                _powerupManager.AddTimedPowerup(last, PowerUpType, 10, () => last.Size = new Vector2(1));
            }
            Reset(area);
            return true;
        }
    }
}