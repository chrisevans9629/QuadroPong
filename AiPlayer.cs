using System;
using Microsoft.Xna.Framework;

namespace MyGame
{
    public class AiPlayer : IPlayer
    {
        private readonly bool _isSides;

        public AiPlayer(bool isSides)
        {
            _isSides = isSides;
        }

        public bool UpdateAcceleration(Sprite sprite, Ball ball)
        {
            if (_isSides)
            {
                var playerY = sprite.Position.Y + (sprite.Texture2D.Height / 2f);

                var ballY = ball.Position.Y + ball.Texture2D.Height / 2f;

                if (Math.Abs(ballY - playerY) < 10)
                {
                    return false;
                }

                if (ballY > playerY)
                {
                    sprite.Acceleration = new Vector2(0, 1);
                }
                else if (ballY < playerY)
                {
                    sprite.Acceleration = new Vector2(0, -1);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                var playerX = sprite.Position.X + (sprite.Texture2D.Width / 2f);

                var ballX = ball.Position.X + ball.Texture2D.Width/ 2f;

                if (Math.Abs(ballX - playerX) < 10)
                {
                    return false;
                }

                if (ballX > playerX)
                {
                    sprite.Acceleration = new Vector2(1, 0);
                }
                else if (ballX < playerX)
                {
                    sprite.Acceleration = new Vector2(-1, 0);
                }
                else
                {
                    return false;
                }
            }

            
            

            return true;
        }
    }
}