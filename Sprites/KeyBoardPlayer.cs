using System.Linq;
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
        public bool UpdateAcceleration(Sprite sprite, Ball ball)
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
                    return true;
                }
            }
            return false;
        }

        public bool IsConnected { get; set; }
    }

    public class PlayerOrAi : IPlayerController
    {
        public PlayerOrAi(bool isSide, params IPlayerController[] userPlayer)
        {
            AiPlayer = new AiPlayer(isSide);
            UserPlayers = userPlayer;
        }

        public AiPlayer AiPlayer { get; set; }
        public IPlayerController[] UserPlayers { get; set; }

        public bool UpdateAcceleration(Sprite sprite, Ball ball)
        {
            return UserPlayers.Any(p => p.UpdateAcceleration(sprite, ball)) || AiPlayer.UpdateAcceleration(sprite, ball);
        }
    }


    public class KeyBoardPlayer : IPlayerController
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
            else if (state.IsKeyDown(Keys.Left))
            {
                sprite.Acceleration = new Vector2(-1, 0);
            }
            else if (state.IsKeyDown(Keys.Right))
            {
                sprite.Acceleration = new Vector2(1, 0);
            }
            else
            {
                return false;
            }

            return true;
        }

    }
}