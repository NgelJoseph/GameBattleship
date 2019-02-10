using System.Collections.Generic;
using Newtonsoft.Json;

namespace GameBattleShip.Models.Response
{
    public class TrackerResult
    {
        public List<Tracker> Track { get; set; }
        public int AttemptsLeft { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Result { get; set; }
    }

    public class Tracker
    {
        public string ShipName { get; set; }
        public int AttemptId { get; set; }
        public int RowId { get; set; }
        public int ColumnId { get; set; }
        public string AttackStatus { get; set; }
    }
}
