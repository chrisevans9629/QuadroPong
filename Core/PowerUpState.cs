namespace MyGame
{
    public class PowerUpState
    {
        public SpriteState SpriteState { get; set; } = new SpriteState();
        public PowerUpType PowerUpType { get; set; }
        public float Power { get; set; } = 200f;

    }
}