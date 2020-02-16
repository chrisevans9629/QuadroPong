using MyGame;
using NUnit.Framework;

namespace Tests
{
    [TestFixture]
    public class PowerUpTests
    {
        private PowerUp powerUp;
        [SetUp]
        public void Setup()
        {
            powerUp = new PowerUp(new Randomizer(), new PowerupManager());
        }


        [Test]
        public void SmallPaddle_Should_ChangePaddleSize()
        {
        }
    }
}