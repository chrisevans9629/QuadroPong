using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace MyGame
{
    public struct Circle
    {
        public Circle(float x, float y, float radius)
            : this()
        {
            X = x;
            Y = y;
            Radius = radius;
        }

        public float Radius { get; private set; }
        public float X { get; private set; }
        public float Y { get; private set; }

        public bool Intersects(RectangleF rectangle)
        {
            // the first thing we want to know is if any of the corners intersect
            var corners = new[]
            {
                new Vector2(rectangle.Top, rectangle.Left),
                new Vector2(rectangle.Top, rectangle.Right),
                new Vector2(rectangle.Bottom, rectangle.Right),
                new Vector2(rectangle.Bottom, rectangle.Left)
            };

            foreach (var corner in corners)
            {
                if (ContainsPoint(corner))
                    return true;
            }

            // next we want to know if the left, top, right or bottom edges overlap
            if (X - Radius > rectangle.Right || X + Radius < rectangle.Left)
                return false;

            if (Y - Radius > rectangle.Bottom || Y + Radius < rectangle.Top)
                return false;

            return true;
        }

        public bool Intersects(Circle circle)
        {
            // put simply, if the distance between the circle centre's is less than
            // their combined radius
            var centre0 = new Vector2(circle.X, circle.Y);
            var centre1 = new Vector2(X, Y);
            return Vector2.Distance(centre0, centre1) < Radius + circle.Radius;
        }

        public bool ContainsPoint(Vector2 point)
        {
            var vector2 = point;
            return vector2.Length() <= Radius;
        }
    }
}