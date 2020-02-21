using Microsoft.Xna.Framework;

namespace MyGame
{
    public class VectorT
    {
        public float X {get;set;}
        public float Y {get;set;}
        public float Z { get; set; }
        

        public static implicit operator Vector3(VectorT t)
        {
            return new Vector3(t.X, t.Y,t.Z);
        }

        public static implicit operator VectorT(Vector3 v)
        {
            return new VectorT() { Y = v.Y, X = v.X, Z = v.Z};
        }
        public static implicit operator Vector2(VectorT t)
        {
            return new Vector2(t.X, t.Y);
        }

        public static implicit operator VectorT(Vector2 v)
        {
            return new VectorT() { Y = v.Y, X = v.X};
        }

        public static implicit operator VectorT(Color color)
        {
            var t = color.ToVector3();
            return t;
        }

        public static implicit operator Color(VectorT vector)
        {
            return new Color(vector);
        }
    }
   

    public class SpriteState
    {
        public VectorT Position { get; set; } = new VectorT();
        public VectorT Size { get; set; } = Vector2.One;
        public float Speed { get; set; }
        public VectorT Acceleration { get; set; } = new VectorT();
        public VectorT Color { get; set; } = Microsoft.Xna.Framework.Color.White;
        public float Angle { get; set; }
        public float AngularVelocity { get; set; }
        public float Layer { get; set; }
        public Rectangle? Source { get; set; } = null;
    }
}