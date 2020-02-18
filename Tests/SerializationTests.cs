using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using Moq;
using MyGame.Levels;
using Newtonsoft.Json;
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

        [Test]
        public void SerializeRectangle()
        {
            var fixture = new Fixture();
            var rect = fixture.Create<RectangleF>();

            var result = JsonConvert.SerializeObject(rect);

            var rev = JsonConvert.DeserializeObject<RectangleF>(result);

            rect.Should().Be(rev);
        }
    }
}