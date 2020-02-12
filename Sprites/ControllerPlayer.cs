using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MyGame
{
    public class ControllerPlayer : IPlayerController
    {
        public PlayerIndex PlayerIndex { get; set; }
        public ControllerPlayer(PlayerIndex playerIndex)
        {
            PlayerIndex = playerIndex;
        }

        public bool HasPressedA { get; set; }
        public InputResult UpdateAcceleration(Sprite sprite, Ball ball)
        {
            var ability = GamePad.GetCapabilities(PlayerIndex);
            IsConnected = ability.IsConnected;
            if (ability.IsConnected)
            {
                var state = GamePad.GetState(PlayerIndex);

                if (ability.HasAButton && state.Buttons.A == ButtonState.Pressed)
                {
                    HasPressedA = true;
                }

                if (ability.HasBButton && state.Buttons.B == ButtonState.Pressed)
                {
                    HasPressedA = false;
                }

                if (ability.HasLeftXThumbStick && ability.HasLeftYThumbStick && HasPressedA)
                {
                    var left = state.ThumbSticks.Left;
                    sprite.Acceleration = new Vector2(left.X, -left.Y);

                    if (left == Vector2.Zero)
                    {
                        return new InputResult(){HasMoved = false, IsHandled = true};
                    }

                    Console.WriteLine($"{PlayerIndex}:{left}");


                    return new InputResult() { HasMoved = true, IsHandled = true };
                }
            }
            return new InputResult();
        }

        public bool IsConnected { get; set; }
    }
}