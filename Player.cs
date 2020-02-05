using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MyGame
{
    public class Player : IPlayer
    {
        public bool UpdateAcceleration(Sprite sprite, Ball ball)
        {
            var state = Keyboard.GetState();
            if (state.IsKeyDown(Keys.Up))
            {
                sprite.Acceleration = new Vector2(0, -1);
            }
            else if (state.IsKeyDown(Keys.Down))
            {
                sprite.Acceleration = new Vector2(0, 1);
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}