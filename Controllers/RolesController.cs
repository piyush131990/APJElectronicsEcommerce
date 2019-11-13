using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCManukauTech.Models.DB;

namespace MVCManukauTech.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class RolesController : Controller
    {
        private readonly XSpy4CoreContext _context;

        public RolesController(XSpy4CoreContext context)
        {
            _context = context;
        }

        // GET: AspNetRoles
        public async Task<IActionResult> Index()
        {
            return View(await _context.AspNetRoles.ToListAsync());
        }

        // GET: AspNetRoles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aspNetRoles = await _context.AspNetRoles
                .SingleOrDefaultAsync(m => m.Id == id);
            if (aspNetRoles == null)
            {
                return NotFound();
            }

            return View(aspNetRoles);
        }

        // GET: AspNetRoles/Create
        public IActionResult Create()
        {
            return View();
        }

        public string PutRoleData(string RoleId, string RoleName)
        {
            // ETM Check if Role Assignment already exists
            string sqlCheck = @"SELECT Id as RoleId, Name AS RoleName FROM AspNetRoles WHERE AspNetRoles.Id = @p0 OR AspNetRoles.Name = @p1";
            var results = _context.RoleViewModels.FromSql(sqlCheck, RoleId, RoleName).ToList();
            if (results.Count() != 0) // 0 - if not exists
            {
                return "ERROR";
            }
            else
            {
                string sql = "INSERT INTO AspNetRoles VALUES (@p0,  @p1, @p2, @p3)";
                string normalized = RoleName.ToUpper();
                int rowsChanged = _context.Database.ExecuteSqlCommand(sql, RoleId, "", RoleName, normalized);
                return "SUCCESS";
            }

        }

        // POST: AspNetRoles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Name")] AspNetRoles aspNetRoles)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(aspNetRoles);
        //        await _context.SaveChangesAsync();
        //        return Redirect(Url.Content("~/RoleManagement/"));
        //    }
        //    return View(aspNetRoles);
        //}

        // GET: AspNetRoles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var aspNetRoles = await _context.AspNetRoles.SingleOrDefaultAsync(m => m.Id == id);
            if (aspNetRoles == null)
            {
                return NotFound();
            }
            return View(aspNetRoles);
        }

        // POST: AspNetRoles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,ConcurrencyStamp,Name,NormalizedName")] AspNetRoles aspNetRoles)
        {
            if (id != aspNetRoles.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(aspNetRoles);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AspNetRolesExists(aspNetRoles.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Redirect(Url.Content("~/RoleManagement/"));
            }
            return View(aspNetRoles);
        }

        public string Delete(string RoleId)
        {
            string sql = @"DELETE FROM AspNetRoles WHERE AspNetRoles.Id=@p0";
            int rowsChanged = _context.Database.ExecuteSqlCommand(sql, RoleId);
            if (rowsChanged != 0)
            {
                return "SUCCESS";
            }
            else return "ERROR";
        }
        // GET: AspNetRoles/Delete/5
        //public async Task<IActionResult> Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var aspNetRoles = await _context.AspNetRoles
        //        .SingleOrDefaultAsync(m => m.Id == id);
        //    if (aspNetRoles == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(aspNetRoles);
        //}

        // POST: AspNetRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var aspNetRoles = await _context.AspNetRoles.SingleOrDefaultAsync(m => m.Id == id);
            _context.AspNetRoles.Remove(aspNetRoles);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AspNetRolesExists(string id)
        {
            return _context.AspNetRoles.Any(e => e.Id == id);
        }
    }
}
