using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportRepository;
using System.Data.Entity;
using System.Diagnostics;
using System.Security.Claims;

namespace Bet_Oven.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
{
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var totalCurrency = _context.Currencies
                                 .Where(t => t.BetUserId == userId)
                                 .Sum(t => t.currencyAmount);

    ViewBag.TotalCurrency = totalCurrency;
    return View();
}


        public IActionResult Privacy()
        {
            return View();
        }
    }
}
