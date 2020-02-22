namespace MyGame
{
    public class BallState
    {
        public SpriteState SpriteState { get; set; } = new SpriteState();
        public GameTimerState GameTimerState { get; set; } = new GameTimerState();
        public bool IsBallColliding { get; set; }

    }
}