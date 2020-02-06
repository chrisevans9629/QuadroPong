using Microsoft.Xna.Framework;

namespace MyGame
{
    public class Boundary : Sprite
    {
        public void Update(Ball ball)
        {
            if (Collision(ball))
            {
                ReflectHelper.ReflectBall(this, ball);
            }
        }
    }
}