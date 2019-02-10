using System;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using GameBattleShip.Tests.Integration.SetUp;
using NUnit.Framework;

namespace GameBattleShip.Tests.Controller
{
    [TestFixture]
    public class HealthTests
    {
        [Test]
        public async Task GivenAHealthyApi_ThenHealthyResponseReturned()
        {
            const string expected = "{\"dbHealth\":true}";
            using (var server = new ServerFixture())
            {
                var httpClient = new HttpClient { BaseAddress = new Uri(server.Url) };
                var response = await httpClient.GetStringAsync("/health");
                response.Should().BeEquivalentTo(expected);
            }
        }
    }
}