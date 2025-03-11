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
        [HttpGet("leagues")]
        public async Task<ActionResult<List<AllLeagues>>> GetLeagues()
        {
            var leagues = await _footballApiService.GetLeagues();
            return Ok(leagues);
        }
        [HttpGet("fixtures/{leagueId}/{season}")]
        public async Task<ActionResult<List<Fixture>>> GetFixtures(int leagueId, int season)
        {
            var fixtures = await _footballApiService.GetFixtures(leagueId, season);
            return Ok(fixtures);
        }
        [HttpGet("{fixtureId}/stats")]
        public async Task<ActionResult<SportDomain.DTO.ApiStatsResponse>> GetMatchStats(int fixtureId)
        {
            var statsResponse = await _footballApiService.GetFixtureStatistics(fixtureId);
            if (statsResponse == null || statsResponse.Response == null || !statsResponse.Response.Any())
            {
                return NotFound();
            }
            return Ok(statsResponse);
        }
        [HttpGet("fixture/{fixtureId}")]
        public async Task<ActionResult<Fixture>> GetFixtureById(int fixtureId)
        {
            var fixture = await _footballApiService.GetFixtureById(fixtureId);
            if (fixture == null)
            {
                return NotFound();
            }
            return Ok(fixture);
        }
    }
}
