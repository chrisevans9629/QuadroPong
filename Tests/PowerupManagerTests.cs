using System;
using Microsoft.Xna.Framework;
using MyGame;
using NUnit.Framework;

namespace Tests
{
    public class PowerUpManagerTests
    {
        private PowerupManager powerupManager;
        [SetUp]
        public void Setup()
        {
            powerupManager = new PowerupManager();
        }

        [Test]
        public void Elapse1Second_Should_Pass()
        {
            powerupManager.AddTimedPowerup(new Sprite(), PowerUpType.BiggerBall, 1, Assert.Pass);
            var elapsed = TimeSpan.FromSeconds(1);
            powerupManager.UpdateTimedPowerup(new GameTime(elapsed, elapsed));
            Assert.Fail();
        }

        [Test]
        public void TwoPowerups_Should_DoubleTime()
        {
            var sprite = new Sprite();
            powerupManager.AddTimedPowerup(sprite, PowerUpType.FastBall, 1, Assert.Fail);
            powerupManager.AddTimedPowerup(sprite, PowerUpType.FastBall, 1, Assert.Fail);
            var elapsed = TimeSpan.FromSeconds(1);
            var time = new GameTime(elapsed, elapsed);
            powerupManager.UpdateTimedPowerup(time);
            Assert.True(powerupManager.HasPowerup(sprite, PowerUpType.FastBall));
        }

        [Test]
        public void TwoPowerups_Should_DoubleTimeFail()
        {
            var sprite = new Sprite();
            var count = 0;
            powerupManager.AddTimedPowerup(sprite, PowerUpType.FastBall, 1, () => count++);
            powerupManager.AddTimedPowerup(sprite, PowerUpType.FastBall, 1, () => count++);
            var elapsed = TimeSpan.FromSeconds(1);
            var time = new GameTime(elapsed, elapsed);
            powerupManager.UpdateTimedPowerup(time);
            powerupManager.UpdateTimedPowerup(time);

            Assert.AreEqual(1, count, "The action was not completed as expected");

            Assert.False(powerupManager.HasPowerup(sprite, PowerUpType.FastBall));
        }
    }
}