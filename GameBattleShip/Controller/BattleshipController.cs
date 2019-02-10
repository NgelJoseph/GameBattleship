using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GameBattleShip.Models.Request;
using GameBattleShip.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace GameBattleShip.Controller
{
    [ApiController]
    public class BattleshipController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IBattleshipService _boardService;

        public BattleshipController(IBattleshipService boardService)
        {
            _boardService = boardService;
        }

        [Route("createBoardAndPositionShips")]
        [HttpPost]
        public async Task<ActionResult> CreateBoardAndPositionShips([Required][FromBody]List<Ship> ships)
        {
            if (ships.Count < 1 || ships.Any(ship => ship.Width <= 0 || ship.Name == string.Empty))
            {
                return BadRequest("Invalid request. Add at least one valid ship to the board");
            }

            if (ships.Any(a => a.Width > 10))
            {
                return BadRequest("Invalid request. Max ship size is 10");
            }

            if (ships.Count >= 5)
            {
                return BadRequest("Invalid request. Max ship count is 5");
            }
            return await _boardService.CreateBoardAndPositionShips(ships);
        }

        [Route("attack")]
        [HttpGet]
        public async Task<ActionResult> Attack([Required]int row, [Required]int column)
        {
            return await _boardService.Play(row, column);
        }

        [Route("tracker")]
        [HttpGet]
        public async Task<ActionResult> Status()
        {
            return await _boardService.Track();
        }

        [Route("restart")]
        [HttpDelete]
        public async Task<ActionResult> RestartGame()
        {
            await _boardService.RestartGame();
            return Ok();
        }
    }
}