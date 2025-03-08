using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using SportDomain.models;
using SportDomain;
using System.Diagnostics;
using SportDomain.DTO;

namespace Bet_Oven.Controllers
{
    public class FixturesController : Controller
    {
        private readonly HttpClient _httpClient;

        public FixturesController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("http://localhost:5202/api/live-matches");

            if (!response.IsSuccessStatusCode)
            {
                return View(new List<Fixture>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var matches = JsonSerializer.Deserialize<List<Fixture>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(matches);
        }
        public async Task<IActionResult> MatchDetails(int fixtureId)
        {
            var fixtureResponse = await _httpClient.GetAsync($"http://localhost:5202/api/live-matches/fixture/{fixtureId}");
            if (!fixtureResponse.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var fixtureJson = await fixtureResponse.Content.ReadAsStringAsync();
            var fixture = JsonSerializer.Deserialize<Fixture>(fixtureJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (fixture == null)
            {
                return NotFound();
            }

            var statsResponse = await _httpClient.GetAsync($"http://localhost:5202/api/live-matches/{fixtureId}/stats");
            var stats = statsResponse.IsSuccessStatusCode
                ? JsonSerializer.Deserialize<ApiStatsResponse>(await statsResponse.Content.ReadAsStringAsync(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                : null;

            var viewModel = new FixtureDetailsViewModel
            {
                Fixture = fixture,
                Stats = stats
            };

            return View(viewModel);
        }




    }
}
