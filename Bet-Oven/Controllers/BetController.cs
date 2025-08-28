using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportDomain.Identity;
using SportDomain.models;
using SportRepository;

namespace Bet_Oven.Controllers
{
    [Authorize]
    public class BetController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BetUser> _userManager;

        public BetController(ApplicationDbContext context, UserManager<BetUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> MyBets()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var betConfirms = await _context.BetConfirms
                .Where(bc => bc.UserId == user.Id)
                .Include(bc => bc.Bets)
                .OrderByDescending(bc => bc.PlacedAt)
                .ToListAsync();

            var balance = await _context.Currencies
                                        .Where(c => c.BetUserId == user.Id && c.IsBalanceRecord)
                                        .Select(c => (float?)c.CurrencyAmount)
                                        .FirstOrDefaultAsync() ?? 0;

            ViewBag.CurrentBalance = balance;
            return View(betConfirms);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceBet([FromBody] PlaceBet request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "User not found." });

            var currency = await _context.Currencies
                .FirstOrDefaultAsync(c => c.BetUserId == user.Id && c.IsBalanceRecord);

            if (currency == null)
                return Json(new { success = false, message = "Balance not found." });

            if (request.TotalStake > currency.CurrencyAmount)
                return Json(new { success = false, message = "Insufficient balance." });

            currency.CurrencyAmount -= request.TotalStake;

            var lossRecord = new VirtualCurrency
            {
                BetUserId = user.Id,
                CurrencyAmount = -request.TotalStake,
                CreatedAt = DateTime.UtcNow,
                IsBalanceRecord = false
            };
            _context.Currencies.Add(lossRecord);

            var betConfirm = new BetConfirm
            {
                UserId = user.Id,
                PlacedAt = DateTime.UtcNow,
                Bets = new List<UserBet>()
            };

            var stakePerBet = request.TotalStake / request.Bets.Count;

            foreach (var betDto in request.Bets)
            {
                betConfirm.Bets.Add(new UserBet
                {
                    UserId = user.Id,
                    HomeTeam = betDto.HomeTeam,
                    AwayTeam = betDto.AwayTeam,
                    BetType = betDto.Type,
                    Odds = betDto.Odds,
                    Stake = stakePerBet,
                    PotentialWin = stakePerBet * betDto.Odds,
                    PlacedAt = DateTime.UtcNow
                });
            }

            _context.BetConfirms.Add(betConfirm);
            await _context.SaveChangesAsync();

            return Json(new { success = true, newBalance = currency.CurrencyAmount });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCurrencyFromView(float amount)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(CurrencyHistory));
            }

            if (amount <= 0)
            {
                TempData["Error"] = "Invalid amount.";
                return RedirectToAction(nameof(CurrencyHistory));
            }

            if (amount > 100)
            {
                TempData["Error"] = "Max 100 per request.";
                return RedirectToAction(nameof(CurrencyHistory));
            }

            var todayTotal = await _context.Currencies
                .Where(c => c.BetUserId == user.Id && c.CreatedAt.Date == DateTime.UtcNow.Date && !c.IsBalanceRecord)
                .SumAsync(c => (float?)c.CurrencyAmount) ?? 0;

            if (todayTotal + amount > 100)
            {
                TempData["Error"] = "Daily limit of 100 credits reached.";
                return RedirectToAction(nameof(CurrencyHistory));
            }

            var balanceRecord = await _context.Currencies
                .FirstOrDefaultAsync(c => c.BetUserId == user.Id && c.IsBalanceRecord);

            if (balanceRecord == null)
            {
                balanceRecord = new VirtualCurrency
                {
                    BetUserId = user.Id,
                    CurrencyAmount = amount,
                    IsBalanceRecord = true,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Currencies.Add(balanceRecord);
            }
            else
            {
                balanceRecord.CurrencyAmount += amount;
            }

            var historyRecord = new VirtualCurrency
            {
                BetUserId = user.Id,
                CurrencyAmount = amount,
                CreatedAt = DateTime.UtcNow,
                IsBalanceRecord = false
            };
            _context.Currencies.Add(historyRecord);

            await _context.SaveChangesAsync();

            TempData["Success"] = $"Successfully added {amount} credits!";
            return RedirectToAction(nameof(CurrencyHistory));
        }

        [HttpGet]
        public async Task<IActionResult> CurrencyHistory()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var balanceRecord = await _context.Currencies
                .Where(c => c.BetUserId == user.Id && c.IsBalanceRecord)
                .FirstOrDefaultAsync();

            var currentBalance = balanceRecord?.CurrencyAmount ?? 0;

            var transactions = await _context.Currencies
                .Where(c => c.BetUserId == user.Id && !c.IsBalanceRecord)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            ViewBag.CurrentBalance = currentBalance;

            return View(transactions);
        }
    }
}
