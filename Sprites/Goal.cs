using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;

namespace MyGame
{
    public class Goal : Collider
    {
        public SpriteFont SpriteFont { get; set; }
        public RectangleF Rectangle { get; set; }
        public Paddles Paddles { get; set; }
        public Song Song { get; set; }

        public override RectangleF Bounds()
        {
            return Rectangle;
        }

        public bool SoundOn { get; set; } = true;
        public void Update(Ball ball, int width, int height)
        {
            if (Collision(ball))
            {
                Health--;

                if (ball.LastPosessor != null)
                {
                    var pos = ball.LastPosessor.LastOrDefault(p => p.Paddles != Paddles);
                    if (pos != null)
                        pos.Score++;
                }

                if (SoundOn)
                    MediaPlayer.Play(Song);
                ball.Reset(width, height);
                ball.Speed += 5;
            }
        }

        public int Health { get; set; } = 10;
        //public int Score { get; set; }
        public int Offset { get; set; }
        public void Draw(SpriteBatch spriteBatch, int width)
        {
            spriteBatch.DrawString(SpriteFont, $"{Paddles}: {Health}", new Vector2(width / 2f + Offset, 10), Color.White);
        }
    }
}