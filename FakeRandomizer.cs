namespace MyGame
{
    public class FakeRandomizer : IRandomizer
    {
        private float _result;

        public FakeRandomizer(float result)
        {
            _result = result;
        }
        public float NextFloat()
        {
            var t = _result;
            _result += 0.1f;
            return t;
        }

        public int Next(int count)
        {
            return count;
        }
    }
}