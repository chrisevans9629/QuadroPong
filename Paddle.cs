using Microsoft.Xna.Framework;

namespace MyGame
{
    public class Paddle : Sprite
    {
        private readonly IPlayer _player;

        public Paddle(IPlayer player)
        {
            _player = player;
        }

        public void Update(GameTime time, Vector2 min, Vector2 maxPort, Ball ball)
        {
            if (Collision(ball))
            {
                ReflectHelper.ReflectBall(this, ball);
            }

            var max = maxPort - EndPoint;

            if (!_player.UpdateAcceleration(this, ball))
            {
                return;
            }
            
            var newPos = Position +
                         Acceleration * (float)(Speed * time.ElapsedGameTime.TotalSeconds);

            Position = Vector2.Clamp(newPos, min, max);
        }

        
    }
}