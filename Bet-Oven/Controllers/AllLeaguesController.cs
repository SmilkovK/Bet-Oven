using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using System.Collections.Generic;
using SportDomain.models;
using SportService.Implementation;

namespace Bet_Oven.Controllers
{
    public class AllLeaguesController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly FootballApiService _footballApiService;

        public AllLeaguesController(HttpClient httpClient, FootballApiService footballApiService)
        {
            _httpClient = httpClient;
            _footballApiService = footballApiService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("http://localhost:5202/api/live-matches/leagues");

            if (!response.IsSuccessStatusCode)
            {
                return View(new List<Fixture>());
            }

            var json = await response.Content.ReadAsStringAsync();
            var leagues = JsonSerializer.Deserialize<List<AllLeagues>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(leagues);
        }
        public async Task<IActionResult> Standings(int leagueId, int season = 2023)
        {
            var standings = await _footballApiService.GetStandings(leagueId, season);

            var availableSeasons = new List<int> { 2021, 2022, 2023 };

            var viewModel = new StandingsViewModel
            {
                Standings = standings,
                AvailableSeasons = availableSeasons,
                SelectedSeason = season,
                LeagueId = leagueId
            };

            return View(viewModel);
        }
    }
}
