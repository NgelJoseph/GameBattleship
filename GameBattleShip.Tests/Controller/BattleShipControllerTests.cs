using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using GameBattleShip.Models.Request;
using GameBattleShip.Tests.Integration.SetUp;
using NUnit.Framework;

namespace GameBattleShip.Tests.Controller
{
    [TestFixture]
    public class BattleShipControllerTests
    {
        [Test]
        public async Task GivenInValidRequestsThenReturnResponse()
        {
            using (var server = new ServerFixture())
            {
                var httpClient = new HttpClient { BaseAddress = new Uri(server.Url) };
                var response = await httpClient.PostAsJsonAsync("/createBoardAndPositionShips", new List<Ship> { new Ship { Name = "Destroyer", Width = 200 } });
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

                response = await httpClient.PostAsJsonAsync("/createBoardAndPositionShips", new List<Ship>());
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

                response = await httpClient.PostAsJsonAsync("/createBoardAndPositionShips", new List<Ship>
                {
                    new Ship { Name = "A", Width = 2 },
                    new Ship { Name = "B", Width = 3 },
                    new Ship { Name = "C", Width = 4 },
                    new Ship { Name = "D", Width = 5 },
                    new Ship { Name = "E", Width = 6 },
                    new Ship { Name = "F", Width = 7 }
                });
                response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }
        }
    }
}
