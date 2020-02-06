using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace MyGame
{
    public class Goal : Collider
    {
        public SpriteFont SpriteFont { get; set; }
        public Rectangle Rectangle { get; set; }

        public Song Song { get; set; }

        public override Rectangle Bounds()
        {
            return Rectangle;
        }

        public void Update(Ball ball, int width, int height)
        {
            if (Collision(ball))
            {
                Score++;
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