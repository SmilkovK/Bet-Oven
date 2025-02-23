// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SportDomain.Identity;
using SportRepository;

namespace Bet_Oven.Areas.Identity.Pages.Account.Manage
{
    public class PersonalDataModel : PageModel
    {
        private readonly UserManager<BetUser> _userManager;
        private readonly ILogger<PersonalDataModel> _logger;
        private readonly ApplicationDbContext _context;

        public PersonalDataModel(
            UserManager<BetUser> userManager,
            ILogger<PersonalDataModel> logger,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var totalCurrency = _context.Currencies
                                     .Where(t => t.BetUserId == userId)
                                     .Sum(t => t.currencyAmount);

            ViewData["TotalCurrency"] = totalCurrency;

            return Page();
        }
    }
}
