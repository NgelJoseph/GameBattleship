using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameBattleShip.Data;
using GameBattleShip.Helpers;
using GameBattleShip.Models.Request;
using GameBattleShip.Models.Response;

namespace GameBattleShip.Services
{
    public class PositionService : IPositionService
    {
        private readonly IData _data;
        private readonly IRandomGenerator _randomGenerator;
        private readonly ICacheProvider _cacheProvider;
        private const string InsertCoordinates = "INSERT INTO shipPosition (shipName, rowId, columnId, id) VALUES(@Name, @RowId, @ColumnId, @Id)";
        private const string DeleteCoordinates = "DELETE FROM shipPosition";
        private const string DeletePlayerInfo = "DELETE FROM playerInfo";

        public PositionService(ICacheProvider cacheProvider, IRandomGenerator randomGenerator, IData data)
        {
            _cacheProvider = cacheProvider;
            _randomGenerator = randomGenerator;
            _data = data;
        }

        public async Task AllocateCoordinates(Ship ship)
        {
            List<ShipPosition> allocatedPositions = null;
            int startRow, endRow, startColumn, endColumn;

            while (true)
            {
                startColumn = endColumn = _randomGenerator.Random(1, 10);
                startRow = endRow = _randomGenerator.Random(1, 10);
                var orientation = _randomGenerator.Random(1, 101) % 2;

                if (orientation == 0)
                {
                    for (var i = 1; i < ship.Width; i++)
                    {
                        endRow++;
                    }
                }
                else
                {
                    for (var i = 1; i < ship.Width; i++)
                    {
                        endColumn++;
                    }
                }

                if (endRow > 10 || endColumn > 10)
                {
                    continue;
                }

                for (var rowId = startRow; rowId <= endRow; rowId++)
                {
                    for (var columnId = startColumn; columnId <= endColumn; columnId++)
                    {
                        allocatedPositions = await _cacheProvider.GetValueAsync<List<ShipPosition>>("shipPositions");
                        if (allocatedPositions == null || allocatedPositions.Count == 0) continue;
                        if (allocatedPositions.Any(a => a.RowId == rowId && a.ColumnId == columnId))
                            continue;
                    }
                }
                break;
            }

            await SaveCoordinates(startRow, endRow, startColumn, endColumn, ship.Name, allocatedPositions?? new List<ShipPosition>());
        }

        public async Task Clear()
        {
            var deleteCacheTask = _cacheProvider.RemoveItemAsync("shipPositions");
            var deleteDbCoordinatesTask = _data.ExecuteAsync(DeleteCoordinates);
            var deletePlayerInfoTask = _data.ExecuteAsync(DeletePlayerInfo);
            await Task.WhenAll(deleteCacheTask, deleteDbCoordinatesTask, deletePlayerInfoTask);
        }

        private async Task SaveCoordinates(int startRow, int endRow, int startColumn, int endColumn, string shipName, List<ShipPosition> allocatedPositions)
        {
            for (var rowId = startRow; rowId <= endRow; rowId++)
            {
                for (var columnId = startColumn; columnId <= endColumn; columnId++)
                {
                    var shipId = Guid.NewGuid();
                    await _data.ExecuteAsync(InsertCoordinates, new { Name = shipName, RowId = rowId, ColumnId = columnId, Id = shipId });
                    allocatedPositions.Add(new ShipPosition
                    {
                        Id = shipId,
                        ShipName = shipName,
                        RowId = rowId,
                        ColumnId = columnId
                    });
                }
            }
            await _cacheProvider.SetValueAsync(allocatedPositions, "shipPositions", 3600);
        }
    }

    public interface IPositionService
    {
        Task AllocateCoordinates(Ship ship);
        Task Clear();
    }
}
