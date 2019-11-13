using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCManukauTech.Models;
using MVCManukauTech.Models.DB;
using MVCManukauTech.ViewModels;


namespace MVCManukauTech.Controllers
{
    public class HomeController : Controller
    {
        private readonly XSpy4CoreContext _context;

        public HomeController(XSpy4CoreContext context)
        {
            _context = context;
        }

        
        public IActionResult Index()
        {
            
            // Jaykumar solanki 04-09-2018
            // Redirect to Premium page if user expired his/her membership
            if (HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) != null)
            {
                string IsPremiumMembeSql = "select * from Premium where UserId = @p0";
                string UserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                List<Premium> IsPremiumMember = _context.Premium.FromSql(IsPremiumMembeSql, UserId).ToList();
                
                if (IsPremiumMember.Count() > 0)
                {
                    string PremiumSql = "select * from Premium where DateOfExpire > GETDATE() and IsActive like 1 and UserId = @p0";
                    List<Premium> Premium = _context.Premium.FromSql(PremiumSql, HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)).ToList();

                    if (Premium.Count() > 0)
                    {
                        //manage session
                        HttpContext.Session.SetInt32("IsVisible", 0);
                        HttpContext.Session.SetInt32("PremiumType", Premium.First().PremiumTypeId);
                    }
                    else
                    {
                        HttpContext.Session.SetInt32("PremiumType", 0);
                        TempData["msg"] = "<script>alert('Your membership is expired');</script>";
                        if (HttpContext.Session.GetString("IsPremiumRedirect") != "No")
                        {
                            return RedirectToAction("Index", "Premiums", new { area = "" });
                        }
                    }
                }
                //Piyush Kapur
                else
                {
                    HttpContext.Session.SetInt32("PremiumType", 0);
                }
            } 
            
            //Piyush Kapur
            //For showing Category Listing on Home view
            string sql = "select * from Categories";
            List<Categories> categories = _context.Categories.FromSql(sql).ToList();
            return View(categories);

        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
