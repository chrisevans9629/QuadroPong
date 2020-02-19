namespace MyGame
{
    public class PongPlayerState
    {
        public PlayerStatsState StatsState { get; set; } = new PlayerStatsState();
        public Paddles Position { get; set; }
        public PaddleState PaddleState { get; set; } = new PaddleState();
        public GoalState GoalState { get; set; } = new GoalState();
        public bool Side => Position == Paddles.Left || Position == Paddles.Right;

    }
}