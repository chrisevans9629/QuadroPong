using MonoGame.Extended;

namespace MyGame
{
    public class GoalState
    {
        public const int DefaultHealth = 10;

        public RectangleF Rectangle { get; set; }
        public Paddles Paddles { get; set; }
        public int Health { get; set; } = DefaultHealth;
        public int Offset { get; set; }
        public bool Died { get; set; }
        public bool SoundOn { get; set; } = true;

    }
}