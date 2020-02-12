using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace MyGame
{
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

            Position = new Vector2(x, y);
        }

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
                ball.LastPosessor.Last().Power += Power;
                Reset(area);
            }
        }
    }
}