using System;
using MonoGame.Extended;

namespace MyGame
{
    public class Randomizer : IRandomizer
    {
        private FastRandom random2;
        public Randomizer()
        { 
            random2 = new FastRandom();
        }



        public float NextFloat()
        {
            return random2.NextSingle();
        }

        public int Next(int count)
        {
            return random2.Next(count-1);
        }

        public int Next(int min, int max)
        {
            return random2.Next(min, max);
        }

        public float Next(float min, float max)
        {
            return random2.NextSingle(min, max);
        }

        public void Reset(int seed)
        {
            random2 = new FastRandom(seed);
        }
    }
}