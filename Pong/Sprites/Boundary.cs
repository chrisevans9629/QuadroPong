using Microsoft.Xna.Framework;

namespace MyGame
{
    public class Boundary : Sprite
    {
        public void Update(Ball ball)
        {
            if (Collision(ball))
            {
                //var side = this.OnRelativeSide(ball);
                //if (side != Direction.None)
                //{
                //    ball.Reflect(side);
                //}
                //else
                //{
                    ball.Reflect(Center - ball.Center, 0);
                //}
            }
        }
    }
}