using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyGame;

namespace PongGame
{
    public class GameResult
    {
        public GameResult()
        {
            Goal = 2;
        }
        public SpriteFont? SpriteFont { get; set; }

        public int Goal { get; set; }

        public PongPlayer? Winner { get; set; }
        public void Update(List<PongPlayer> players)
        {
            var player = players.FirstOrDefault(p => p.Paddle.Score >= Goal);
            if (player != null)
            {
                Winner = player;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 center)
        {
            if (Winner != null)
                spriteBatch.DrawString(SpriteFont, $"Player {Winner.Position} won!", center, Color.White);
        }

        public void Reset()
        {
            Winner = null;
        }
    }
}
