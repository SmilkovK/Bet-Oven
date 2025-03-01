using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportDomain.models;
using SportRepository;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using SportService.Implementation;

namespace Bet_Oven.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly FootballApiService _footballService; // Using your service

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, FootballApiService footballService)
        {
            _logger = logger;
            _context = context;
            _footballService = footballService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var totalCurrency = _context.Currencies
                                        .Where(t => t.BetUserId == userId)
                                        .Sum(t => t.currencyAmount);
            ViewBag.TotalCurrency = totalCurrency;

            // Fetch leagues and today's fixtures separately
            var leagues = await _footballService.GetLeagues();
            var fixtures = await _footballService.GetTodaysFixtures();

             //Group fixtures by league
            var fixturesGroupedByLeague = fixtures
                .GroupBy(f => f.League.Id)
                .ToDictionary(g => g.Key, g => g.ToList());

            //Pass both leagues & grouped fixtures to the view
            var viewModel = new LeagueMatchesViewModel
            {
                Leagues = leagues,
                FixturesGroupedByLeague = fixturesGroupedByLeague
            };

            return View(viewModel);
            return View();
        }
      

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
