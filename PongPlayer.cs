using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MyGame
{
    public class PongPlayer
    {
        public Position Position { get; }
        public bool Side => Position == Position.Left || Position == Position.Right;
        public PongPlayer(IPlayer player, Position position)
        {
            Position = position;
            Paddle = new Paddle(player);
            Paddle.Speed = 300;
            Goal = new Goal();
        }

        public void Load(SpriteFont font, Texture2D paddle, int goalOffset)
        {
            Paddle.Texture2D = paddle;
            Goal.SpriteFont = font;
            Goal.Offset = goalOffset;
        }

        public void SetPosition(int Width, int Height)
        {
            var offset = 30;
            var paddleWidth = Paddle.Texture2D.Width;
            var halfPaddleWidth = new Vector2(paddleWidth / 2f, 0);
            var halfWinWidth = Width / 2f;

            var goalOffset = 5;
            var goalWidth = 1;

            if (Position == Position.Bottom)
            {
                Paddle.Position = new Vector2(halfWinWidth, Height - offset) - halfPaddleWidth;
                Goal.Rectangle = new Rectangle(0, Height - goalOffset, Width, goalWidth);
            }
            else if (Position == Position.Top)
            {
                Paddle.Position = new Vector2(halfWinWidth, offset) - halfPaddleWidth;
                Goal.Rectangle = new Rectangle(0, goalOffset, Width, goalWidth);
            }

            var halfPaddleHeight = new Vector2(0, Paddle.Texture2D.Height / 2f);
            var halfWinHeight = Height / 2f;

            if (Position == Position.Left)
            {
                Paddle.Position = new Vector2(offset, halfWinHeight) - halfPaddleHeight;
                Goal.Rectangle = new Rectangle(goalOffset, 0, goalWidth, Height);
            }
            else if (Position == Position.Right)
            {
                Paddle.Position = new Vector2(Width - offset, halfWinHeight) - halfPaddleHeight;
                Goal.Rectangle = new Rectangle(Width - goalOffset, 0, goalWidth, Height);
            }
        }

        public Paddle Paddle { get; set; }
        public Goal Goal { get; set; }

        public void Update(GameTime gameTime, Vector2 viewPort, Ball ball, int width, int height, int boundarySize)
        {
            Goal.Update(ball, width, height);
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