using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using GameBattleShip.Models.Request;
using GameBattleShip.Models.Response;
using GameBattleShip.Tests.Integration.SetUp;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GameBattleShip.Tests.Integration.EndToEndTests
{
    [TestFixture]
    public class EndToEndTests
    {
        [Test]
        public async Task GivenIWantToPlay_ThenCreateBattleshipBoardPlayAndTrackResult()
        {
            using (var server = new ServerFixture())
            {
                // A game with 2 ships - Destroyer and BattleShip of width 2 and 5 respectively
                var httpClient = new HttpClient { BaseAddress = new Uri(server.Url) };
                var firstAttemptResult = await httpClient.PostAsJsonAsync("/createBoardAndPositionShips", new List<Ship> {new Ship { Name = "Destroyer", Width = 2 }, new Ship { Name = "Battleship", Width = 5 }});
                firstAttemptResult.StatusCode.Should().Be(HttpStatusCode.OK);
                var shipPositions = await server.SqlClient.QueryAsyncList<ShipPosition>("SELECT rowid AS RowId, columnid AS ColumnId FROM shipposition");
                shipPositions.Count.Should().Be(7);

                var savedPositionInDb1 = shipPositions.FirstOrDefault();
                var attackResult = await httpClient.GetStringAsync($"attack?row={savedPositionInDb1?.RowId}&column={savedPositionInDb1?.ColumnId}");
                attackResult.Should().BeOfType<string>().Which.Should().Be("HIT");

                var trackResult = await httpClient.GetStringAsync("tracker");
                var tracker = JsonConvert.DeserializeObject<TrackerResult>(trackResult);
                tracker.Track.Count.Should().Be(1);
                var firstOrDefault = tracker.Track.FirstOrDefault();
                firstOrDefault?.RowId.Should().Be(savedPositionInDb1?.RowId);
                firstOrDefault?.ColumnId.Should().Be(savedPositionInDb1?.ColumnId);
                firstOrDefault?.AttackStatus.Should().Be("HIT");

                //A game with one ship - Destroyer and BattleShip of width 2
                await httpClient.DeleteAsync("/restart");
                var secondGameResult = await httpClient.PostAsJsonAsync("/createBoardAndPositionShips", new List<Ship> { new Ship { Name = "Destroyer", Width = 2 } });
                secondGameResult.StatusCode.Should().Be(HttpStatusCode.OK);
                var secondGamesShipPositions = await server.SqlClient.QueryAsyncList<ShipPosition>("SELECT rowid AS RowId, columnid AS ColumnId FROM shipposition");
                secondGamesShipPositions.Count.Should().Be(2);
                
                var attackResult2 = await httpClient.GetStringAsync($"attack?row={secondGamesShipPositions.ToList()[0].RowId}&column={secondGamesShipPositions.ToList()[0].ColumnId}");
                attackResult2.Should().BeOfType<string>().Which.Should().Be("HIT");
                attackResult2 = await httpClient.GetStringAsync($"attack?row={secondGamesShipPositions.ToList()[1].RowId}&column={secondGamesShipPositions.ToList()[1].ColumnId}");
                attackResult2.Should().BeOfType<string>().Which.Should().Be("HIT");

                var trackResult2 = await httpClient.GetStringAsync("tracker");
                var tracker2 = JsonConvert.DeserializeObject<TrackerResult>(trackResult2);
                tracker2.Track.Count.Should().Be(2);
                tracker2.Result.Should().Be("You have won the game");
            }
        }
    }
}