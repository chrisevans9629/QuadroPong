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

        public InputResult<bool> TriggerPressed()
        {
            var ability = GamePad.GetCapabilities(PlayerIndex);
            IsConnected = ability.IsConnected;
            if (ability.IsConnected && ability.HasRightTrigger && HasPressedA)
            {
                var state = GamePad.GetState(PlayerIndex);
                return new InputResult<bool>(){Value = state.Triggers.Right > 0 , IsHandled = true};
            }
            return new InputResult<bool>();
        }

        public InputResult<Vector2> GetDirectional(Vector2 defaultVector2)
        {
            var ability = GamePad.GetCapabilities(PlayerIndex);
            IsConnected = ability.IsConnected;
            if (ability.IsConnected && ability.HasRightYThumbStick && ability.HasRightXThumbStick && HasPressedA)
            {
                var state = GamePad.GetState(PlayerIndex);
                var left = state.ThumbSticks.Right;
                if (left == Vector2.Zero)
                {
                    return new InputResult<Vector2>(){IsHandled = true, Value = defaultVector2};
                }
                return new InputResult<Vector2>(){IsHandled = true, Value = left };
            }

            return new InputResult<Vector2>(){IsHandled = false, Value = defaultVector2};
        }

        public bool IsConnected { get; set; }
    }
}