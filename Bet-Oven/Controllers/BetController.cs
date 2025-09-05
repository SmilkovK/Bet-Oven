using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportDomain.DTO;
using SportDomain.Identity;
using SportDomain.models;
using SportRepository;
using SportService.Implementation;
using System.Diagnostics;

namespace Bet_Oven.Controllers
{
    [Authorize]
    public class BetController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BetUser> _userManager;
        private readonly FootballApiService _footballApiService;

        public BetController(ApplicationDbContext context, UserManager<BetUser> userManager, FootballApiService footballApiService)
        {
            _context = context;
            _userManager = userManager;
            _footballApiService = footballApiService;
        }

        [HttpGet]
        public async Task<IActionResult> MyBets()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            await UpdateBetResults(user.Id);

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

            float combinedOdds = 1;
            foreach (var betDto in request.Bets)
                combinedOdds *= betDto.Odds;

            var betConfirm = new BetConfirm
            {
                UserId = user.Id,
                PlacedAt = DateTime.UtcNow,
                CombinedPotentialWin = request.TotalStake * combinedOdds,
                Bets = request.Bets.Select(betDto => new UserBet
                {
                    UserId = user.Id,
                    HomeTeam = betDto.HomeTeam,
                    AwayTeam = betDto.AwayTeam,
                    BetType = betDto.Type,
                    Odds = betDto.Odds,
                    Stake = request.TotalStake,
                    PotentialWin = request.TotalStake * combinedOdds,
                    PlacedAt = DateTime.UtcNow,
                    FixtureId = betDto.FixtureId
                }).ToList()
            };

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

            var todayAdded = await _context.Currencies
                .Where(c => c.BetUserId == user.Id && !c.IsBalanceRecord && c.CreatedAt.Date == DateTime.UtcNow.Date && c.CurrencyAmount > 0)
                .SumAsync(c => (float?)c.CurrencyAmount) ?? 0;

            float remainingToday = 100 - todayAdded;

            if (remainingToday <= 0)
            {
                TempData["Error"] = $"Daily limit of 100 credits reached. You cannot add more today.";
                return RedirectToAction(nameof(CurrencyHistory));
            }

            if (amount > remainingToday)
            {
                TempData["Error"] = $"Daily limit of 100 credits reached. You can add up to {remainingToday:F2} more today.";
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

            TempData["Success"] = $"Successfully added {amount:F2} credits!";
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
        public async Task UpdateBetResults(string userId)
        {
            var betConfirms = await _context.BetConfirms
                .Include(bc => bc.Bets)
                .Where(bc => bc.UserId == userId)
                .ToListAsync();

            var fixtureIds = betConfirms
                .SelectMany(bc => bc.Bets)
                .Where(b => b.FixtureId.HasValue)
                .Select(b => b.FixtureId.Value)
                .Distinct()
                .ToList();

            var liveMatches = await _footballApiService.GetLiveMatches();
            var todayMatches = await _footballApiService.GetFixturesByDate(DateTime.UtcNow);

            var fixtureLookup = liveMatches
                .Concat(todayMatches)
                .Where(f => fixtureIds.Contains(f.Id))
                .GroupBy(f => f.Id)
                .ToDictionary(g => g.Key, g => g.First());

            foreach (var confirm in betConfirms)
            {
                bool allFinished = true;
                bool allWon = true;

                foreach (var bet in confirm.Bets)
                {
                    if (!bet.FixtureId.HasValue) continue;

                    fixtureLookup.TryGetValue(bet.FixtureId.Value, out var fixture);
                    if (fixture == null) continue;

                    bet.HomeGoals = fixture.Goals?.Home ?? 0;
                    bet.AwayGoals = fixture.Goals?.Away ?? 0;

                    string statusShort = fixture.Status?.Short ?? "NS";

                    switch (statusShort)
                    {
                        case "NS":
                        case "TBD":
                            bet.Status = "Pending";
                            bet.DisplayScore = "Match has not started";
                            allFinished = false;
                            allWon = false;
                            break;

                        case "1H":
                        case "2H":
                        case "HT":
                        case "ET":
                        case "BT":
                        case "LIVE":
                            bet.Status = "Live";
                            bet.DisplayScore = $"{bet.HomeGoals} : {bet.AwayGoals} (Live)";
                            allFinished = false;
                            allWon = false;
                            break;

                        case "FT":
                        case "AET":
                        case "PEN":
                            bool isDraw = bet.HomeGoals == bet.AwayGoals;
                            bool isHomeWin = bet.HomeGoals > bet.AwayGoals;
                            bool isAwayWin = bet.AwayGoals > bet.HomeGoals;

                            bet.Status = bet.BetType.ToLower() switch
                            {
                                "draw" => isDraw ? "Won" : "Lost",
                                "home" => isHomeWin ? "Won" : "Lost",
                                "away" => isAwayWin ? "Won" : "Lost",
                                _ => "Lost"
                            };

                            bet.DisplayScore = $"{bet.HomeGoals} : {bet.AwayGoals} (Finished)";
                            if (bet.Status == "Lost") allWon = false;
                            break;

                        default:
                            bet.Status = "Pending";
                            bet.DisplayScore = "Match has not started";
                            allFinished = false;
                            allWon = false;
                            break;
                    }
                }

                if (allFinished && !confirm.IsPaidOut)
                {
                    var balanceRecord = await _context.Currencies
                        .FirstOrDefaultAsync(c => c.BetUserId == userId && c.IsBalanceRecord);

                    if (balanceRecord != null)
                    {
                        if (allWon)
                        {
                            float payout = confirm.CombinedPotentialWin;
                            balanceRecord.CurrencyAmount += payout;

                            _context.Currencies.Add(new VirtualCurrency
                            {
                                BetUserId = userId,
                                CurrencyAmount = payout,
                                CreatedAt = DateTime.UtcNow,
                                IsBalanceRecord = false
                            });

                            foreach (var bet in confirm.Bets)
                                bet.IsPaid = true;

                            confirm.Status = "Won";
                        }
                        else
                        {
                            confirm.Status = "Lost";
                        }
                        confirm.IsPaidOut = true;
                    }
                }
                else if (!allFinished)
                {
                    confirm.Status = confirm.Bets.Any(b => b.Status == "Live") ? "Live" : "Pending";
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
