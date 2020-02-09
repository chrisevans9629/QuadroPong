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
                Score++;
                if (SoundOn)
                    MediaPlayer.Play(Song);
                ball.Reset(width, height);
                ball.Speed += 5;
            }
        }

        public int Score { get; set; }
        public int Offset { get; set; }
        public void Draw(SpriteBatch spriteBatch, int width)
        {
            spriteBatch.DrawString(SpriteFont, Score.ToString(), new Vector2(width / 2f + Offset, 10), Color.White);
        }
    }
}