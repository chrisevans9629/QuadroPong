using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace MyGame
{
    public class PongPlayer
    {
        public Paddles Position { get; }
        public bool Side => Position == Paddles.Left || Position == Paddles.Right;
        public PongPlayer(IPlayerController player, Paddles position)
        {
            Position = position;
            Paddle = new Paddle(player);
            Paddle.Speed = 500;
            Goal = new Goal();
        }

        private SoundEffect death;

        public void Load(SpriteFont font, Texture2D paddle, int goalOffset, Song goalSong, SoundEffect deathSound)
        {
            death = deathSound;
            Paddle.Texture2D = paddle;
            Paddle.SpriteFont = font;
            Goal.SpriteFont = font;
            Goal.Offset = goalOffset;
            Goal.Song = goalSong;
        }

        public void Reset(int width, int height)
        {
            Goal.Died = false;  
            Paddle.Power = 0;
            Paddle.Score = 0;
            Goal.Health = Goal.DefaultHealth;
            SetPosition(width, height);
        }

        public void SetPosition(int Width, int Height)
        {
            var offset = 30;
            var paddleWidth = Paddle.Texture2D.Width;
            var halfPaddleWidth = new Vector2(paddleWidth / 2f, 0);
            var halfWinWidth = Width / 2f;

            var goalOffset = 5;
            var goalWidth = 1;
            Paddle.Paddles = Position;
            Goal.Paddles = Position;
            if (Position == Paddles.Bottom)
            {
                Paddle.Position = new Vector2(halfWinWidth, Height - offset) - halfPaddleWidth;
                Goal.Rectangle = new Rectangle(0, Height - goalOffset, Width, goalWidth);
            }
            else if (Position == Paddles.Top)
            {
                Paddle.Position = new Vector2(halfWinWidth, offset) - halfPaddleWidth;
                Goal.Rectangle = new Rectangle(0, goalOffset, Width, goalWidth);
            }

            var halfPaddleHeight = new Vector2(0, Paddle.Texture2D.Height / 2f);
            var halfWinHeight = Height / 2f;

            if (Position == Paddles.Left)
            {
                Paddle.Position = new Vector2(offset, halfWinHeight) - halfPaddleHeight;
                Goal.Rectangle = new Rectangle(goalOffset, 0, goalWidth, Height);
            }
            else if (Position == Paddles.Right)
            {
                Paddle.Position = new Vector2(Width - offset, halfWinHeight) - halfPaddleHeight;
                Goal.Rectangle = new Rectangle(Width - goalOffset, 0, goalWidth, Height);
            }
        }

        public Paddle Paddle { get; set; }
        public Goal Goal { get; set; }

        public void Update(GameTime gameTime, Vector2 viewPort, List<Ball> balls, int width, int height, int boundarySize)
        {
            if (Goal.Died)
                return;

            Goal.Update(balls, width, height);

            if (Goal.Health <= 0 && !Goal.Died)
            {
                death.Play();
                Goal.Died = true;
            }

            

            var ball = balls.OrderBy(p => Vector2.Distance(p.Position, Goal.Rectangle.Center)).First();

            var width4 = width / 4f;
            var height4 = height / 4f;


            if (this.Position == Paddles.Left)
            {
                Paddle.Update(gameTime, new Vector2(0, boundarySize), viewPort - new Vector2(width4, boundarySize), ball);
            }
            else if (Position == Paddles.Right)
            {
                Paddle.Update(gameTime, new Vector2(width4 * 3, boundarySize), viewPort - new Vector2(0, boundarySize), ball);
            }
            else if (Position == Paddles.Top)
            {
                Paddle.Update(gameTime, new Vector2(boundarySize, 0), viewPort - new Vector2(boundarySize, height4), ball);
            }
            else
            {
                Paddle.Update(gameTime, new Vector2(boundarySize, height4 * 3), viewPort - new Vector2(boundarySize, 0), ball);
            }
        }

        public void Draw(SpriteBatch spriteBatch, int width)
        {
            Paddle.Draw(spriteBatch);
            Goal.Draw(spriteBatch, width);
        }
    }
}