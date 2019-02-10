using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using GameBattleShip.Data;
using GameBattleShip.Models;
using GameBattleShip.Models.Request;
using GameBattleShip.Models.Response;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace GameBattleShip.Services
{
    public class BattleshipService : IBattleshipService
    {
        private readonly IData _data;
        private readonly IPositionService _positionService;
        private readonly ILogger _logger;

        private const string SelectPosition = "SELECT id FROM shipPosition WHERE rowId=@RowId and columnId=@ColumnId";
        private const string InsertPlayResult = "INSERT INTO playerInfo (rowId, columnId, attackStatus, shipId) VALUES(@RowId, @ColumnId, @Status, @ShipId)";
        private const string SelectShipWidth = "SELECT COUNT(*) FROM shipPosition";
        private const string SelectPlayResult = "SELECT pi.attemptid, pi.rowid, pi.columnid, pi.attackstatus, sp.shipname from playerInfo pi INNER JOIN shipposition sp ON pi.shipid = sp.id";
        public BattleshipService(IPositionService positionService, IData data, ILogger logger)
        {
            _positionService = positionService;
            _data = data;
            _logger = logger;
        }

        public async Task<ActionResult> CreateBoardAndPositionShips(List<Ship> ships)
        {
            try
            {
                foreach (var ship in ships)
                {
                    await _positionService.AllocateCoordinates(ship);
                }
                return new OkResult();
            }
            catch(Exception ex)
            {
                _logger.Error($"Exception {ex} while positioning {ships}");
                return new StatusCodeResult((int) HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ActionResult> Play(int row, int column)
        {
            try
            {
                var shipId = await _data.QueryAsync<string>(SelectPosition, new { RowId = row, ColumnId = column });
                var status = !shipId.Equals(Guid.Empty.ToString()) ? "HIT" : "MISS";
                await _data.ExecuteAsync(InsertPlayResult, new { Status = status, RowId = row, ColumnId = column, ShipId = shipId });
                return new OkObjectResult(status);
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception {ex} while attacking at row {row} and column {column}");
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ActionResult> Track()
        {
            try
            {
                var positionsMarkedTask = _data.QueryAsync<int>(SelectShipWidth);
                var playResultTask = _data.QueryAsyncList<Tracker>(SelectPlayResult);

                await Task.WhenAll(positionsMarkedTask, playResultTask);
                var playResult = playResultTask.Result;
                var shipWidthResult = positionsMarkedTask.Result;
                var attemptsLeft = shipWidthResult + 2 - playResult.Count;

                var result = new TrackerResult
                {
                    AttemptsLeft = attemptsLeft,
                    Track = playResult,
                    Result = playResult.Count(a => a.AttackStatus == "HIT") >= shipWidthResult ? "You have won the game" : attemptsLeft == 0 ? "You have lost the game" : null
                };
                return new OkObjectResult(result);
            }
            catch (Exception ex)
            {
                _logger.Error($"Exception {ex} while tracking status");
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        public async Task RestartGame()
        {
            await _positionService.Clear();
        }
    }

    public interface IBattleshipService
    {
        Task<ActionResult> CreateBoardAndPositionShips(List<Ship> ships);
        Task<ActionResult> Play(int rowToHit, int columnToHit);
        Task<ActionResult> Track();
        Task RestartGame();
    }
}
