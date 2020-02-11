using System.Collections.Generic;
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
        public void Update(List<Ball> balls, int width, int height)
        {
            foreach (var ball in balls)
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
          
        }

        public const int DefaultHealth = 10;
        public int Health { get; set; } = DefaultHealth;
        //public int Score { get; set; }
        public int Offset { get; set; }
        public bool Died { get; set; }

        public void Draw(SpriteBatch spriteBatch, int width)
        {
            spriteBatch.DrawString(SpriteFont, $"{Paddles}: {Health}", new Vector2(width / 2f + Offset, 10), Color.White);
        }
    }
}