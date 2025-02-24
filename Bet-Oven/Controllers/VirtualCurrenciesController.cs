using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SportDomain.models;
using SportRepository;

namespace Bet_Oven.Controllers
{
    [Authorize]
    public class VirtualCurrenciesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VirtualCurrenciesController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: VirtualCurrencies
        public async Task<IActionResult> Index()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var totalCurrency = await _context.Currencies
                 .Where( t => t.BetUserId == userId)
                .SumAsync(t => t.currencyAmount);

            ViewBag.TotalCurrency = totalCurrency;

            var applicationDbContext = _context.Currencies.Include(v => v.BetUser);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: VirtualCurrencies/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var virtualCurrency = await _context.Currencies
                .Include(v => v.BetUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (virtualCurrency == null)
            {
                return NotFound();
            }

            return View(virtualCurrency);
        }
        
        // GET: VirtualCurrencies/Create
        public IActionResult Create()
        {
            ViewData["BetUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: VirtualCurrencies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("currencyAmount,CreatedAt,BetUserId,Id")] VirtualCurrency virtualCurrency)
        {
            if (ModelState.IsValid)
            {
                var today = DateTime.Today;
                var tommorow = today.AddDays(1);
                var currencyCountToday = await _context.Currencies
                    .Where(v => v.CreatedAt >= today && v.CreatedAt < tommorow)
                    .CountAsync();

                if (currencyCountToday > 1000)
                {
                    ModelState.AddModelError(string.Empty, "You cannot add more than 1000 VirtualCurrency records per day.");
                    return View(virtualCurrency);
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                virtualCurrency.Id = Guid.NewGuid();
                virtualCurrency.CreatedAt = DateTime.Now;
                virtualCurrency.BetUserId= userId;
                _context.Add(virtualCurrency);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(virtualCurrency);
        }
        [Authorize(Roles ="Admin, Editor")]
        // GET: VirtualCurrencies/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var virtualCurrency = await _context.Currencies.FindAsync(id);
            if (virtualCurrency == null)
            {
                return NotFound();
            }
            ViewData["BetUserId"] = new SelectList(_context.Users, "Id", "Id", virtualCurrency.BetUserId);
            return View(virtualCurrency);
        }

        // POST: VirtualCurrencies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("currencyAmount,CreatedAt,BetUserId,Id")] VirtualCurrency virtualCurrency)
        {
            if (id != virtualCurrency.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(virtualCurrency);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VirtualCurrencyExists(virtualCurrency.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BetUserId"] = new SelectList(_context.Users, "Id", "Id", virtualCurrency.BetUserId);
            return View(virtualCurrency);
        }

        // GET: VirtualCurrencies/Delete/5
        [Authorize(Roles = "Admin, Editor")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var virtualCurrency = await _context.Currencies
                .Include(v => v.BetUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (virtualCurrency == null)
            {
                return NotFound();
            }

            return View(virtualCurrency);
        }

        // POST: VirtualCurrencies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var virtualCurrency = await _context.Currencies.FindAsync(id);
            if (virtualCurrency != null)
            {
                _context.Currencies.Remove(virtualCurrency);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VirtualCurrencyExists(Guid id)
        {
            return _context.Currencies.Any(e => e.Id == id);
        }
    }
}
