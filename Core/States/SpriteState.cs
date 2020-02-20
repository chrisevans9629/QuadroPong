using Microsoft.Xna.Framework;

namespace MyGame
{
    public struct VectorT
    {
        public float X;
        public float Y;

        public VectorT(float x, float y)
        {
            X = x;
            Y = y;
        }
        public static implicit operator Vector2(VectorT t)
        {
            return new Vector2(t.X, t.Y);
        }

        public static implicit operator VectorT(Vector2 v)
        {
            return new VectorT() { Y = v.Y, X = v.X };
        }
    }
    //public class SpriteStateTest
    //{
    //    public VectorT Position { get; set; }
    //    public VectorT Size { get; set; } = Vector2.One;
    //    public float Speed { get; set; }
    //    public VectorT Acceleration { get; set; }
    //    public Color Color { get; set; } = Color.White;
    //    public float Angle { get; set; }
    //    public float AngularVelocity { get; set; }
    //    public float Layer { get; set; }
    //    public Rectangle? Source { get; set; } = null;
    //}
    public class SpriteState
    {
        public VectorT Position { get; set; }
        public VectorT Size { get; set; } = Vector2.One;
        public float Speed { get; set; }
        public VectorT Acceleration { get; set; }
        public Color Color { get; set; } = Color.White;
        public float Angle { get; set; }
        public float AngularVelocity { get; set; }
        public float Layer { get; set; }
        public Rectangle? Source { get; set; } = null;
    }
}