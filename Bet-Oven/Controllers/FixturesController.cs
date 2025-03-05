using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using SportDomain.models;
using SportDomain;
using System.Diagnostics;

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
            try
            {
                if (fixtureId <= 0)
                {
                    return RedirectToAction("Error", "Home", new { message = "Invalid match ID" });
                }

                var response = await _httpClient.GetAsync($"http://localhost:5202/api/live-matches/{fixtureId}/stats");

                if (!response.IsSuccessStatusCode)
                {
                    return View("StatsError", new ErrorViewModel
                    {
                        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                        Message = "Statistics unavailable for this match"
                    });
                }

                var json = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"API Response: {json}");

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var stats = JsonSerializer.Deserialize<MatchStats>(json, options) ?? new MatchStats();

                if (stats.Home == null) stats.Home = new TeamStatistics();
                if (stats.Away == null) stats.Away = new TeamStatistics();

                return View(stats);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching stats: {ex}");
                return RedirectToAction("Error", "Home", new { message = "Failed to load match details" });
            }
        }
    }
}
