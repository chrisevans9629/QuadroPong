using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;
using Server;

namespace Tests
{
    [TestFixture]
    public class ServerTests
    {
        [Test]
        public void Test()
        {
            var builder = new WebHostBuilder()
                .UseStartup<Startup>()
                .UseTestServer();
        }
    }
}