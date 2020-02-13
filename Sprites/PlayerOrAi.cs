using System;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MyGame
{
    public class PlayerOrAi : IPlayerController
    {
        public PlayerOrAi(bool isSide, params IPlayerController[] userPlayer)
        {
            AiPlayer = new AiPlayer(isSide);
            UserPlayers = userPlayer;
        }

        public AiPlayer AiPlayer { get; set; }
        public IPlayerController[] UserPlayers { get; set; }

        public InputResult UpdateAcceleration(Sprite sprite, Ball ball)
        {
            foreach (var playerController in UserPlayers)
            {
                var result = playerController.UpdateAcceleration(sprite, ball);
                if (result.IsHandled)
                {
                    return result;
                }
            }
            return AiPlayer.UpdateAcceleration(sprite, ball);
        }

        public InputResult<bool> TriggerPressed()
        {
            foreach (var playerController in UserPlayers)
            {
                var result = playerController.TriggerPressed();
                if (result.IsHandled)
                    return result;
            }
            return AiPlayer.TriggerPressed();
        }

        public InputResult<Vector2> GetDirectional(Vector2 defaultVector2)
        {
            foreach (var items in UserPlayers)
            {
                var d = items.GetDirectional(defaultVector2);
                if (d.IsHandled)
                    return d;
            }

            return AiPlayer.GetDirectional(defaultVector2);
        }
    }
}