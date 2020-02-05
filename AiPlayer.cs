using System;
using Microsoft.Xna.Framework;

namespace MyGame
{
    public class AiPlayer : IPlayer
    {
        public bool UpdateAcceleration(Sprite sprite, Ball ball)
        {
            var playerY = sprite.Position.Y + (sprite.Texture2D.Height/2f);

            var ballY = ball.Position.Y + ball.Texture2D.Height/2f;

            if (Math.Abs(ballY - playerY) < 10)
            {
                return false;
            }

            if (ballY > playerY)
            {
                sprite.Acceleration = new Vector2(0,1);
            }
            else if (ballY < playerY)
            {
                sprite.Acceleration = new Vector2(0,-1);
            }
            else
            {
                return false;
            }
            

            return true;
        }
    }
}