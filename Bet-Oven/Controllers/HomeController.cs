using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportDomain.Identity;
using SportDomain.models;
using SportRepository;
using SportService.Implementation;
using SportService.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Bet_Oven.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly FootballApiService _footballService;
        private readonly IFavoriteService _favoriteService;
        private readonly UserManager<BetUser> _userManager;

        private readonly int[] PopularLeagueIds =
        {
            39,
            140,
            135, 
            78, 
            61  
        };

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context,
            FootballApiService footballService,
            IFavoriteService favoriteService,
            UserManager<BetUser> userManager)
        {
            _logger = logger;
            _context = context;
            _footballService = footballService;
            _favoriteService = favoriteService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var favoriteLeagues = new List<int>();

            if (user != null)
            {
                favoriteLeagues = await _context.FavoriteLeagues
                    .Where(f => f.UserId == user.Id)
                    .Select(f => f.LeagueId)
                    .ToListAsync();
            }

            var fixtures = await _footballService.GetTodaysFixtures(PopularLeagueIds);

            var fixturesGroupedByLeague = fixtures
                .Where(f => f.League != null)
                .GroupBy(f => f.League.Id)
                .ToDictionary(g => g.Key, g => g.ToList());

            var leagues = fixtures
                .Where(f => f.League != null)
                .Select(f => new AllLeagues
                {
                    League = f.League,
                    Country = new CountryInfo { Name = f.League.Country },
                    Seasons = new List<SeasonInfo>()
                })
                .GroupBy(l => l.League.Id)
                .Select(g => g.First())
                .OrderByDescending(l => favoriteLeagues.Contains(l.League.Id))
                .ThenBy(l => l.League.Name)
                .ToList();

            var model = new LeagueMatchesViewModel
            {
                Leagues = leagues,
                FixturesGroupedByLeague = fixturesGroupedByLeague
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetOtherFixtures(int skip = 0, int take = 10)
        {
            var fixtures = await _footballService.GetTodaysFixtures();

            var otherFixtures = fixtures
                .Where(f => !PopularLeagueIds.Contains(f.League.Id))
                .GroupBy(f => f.League.Id)
                .Skip(skip)
                .Take(take)
                .ToDictionary(g => g.Key, g => g.ToList());

            return Json(otherFixtures);
        }

        [HttpPost("ToggleFavorite")]
        public IActionResult ToggleFavorite(int leagueId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (_favoriteService.IsFavorite(userId, leagueId))
            {
                _favoriteService.RemoveFavoriteLeague(userId, leagueId);
            }
            else
            {
                _favoriteService.AddFavoriteLeague(userId, leagueId);
            }

            var updatedFavorites = _favoriteService.GetFavoriteLeagues(userId);

            return Json(new { success = true, favoriteLeagues = updatedFavorites });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Terms()
        {
            return View();
        }
    }
}
