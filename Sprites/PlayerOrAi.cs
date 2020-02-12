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
    }
}