using ConwaysGameOfLife.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace ConwaysGameOfLife.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IGameOfLife _gameOfLife;

        public GameController(IGameOfLife gameOfLife)
        {
            _gameOfLife = gameOfLife;
        }
        /// <summary>
        /// API Endpoint to request for the next generation in the game
        /// based on the current generation input
        /// </summary>
        /// <param name="currentGen"></param>
        /// <returns>Next generation values</returns>
        [HttpGet]
        [Route("getnextgen/{currentGen}")]
        public ActionResult<string> Get(string currentGen)
        {
            try
            {
                var emptyArray = Array.Empty<int[]>();
                if (string.IsNullOrEmpty(currentGen))
                    return BadRequest();

                var result = ParseInput(currentGen);

                if (result == emptyArray)
                    return BadRequest();

                return Ok(_gameOfLife.GetNextGeneration(result));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Parses the input string into a jagged array and
        /// returns empty array if the input cannot be parsed
        /// </summary>
        /// <param name="currentGen"></param>
        /// <returns>Parsed value of current generation input</returns>
        private static int[][] ParseInput(string currentGen)
        {
            try
            {
                var result = JsonConvert.DeserializeObject<int[][]>(currentGen);
                if (result == null)
                    return Array.Empty<int[]>();

                return result;
            }
            catch (Exception)
            {
                return Array.Empty<int[]>();
            }
        }


    }
}