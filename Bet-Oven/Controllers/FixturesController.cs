using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using SportDomain.models;

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
    }
}
