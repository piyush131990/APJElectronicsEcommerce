using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCManukauTech.Models.DB;
using Newtonsoft.Json;

using System.ComponentModel.DataAnnotations;

namespace MVCManukauTech.Controllers
{
    // 20180409 ETM Turn cache off
    [ResponseCache(Location =ResponseCacheLocation.None, NoStore =true)]
    public class RoleManagementController : Controller
    {
        private readonly XSpy4CoreContext _context;

        public RoleManagementController(XSpy4CoreContext context)
        {
            _context = context;
        }
        

        // 20180330 ETM Index View
        public IActionResult Index()
        {
            return View();
        }       

        public IActionResult Create()
        {
            return View();
        }

        public string GetUserData()
        {
            string sql = @"SELECT AspNetUsers.Id as UserId, AspNetUsers.UserName, AspNetUsers.Email AS UserEmail, ISNULL(PhoneNumber,'0')as PhoneNumber
            FROM AspNetUsers";

            var users = _context.UserViewModels.FromSql(sql);
            string json = JsonConvert.SerializeObject(users);
            return json;
        }

        public string GetRoleData()
        {
            string sql = @"SELECT AspNetRoles.Id AS RoleId, AspNetRoles.Name As RoleName
            FROM AspNetRoles";

            var roles = _context.RoleViewModels.FromSql(sql);
            string json = JsonConvert.SerializeObject(roles);
            return json;

        }

        // 20180406 ETM To get selected ids from Edit and Delete
        public string GetUserId(string id)
        {
            return id;
        }

        public string GetRoleId(string id)
        {
            return id;

        }

        public string GetRoleAssignment()
        {
            string sql = @"SELECT  ISNULL(r.Id,'0') AS RoleId, ISNULL(r.Name,' ') AS RoleName, ISNULL(u.Id,'0') AS UserId, ISNULL(u.UserName,' ')  as  UserName
                FROM AspNetUsers u
                LEFT JOIN AspNetUserRoles ur ON  ur.UserId = u.Id 
                LEFT JOIN AspNetRoles r ON r.Id = ur.RoleId
                WHERE r.Id IS NOT NULL";

            var userroles = _context.UserRoleViewModels.FromSql(sql);
            string json = JsonConvert.SerializeObject(userroles);
            return json;
        }

        public IActionResult Edit(string  UserId, string UserName, string RoleId, string RoleName)
        {
            ViewData["UserId"] = UserId;
            ViewData["RoleId"] = RoleId;
            ViewData["UserName"] = UserName;
            ViewData["RoleName"] = RoleName;
            string sql = "SELECT * from AspNetRoles ar inner join AspNetUserRoles aur on ar.Id=aur.RoleId where UserId="+"'"+ UserId +"'";
            var aspNetRoles =_context.AspNetRoles.FromSql(sql).ToList();
            if (aspNetRoles == null)
            {
                return NotFound();
            }
            return View(aspNetRoles);
        }

        // 20180328 ETM Writing back to the database from Role Management - Create page
        public string PutUserRoleData(string UserId, string RoleId)
        {
            // ETM Check if Role Assignment already exists
            string sqlCheck = @"SELECT * FROM AspNetUserRoles WHERE AspNetUserRoles.UserId = @p0 AND AspNetUserRoles.RoleId = @p1";
            var results = _context.AspNetUserRoles.FromSql(sqlCheck, UserId, RoleId).ToList();
            if (results.Count() != 0) // 0 - if not exists
            {
                return "ERROR";
            }
            else
            {
                string sqlCheckuser = @"SELECT * FROM AspNetUserRoles WHERE AspNetUserRoles.UserId = @p0";
                var user = _context.AspNetUserRoles.FromSql(sqlCheckuser, UserId).ToList();
                if (user.Count() != 0) // 0 - if not exists
                {
                    return "DUPLICATE";
                }
                else
                {
                    string sql = "INSERT INTO AspNetUserRoles VALUES (@p0,  @p1)";
                    int rowsChanged = _context.Database.ExecuteSqlCommand(sql, UserId, RoleId);
                    return "SUCCESS";
                }
            }

        }
        // 20180409 ETM For Editing Role Management
        public string UpdateUserRoleData(string UserId, string RoleId)
        {
            // ETM Check if Role Assignment already exists
            string sqlCheck = @"SELECT * FROM AspNetUserRoles WHERE AspNetUserRoles.UserId = @p0 AND AspNetUserRoles.RoleId = @p1";
            var results = _context.AspNetUserRoles.FromSql(sqlCheck, UserId, RoleId).ToList();
            if (results.Count() != 0) // 0 - if not exists
            {
                return "ERROR";
            }
            else
            {
                
                string sql = @"UPDATE AspNetUserRoles SET RoleId=@p0
                            WHERE AspNetUserRoles.UserId=@p1";
                int rowsChanged = _context.Database.ExecuteSqlCommand(sql, RoleId, UserId);
                if (rowsChanged == 0)
                {
                    return "ERROR";
                }
                else
                    return "SUCCESS";
                
            }

        }


        public string Delete(string UserId, string RoleId)
        {
            string sql = @"DELETE FROM AspNetUserRoles WHERE AspNetUserRoles.UserId=@p0 AND AspNetUserRoles.RoleId=@p1";
            int rowsChanged = _context.Database.ExecuteSqlCommand(sql, UserId, RoleId);
            return "SUCCESS";
        }

        // POST: AspNetUserRoles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("UserId,RoleId")] AspNetUserRoles aspNetUserRoles)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(aspNetUserRoles);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["RoleId"] = new SelectList(_context.AspNetRoles, "Id", "Id", aspNetUserRoles.RoleId);
        //    ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", aspNetUserRoles.UserId);
        //    return View(aspNetUserRoles);
        //}

        // GET: AspNetUserRoles/Edit/5
        //public async Task<IActionResult> Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var aspNetUserRoles = await _context.AspNetUserRoles.SingleOrDefaultAsync(m => m.UserId == id);
        //    if (aspNetUserRoles == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["RoleId"] = new SelectList(_context.AspNetRoles, "Id", "Id", aspNetUserRoles.RoleId);
        //    ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", aspNetUserRoles.UserId);
        //    return View(aspNetUserRoles);
        //}

        //// POST: AspNetUserRoles/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(string id, [Bind("UserId,RoleId")] AspNetUserRoles aspNetUserRoles)
        //{
        //    if (id != aspNetUserRoles.UserId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(aspNetUserRoles);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!AspNetUserRolesExists(aspNetUserRoles.UserId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["RoleId"] = new SelectList(_context.AspNetRoles, "Id", "Id", aspNetUserRoles.RoleId);
        //    ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", aspNetUserRoles.UserId);
        //    return View(aspNetUserRoles);
        //}

        // GET: AspNetUserRoles/Delete/5
        //public async Task<IActionResult> Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var aspNetUserRoles = await _context.AspNetUserRoles
        //        .Include(a => a.Role)
        //        .Include(a => a.User)
        //        .SingleOrDefaultAsync(m => m.UserId == id);
        //    if (aspNetUserRoles == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(aspNetUserRoles);
        //}

        //// POST: AspNetUserRoles/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(string id)
        //{
        //    var aspNetUserRoles = await _context.AspNetUserRoles.SingleOrDefaultAsync(m => m.UserId == id);
        //    _context.AspNetUserRoles.Remove(aspNetUserRoles);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool AspNetUserRolesExists(string id)
        {
            return _context.AspNetUserRoles.Any(e => e.UserId == id);
        }
    }
}
