using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportDomain.Identity;
using SportDomain.models;
using SportRepository;
using System.Threading.Tasks;

namespace Bet_Oven.Controllers
{
    [Authorize]
    public class RewardsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BetUser> _userManager;

        private const float REWARD_1_THRESHOLD = 10000f;
        private const float REWARD_2_THRESHOLD = 50000f;
        private const float DEFAULT_DAILY_LIMIT = 100f;
        private const float REWARD_1_DAILY_LIMIT = 1000f;

        public RewardsController(ApplicationDbContext context, UserManager<BetUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Account");

            var balance = await _context.Currencies
                .Where(c => c.BetUserId == user.Id && c.IsBalanceRecord)
                .Select(c => (float?)c.CurrencyAmount)
                .FirstOrDefaultAsync() ?? 0;

            var rewards = new RewardsViewModel
            {
                CurrentBalance = balance,
                Reward1Threshold = REWARD_1_THRESHOLD,
                Reward2Threshold = REWARD_2_THRESHOLD,
                DefaultDailyLimit = DEFAULT_DAILY_LIMIT,
                Reward1DailyLimit = REWARD_1_DAILY_LIMIT,
                HasReward1 = balance >= REWARD_1_THRESHOLD,
                HasReward2 = balance >= REWARD_2_THRESHOLD,
                Reward1Description = $"Daily currency addition limit increased from {DEFAULT_DAILY_LIMIT} to {REWARD_1_DAILY_LIMIT}",
                Reward2Description = "No maximum bet limit (you can bet any amount)"
            };

            return View(rewards);
        }

        [HttpGet]
        public async Task<IActionResult> GetRewardStatus()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "User not found" });

            var balance = await _context.Currencies
                .Where(c => c.BetUserId == user.Id && c.IsBalanceRecord)
                .Select(c => (float?)c.CurrencyAmount)
                .FirstOrDefaultAsync() ?? 0;

            return Json(new
            {
                success = true,
                hasReward1 = balance >= REWARD_1_THRESHOLD,
                hasReward2 = balance >= REWARD_2_THRESHOLD,
                currentBalance = balance,
                reward1Threshold = REWARD_1_THRESHOLD,
                reward2Threshold = REWARD_2_THRESHOLD
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetBettingLimits()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "User not found" });

            var balance = await _context.Currencies
                .Where(c => c.BetUserId == user.Id && c.IsBalanceRecord)
                .Select(c => (float?)c.CurrencyAmount)
                .FirstOrDefaultAsync() ?? 0;

            var maxBetAmount = balance >= REWARD_2_THRESHOLD ? float.MaxValue : 100f;

            return Json(new
            {
                success = true,
                minBetAmount = 20f,
                maxBetAmount = maxBetAmount,
                hasNoBetLimit = balance >= REWARD_2_THRESHOLD
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetDailyAdditionLimit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Json(new { success = false, message = "User not found" });

            var balance = await _context.Currencies
                .Where(c => c.BetUserId == user.Id && c.IsBalanceRecord)
                .Select(c => (float?)c.CurrencyAmount)
                .FirstOrDefaultAsync() ?? 0;

            var dailyLimit = balance >= REWARD_1_THRESHOLD ? REWARD_1_DAILY_LIMIT : DEFAULT_DAILY_LIMIT;

            var todayAdded = await _context.Currencies
                .Where(c => c.BetUserId == user.Id &&
                           !c.IsBalanceRecord &&
                           c.CreatedAt.Date == DateTime.UtcNow.Date &&
                           c.CurrencyAmount > 0)
                .SumAsync(c => (float?)c.CurrencyAmount) ?? 0;

            var remainingToday = dailyLimit - todayAdded;

            return Json(new
            {
                success = true,
                dailyLimit = dailyLimit,
                todayAdded = todayAdded,
                remainingToday = remainingToday,
                hasIncreasedDailyLimit = balance >= REWARD_1_THRESHOLD
            });
        }
    }
}