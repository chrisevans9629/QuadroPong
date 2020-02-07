using System;
using Microsoft.Xna.Framework;

namespace MyGame
{
    public class ReflectHelper
    {
        public static void ReflectBall(Collider collider, Ball ball)
        {
            ball.Reflect(collider.OnRelativeSide(ball));
            //var pi = (float)Math.PI;

            //var t = 0.20f;

            //if (collider.BetweenY(ball))
            //{
            //    ball.Reflect(new Vector2(1, 0), t, pi-t);
            //}
            //else if (collider.BetweenX(ball))
            //{
            //    ball.Reflect(new Vector2(0, 1), (-pi/2f) + t, (pi/2f)-t);
            //}
            //else
            //{
            //    ball.Reflect(new Vector2(1,1),-pi, pi);
            //}
        }
    }
}