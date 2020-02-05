﻿using Microsoft.Xna.Framework;

namespace MyGame
{
    public class Paddle : Sprite
    {
        private readonly IPlayer _player;

        public Paddle(IPlayer player)
        {
            _player = player;
        }

        public void Update(GameTime time, Vector2 viewPortSize, Ball ball)
        {
            if (Collision(ball))
            {
                ball.Reflect(new Vector2(1,0));
            }

            var min = Vector2.Zero;
            var max = viewPortSize - EndPoint;

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