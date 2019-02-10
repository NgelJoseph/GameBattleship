using System.Threading.Tasks;
using GameBattleShip.Data;
using Microsoft.AspNetCore.Mvc;

namespace GameBattleShip.Controller
{
    [Route("health")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        private readonly IData _data;

        public HealthController(IData data)
        {
            _data = data;
        }

        [HttpGet]
        public async Task<ActionResult> Healthy()
        {
            var dbHealth = await _data.IsHealthy();

            return Ok(new
            {
                DbHealth = dbHealth
            });
        }
    }
}
