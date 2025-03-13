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
using SportService.Interface;
using SportService.Implementation; 

namespace Bet_Oven.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly FootballApiService _footballService;
        private readonly IFavoriteService _favoriteService; 

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context,
            FootballApiService footballService,
            IFavoriteService favoriteService)
        {
            _logger = logger;
            _context = context;
            _footballService = footballService;
            _favoriteService = favoriteService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var totalCurrency = _context.Currencies
                                        .Where(t => t.BetUserId == userId)
                                        .Sum(t => t.currencyAmount);
            ViewBag.TotalCurrency = totalCurrency;

            var leagues = await _footballService.GetLeagues();
            var fixtures = await _footballService.GetTodaysFixtures();
            var favoriteLeagues = _favoriteService.GetFavoriteLeagues(userId);

            var fixturesGroupedByLeague = fixtures
                .GroupBy(f => f.League.Id)
                .ToDictionary(g => g.Key, g => g.ToList());

            var viewModel = new LeagueMatchesViewModel
            {
                Leagues = leagues,
                FixturesGroupedByLeague = fixturesGroupedByLeague,
                FavoriteLeagues = new HashSet<int>(favoriteLeagues)
            };
            viewModel.Leagues = viewModel.Leagues.OrderByDescending(l => favoriteLeagues.Contains(l.League.Id)).ToList();

            return View(viewModel);
        }


        public IActionResult Privacy()
        {
            return View();
        }


        [HttpPost("ToggleFavorite")]
        public IActionResult ToggleFavorite(int leagueId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var favoriteService = HttpContext.RequestServices.GetService<IFavoriteService>();

            if (favoriteService.IsFavorite(userId, leagueId))
            {
                favoriteService.RemoveFavoriteLeague(userId, leagueId);
            }
            else
            {
                favoriteService.AddFavoriteLeague(userId, leagueId);
            }

            var updatedFavorites = favoriteService.GetFavoriteLeagues(userId);

            return Json(new { success = true, favoriteLeagues = updatedFavorites });
        }
    }
}