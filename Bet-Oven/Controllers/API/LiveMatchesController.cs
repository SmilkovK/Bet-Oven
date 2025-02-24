using Microsoft.AspNetCore.Mvc;
using SportService.Implementation;
using SportDomain.models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bet_Oven.Controllers.API
{
    [Route("api/live-matches")]
    [ApiController]
    public class LiveMatchesController : ControllerBase
    {
        private readonly FootballApiService _footballApiService;

        public LiveMatchesController(FootballApiService footballApiService)
        {
            _footballApiService = footballApiService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Fixture>>> GetLiveMatches()
        {
            var result = await _footballApiService.GetLiveMatches();
            return Ok(result);
        }
    }
}
