using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MyGame
{
    public class Paddle : Sprite
    {
        public void Update(GameTime time, Vector2 viewPortSize, Ball ball)
        {
            if (Collision(ball))
            {
                ball.Reflect();
            }

            var min = Vector2.Zero;
            var max = viewPortSize - EndPoint;

            var state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Up))
            {
                Acceleration = new Vector2(0, -1);
            }
            else if (state.IsKeyDown(Keys.Down))
            {
                Acceleration = new Vector2(0, 1);
            }
            else
            {
                return;
            }
            var newPos = Position +
                         Acceleration * (float)(Speed * time.ElapsedGameTime.TotalSeconds);

            Position = Vector2.Clamp(newPos, min, max);
        }
    }
}