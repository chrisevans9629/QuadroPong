using Microsoft.Xna.Framework;

namespace MyGame
{
    public class Boundary : Sprite
    {
        public void Update(Ball ball)
        {
            if (Collision(ball))
            {
                if (BetweenY(ball))
                {
                    ball.Reflect(new Vector2(1, 0));
                }
                else if (BetweenX(ball))
                {
                    ball.Reflect(new Vector2(0, 1));
                }
                

            }
        }
    }
}