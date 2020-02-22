namespace MyGame
{
    public class GameTimerState
    {
        public float EveryNumOfSeconds { get; set; } = 4f;

        public float CurrentTime { get; set; }

        public bool IsRunning { get; set; }
        public bool IsCompleted { get; set; }
    }
}