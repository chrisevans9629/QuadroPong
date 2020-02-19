using Microsoft.Xna.Framework;

namespace MyGame
{
    public class Boundary : Sprite
    {
        public void Update(Ball ball)
        {
            if (Collision(ball))
            {
                ball.Reflect(Center - ball.Center, 0);
            }
        }
    }
}