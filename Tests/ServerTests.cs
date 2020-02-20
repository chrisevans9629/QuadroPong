using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using MyGame;
using MyGame.Levels;
using NUnit.Framework;
using Server;

namespace Tests
{
    [TestFixture]
    public class ServerTests
    {
        private ServerClient client1;
        private ServerClient client2;
        private Fixture fixture;
        [SetUp]
        public async Task Setup()
        {
            fixture = new Fixture();
            var builder = new WebHostBuilder()
                .UseStartup<Startup>()
                .UseTestServer()
                .Build();
            builder.Start();

            var server = builder.GetTestServer(); 
            
            client1 = new ServerClient(server.CreateHandler());
            await client1.Start();

            client2 = new ServerClient(server.CreateHandler());
            await client2.Start();
        }

        [Test]
        public async Task LevelStates_Should_BeEmpty()
        {
            await client1.SendMove(fixture.Create<LevelState>());
            await Task.Delay(100);
            client1.LevelStates.Should().BeEmpty();
        }

        [Test]
        public void LevelStates_Default_Should_BeEmpty()
        {
            client1.LevelStates.Should().BeEmpty();
        }

        [Test]
        public async Task LevelStates_Should_HaveOne()
        {
            await client1.SendMove(fixture.Create<LevelState>());
            await Task.Delay(100);
            client2.LevelStates.Should().HaveCount(1);
        }

        [Test]
        public async Task Speeds_Should_Match()
        {
            var state = fixture.Create<LevelState>();
            await client1.SendMove(state);
            await Task.Delay(100);
            var stateResult = client2.LevelStates.First();
            stateResult.ShipState.SpriteState.Speed.Should().Be(state.ShipState.SpriteState.Speed);
        }

        [Test]
        public async Task Positions_Should_Match()
        {
            var state = fixture.Create<LevelState>();
            await client1.SendMove(state);
            await Task.Delay(100);
            var stateResult = client2.LevelStates.First();
            stateResult.ShipState.SpriteState.Position.Should().Be(state.ShipState.SpriteState.Position);
        }
    }
}