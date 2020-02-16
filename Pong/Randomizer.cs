using System;
using MonoGame.Extended;

namespace MyGame
{
    public class Randomizer : IRandomizer
    {
        private Random random;
        private FastRandom random2;
        public Randomizer()
        { 
            random2 = new FastRandom();
            
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

        public int Next(int min, int max)
        {
            return random.Next(min, max);
        }

        public float Next(float min, float max)
        {
            return random2.NextSingle(min, max);
        }
    }
}