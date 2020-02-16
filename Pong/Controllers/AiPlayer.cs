using System;
using Microsoft.Xna.Framework;

namespace MyGame
{
    public class AiPlayer : IPlayerController
    {
        private readonly bool _isSides;

        public AiPlayer(bool isSides)
        {
            _isSides = isSides;
        }

        public InputResult UpdateAcceleration(Sprite sprite, Ball ball)
        {
            var moved = false;
            if (_isSides)
            {
                var playerY = sprite.Position.Y + (sprite.Texture2D.Height / 2f);

                var ballY = ball.Position.Y + ball.Texture2D.Height / 2f;

                if (Math.Abs(ballY - playerY) < 10)
                {
                }
                else if (ballY > playerY)
                {
                    sprite.Acceleration = new Vector2(0, 1);
                    moved = true;

                }
                else if (ballY < playerY)
                {
                    sprite.Acceleration = new Vector2(0, -1);
                    moved = true;
                }
            }
            else
            {
                var playerX = sprite.Position.X + (sprite.Texture2D.Width / 2f);

                var ballX = ball.Position.X + ball.Texture2D.Width / 2f;

                if (Math.Abs(ballX - playerX) < 10)
                {
                }
                else if (ballX > playerX)
                {
                    sprite.Acceleration = new Vector2(1, 0);
                    moved = true;
                }
                else if (ballX < playerX)
                {
                    sprite.Acceleration = new Vector2(-1, 0);
                    moved = true;
                }
            }

            return new InputResult() { HasMoved = moved, IsHandled = true };
        }

        public InputResult<bool> TriggerPressed()
        {
            return new InputResult<bool>() { Value = true, IsHandled = true };
        }

        public InputResult<Vector2> GetDirectional(Vector2 defaultVector2)
        {
            return new InputResult<Vector2>() { Value = defaultVector2, IsHandled = true };
        }
    }
}