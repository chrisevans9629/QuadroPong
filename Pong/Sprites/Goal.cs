using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;

namespace MyGame
{
    public class GoalState
    {
        public const int DefaultHealth = 10;

        public RectangleF Rectangle { get; set; }
        public Paddles Paddles { get; set; }
        public int Health { get; set; } = DefaultHealth;
        public int Offset { get; set; }
        public bool Died { get; set; }
        public bool SoundOn { get; set; } = true;

    }
    public class Goal : Collider, IDisposable
    {
        public GoalState State { get; set; } = new GoalState();
        public SpriteFont? SpriteFont { get; set; }
        public RectangleF Rectangle { get => State.Rectangle; set=>State.Rectangle = value; }
        public Paddles Paddles { get=>State.Paddles; set=>State.Paddles =value; }
        public Song? Song { get; set; }
        public int Health { get=>State.Health; set=>State.Health=value; }
        public int Offset { get=>State.Offset; set=>State.Offset=value; }
        public bool Died { get=>State.Died; set=>State.Died=value; }
        public bool SoundOn { get=>State.SoundOn; set=>State.SoundOn=value; }

        public override RectangleF Bounds()
        {
            return Rectangle;
        }

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

        public void Draw(SpriteBatch spriteBatch, int width)
        {
            spriteBatch.DrawString(SpriteFont, $"{Paddles}: {Health}", new Vector2(width / 2f + Offset, 10), Color.White);
        }

        public void Dispose()
        {
            Song?.Dispose();
        }
    }
}