﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace MyGame
{
    public class PongPlayer
    {
        public Direction Position { get; }
        public bool Side => Position == Direction.Left || Position == Direction.Right;
        public PongPlayer(IPlayer player, Direction position)
        {
            Position = position;
            Paddle = new Paddle(player);
            Paddle.Speed = 300;
            Goal = new Goal();
        }

        public void Load(SpriteFont font, Texture2D paddle, int goalOffset, Song goalSong)
        {
            Paddle.Texture2D = paddle;
            Goal.SpriteFont = font;
            Goal.Offset = goalOffset;
            Goal.Song = goalSong;
        }

        public void SetPosition(int Width, int Height)
        {
            var offset = 30;
            var paddleWidth = Paddle.Texture2D.Width;
            var halfPaddleWidth = new Vector2(paddleWidth / 2f, 0);
            var halfWinWidth = Width / 2f;

            var goalOffset = 5;
            var goalWidth = 1;

            if (Position == Direction.Bottom)
            {
                Paddle.Position = new Vector2(halfWinWidth, Height - offset) - halfPaddleWidth;
                Goal.Rectangle = new Rectangle(0, Height - goalOffset, Width, goalWidth);
            }
            else if (Position == Direction.Top)
            {
                Paddle.Position = new Vector2(halfWinWidth, offset) - halfPaddleWidth;
                Goal.Rectangle = new Rectangle(0, goalOffset, Width, goalWidth);
            }

            var halfPaddleHeight = new Vector2(0, Paddle.Texture2D.Height / 2f);
            var halfWinHeight = Height / 2f;

            if (Position == Direction.Left)
            {
                Paddle.Position = new Vector2(offset, halfWinHeight) - halfPaddleHeight;
                Goal.Rectangle = new Rectangle(goalOffset, 0, goalWidth, Height);
            }
            else if (Position == Direction.Right)
            {
                Paddle.Position = new Vector2(Width - offset, halfWinHeight) - halfPaddleHeight;
                Goal.Rectangle = new Rectangle(Width - goalOffset, 0, goalWidth, Height);
            }
        }

        public Paddle Paddle { get; set; }
        public Goal Goal { get; set; }

        public void Update(GameTime gameTime, Vector2 viewPort, List<Ball> balls, int width, int height, int boundarySize)
        {

            foreach (var ball1 in balls)
            {
                Goal.Update(ball1, width, height);
            }


            var ball = balls.OrderBy(p => Vector2.Distance(p.Position, Goal.Rectangle.Center.ToVector2())).First();


            if (Side)
            {
                Paddle.Update(gameTime, new Vector2(0, boundarySize), viewPort - new Vector2(0, boundarySize), ball);
            }
            else
            {
                Paddle.Update(gameTime, new Vector2(boundarySize, 0), viewPort - new Vector2(boundarySize, 0), ball);
            }
        }

        public void Draw(SpriteBatch spriteBatch, int width)
        {
            Paddle.Draw(spriteBatch);
            Goal.Draw(spriteBatch, width);
        }
    }
}