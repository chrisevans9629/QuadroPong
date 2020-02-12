using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MyGame
{
    public class KeyBoardPlayer : IPlayerController
    {
        public bool HasPressedEnter { get; set; }

        public InputResult UpdateAcceleration(Sprite sprite, Ball ball)
        {

            var state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Enter))
            {
                HasPressedEnter = true;
            }

            if (state.IsKeyDown(Keys.Delete))
            {
                HasPressedEnter = false;
            }

            if (!HasPressedEnter)
            {
                return new InputResult();
            }

            var moved = false;

            if (state.IsKeyDown(Keys.Up))
            {
                sprite.Acceleration = new Vector2(0, -1);
                moved = true;
            }
            else if (state.IsKeyDown(Keys.Down))
            {
                sprite.Acceleration = new Vector2(0, 1);
                moved = true;
            }
            else if (state.IsKeyDown(Keys.Left))
            {
                sprite.Acceleration = new Vector2(-1, 0);
                moved = true;
            }
            else if (state.IsKeyDown(Keys.Right))
            {
                sprite.Acceleration = new Vector2(1, 0);
                moved = true;
            }

            return new InputResult() { HasMoved = moved, IsHandled = true };
        }

    }
}