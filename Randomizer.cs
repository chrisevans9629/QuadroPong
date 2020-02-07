using System;

namespace MyGame
{
    public class Randomizer : IRandomizer
    {
        private Random random;
        public Randomizer()
        {
            random = new Random();
        }

        public float NextFloat()
        {
            return (float)random.NextDouble();
        }

        public int Next(int count)
        {
            return random.Next(count);
        }
    }
}