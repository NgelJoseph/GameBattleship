using System;

namespace GameBattleShip.Models.Response
{
    public class ShipPosition
    {
        public string ShipName { get; set; }
        public Guid Id { get; set; }
        public int RowId { get; set; }
        public int ColumnId { get; set; }
    }
}
