using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Moq;
using MyGame.Levels;
using NUnit.Framework;
using PongGame = MyGame.PongGame;

namespace Tests
{
    [TestFixture]
    public class SerializationTests
    {
        [Test]
        public void Level_Serialize()
        {
            var fixture = new Fixture();
            fixture.Customize(new AutoMoqCustomization());

            var level = fixture.Build<RegularPongLevel>().OmitAutoProperties().Create();

            level.Initialize();

            var content = fixture.Freeze<Mock<IContentManager>>();
            content.Setup(p => p.Load<Texture2D>(It.IsAny<string>())).Returns(value: null);

            level.LoadContent(content.Object, fixture.Create<Point>());
            level.SaveGame();
        }
    }
}