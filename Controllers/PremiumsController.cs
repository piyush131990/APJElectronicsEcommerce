using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCManukauTech.Models.DB;

// Made by Jaykumar solanki
namespace MVCManukauTech.Controllers
{
    public class PremiumsController : Controller
    {
        private readonly XSpy4CoreContext _context;

        public PremiumsController(XSpy4CoreContext context)
        {
            _context = context;
        }

        // Jaykumar solanki 28-08-2018
        // User will see the Premium page
        // GET: Premiums
        public IActionResult Index()
        {
            HttpContext.Session.SetString("IsPremiumRedirect", "No");
            string sql = "select * from PremiumTypes";
            var PremiumTypes = _context.PremiumTypes.FromSql(sql).ToList();
            return View(PremiumTypes);
        }

        // GET: Premiums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var premium = await _context.Premium
                .SingleOrDefaultAsync(m => m.PremiumId == id);
            if (premium == null)
            {
                return NotFound();
            }

            return View(premium);
        }

        // GET: Premiums/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Premiums/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PremiumId,UserId,DateOfBuy,DateOfExpire,IsActive")] Premium premium)
        {
            if (ModelState.IsValid)
            {
                _context.Add(premium);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(premium);
        }

        // GET: Premiums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var premium = await _context.Premium.SingleOrDefaultAsync(m => m.PremiumId == id);
            if (premium == null)
            {
                return NotFound();
            }
            return View(premium);
        }

        // POST: Premiums/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PremiumId,UserId,DateOfBuy,DateOfExpire,IsActive")] Premium premium)
        {
            if (id != premium.PremiumId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(premium);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PremiumExists(premium.PremiumId))
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
            return View(premium);
        }

        // GET: Premiums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var premium = await _context.Premium
                .SingleOrDefaultAsync(m => m.PremiumId == id);
            if (premium == null)
            {
                return NotFound();
            }

            return View(premium);
        }

        // POST: Premiums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var premium = await _context.Premium.SingleOrDefaultAsync(m => m.PremiumId == id);
            _context.Premium.Remove(premium);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PremiumExists(int id)
        {
            return _context.Premium.Any(e => e.PremiumId == id);
        }
    }
}
