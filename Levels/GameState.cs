using Microsoft.Xna.Framework;

namespace MyGame.Levels
{
    public struct GameState
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public Rectangle ViewPort { get; set; }
        public bool IsDebug { get; set; }
        public bool IsSoundOn { get; set; }
    }
}