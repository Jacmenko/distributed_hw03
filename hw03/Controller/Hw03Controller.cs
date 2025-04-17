using hw03.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace hw03.Controllers
{


    [ApiController]
    [Route("hw03")]
    public class Hw03Controller : ControllerBase
    {
        private readonly Hw03Service _hw03Service;

        public Hw03Controller()
        {
            _hw03Service = new Hw03Service();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? queryAirportTemp, [FromQuery] string? queryStockPrice, [FromQuery] string? queryEval)
        {
            var result = await _hw03Service.Get(queryAirportTemp, queryStockPrice, queryEval);

            if (result == null)
                return BadRequest("Invalid request.");

            return Content(result.ToString(), "application/json");
        }
    }
}
