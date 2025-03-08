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
                FavoriteLeagues = new HashSet<int>(_favoriteService.GetFavoriteLeagues(userId))
            };
            viewModel.Leagues = viewModel.Leagues.OrderByDescending(l => favoriteLeagues.Contains(l.League.Id)).ToList();
            return View(viewModel);
        }

        //FOR TESTING VVV
        //public IActionResult Index()
        //{
        //    //var userId = "mocked-user-id"; // Fake user ID
        //    var totalCurrency = 1000; // Mocked currency value
        //    ViewBag.TotalCurrency = totalCurrency;

        //    // Mocked API response for leagues
        //    var leagues = new List<AllLeagues>
        //    {
        //        new AllLeagues
        //        {
        //            League = new LeagueInfo
        //            {
        //                Id = 39,
        //                Name = "Premier League",
        //                Logo = "https://example.com/premier-league-logo.png",
        //                Type = "League"
        //            },
        //            Country = new CountryInfo()
        //            {
        //                Name = "England",
        //                Code = "ENG",
        //                Flag = "https://example.com/england-flag.png"
        //            }
        //        },
        //        new AllLeagues
        //        {
        //            League = new LeagueInfo
        //            {
        //                Id = 140,
        //                Name = "La Liga",
        //                Logo = "https://example.com/la-liga-logo.png",
        //                Type = "League"
        //            },
        //            Country = new CountryInfo()
        //            { 
        //                Name = "Spain",
        //                Code = "ESP",
        //                Flag = "https://example.com/spain-flag.png"
        //            }
        //        }
        //    };


        // Mocked API response for today's fixtures
        //    var fixtures = new List<Fixture>
        //    {
        //        new Fixture
        //        {
        //            Id = 1001,
        //            Timestamp = 1710000000,
        //            Date = System.DateTime.UtcNow,
        //            League = new LeagueInfo { Id = 39, Name = "Premier League" },
        //            Teams = new Teams
        //            {
        //                Home = new TeamInfo { Id = 1, Name = "Manchester City", Logo = "https://example.com/city.png" },
        //                Away = new TeamInfo { Id = 2, Name = "Liverpool", Logo = "https://example.com/liverpool.png" }
        //            },
        //            Status = new MatchStatus { Long = "Match Finished", Short = "FT", Elapsed = 90 }
        //        },
        //        new Fixture
        //        {
        //            Id = 1002,
        //            Timestamp = 1710000001,
        //            Date = System.DateTime.UtcNow,
        //            League = new LeagueInfo { Id = 140, Name = "La Liga" },
        //            Teams = new Teams
        //            {
        //                Home = new TeamInfo { Id = 3, Name = "Real Madrid", Logo = "https://example.com/madrid.png" },
        //                Away = new TeamInfo { Id = 4, Name = "Barcelona", Logo = "https://example.com/barcelona.png" }
        //            },
        //            Status = new MatchStatus { Long = "Match In Progress", Short = "1H", Elapsed = 45 },
        //            Goals = new Goals { Home = 1, Away = 0 }
        //        }
        //    };

        //    // Mocked favorite leagues for user
        //    var favoriteLeagues = new HashSet<int> { 39 };

        //    var fixturesGroupedByLeague = fixtures
        //        .GroupBy(f => f.League.Id)
        //        .ToDictionary(g => g.Key, g => g.ToList());

        //    var viewModel = new LeagueMatchesViewModel
        //    {
        //        Leagues = leagues,
        //        FixturesGroupedByLeague = fixturesGroupedByLeague,
        //        FavoriteLeagues = favoriteLeagues
        //    };

        //    viewModel.Leagues = viewModel.Leagues
        //        .OrderByDescending(l => favoriteLeagues.Contains(l.League.Id))
        //        .ToList();

        //    return View(viewModel);
        //}
        //FOR TESTING ^^^

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

        public IActionResult Privacy()
        {
            return View();
        }
    }
}