using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCManukauTech.Models.DB;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;



namespace MVCManukauTech.Controllers
{
    public class ProductsController : Controller
    {
        private readonly XSpy4CoreContext _context;

        public ProductsController(XSpy4CoreContext context)
        {
            _context = context;
        }
     
        public IActionResult Index(int pageno)
        {
            int totalrow = 6;
            int procount = 0;
            HttpContext.Session.SetInt32("PCountIND", 0);
            int endrow = (pageno * totalrow); // cal end row number
            int startrow = (endrow - totalrow) + 1;  // cal start row number
            string sql1 = "select * from products";
            string sql = "SELECT  * FROM(SELECT ROW_NUMBER() OVER(ORDER BY Products.Name) AS RowNum, * FROM Products) AS RowConstrainedResult WHERE RowNum >= @p0 AND RowNum <= @p1 ORDER BY RowNum";
            List<Products> prd = _context.Products.FromSql(sql1).ToList();
            List<Products> products = _context.Products.FromSql(sql, startrow, endrow).ToList();
            procount = prd.Count();
            HttpContext.Session.SetInt32("PCountIND", procount);
            return View(products);
        }
        //Piyush Kapur
        public IActionResult EachProductDetails(int id, int pageno)
        {
            int totalrow = 6;
            int procount = 0;
            HttpContext.Session.SetInt32("PCount", 0);
            HttpContext.Session.SetInt32("CtId", 0);
            int endrow = (pageno * totalrow); // cal end row number
            int startrow = (endrow - totalrow) + 1;  // cal start row number
            string sql1 = "select * from products where categoryid=" + id;
            string sql = "SELECT  * " +
                " FROM(SELECT    ROW_NUMBER() OVER(ORDER BY CategoryId) AS RowNum, * " +
                " FROM      Products where CategoryId=" + id +
                " ) AS RowConstrainedResult" +
                " WHERE RowNum >= @p0" +
                " AND RowNum <= @p1" +
                " ORDER BY RowNum";
            List<Products> prd = _context.Products.FromSql(sql1).ToList();
            List<Products> products = _context.Products.FromSql(sql, startrow, endrow).ToList();
            procount = prd.Count();
            HttpContext.Session.SetInt32("PCount", procount);
            HttpContext.Session.SetInt32("CtId", id);
            return View(products);

        }
       
        //Piyush Kapur
        public ActionResult Add(string pro,string name)
        {
            
            if (HttpContext.User.Identity.IsAuthenticated==true)
            {
                List<string> li = new List<string>();
                HttpContext.Session.Set("wishlist", null);
                var userid = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
                li.Add(pro);
                for(var i=0;i<li.Count;i++)
                {
                    string sql = "insert into TempCart(ProductId,ProductName,UserId) values('"+ pro +"','"+ name +"','"+ userid +"')";
                    _context.Database.ExecuteSqlCommand(sql);

                }
                string wish = li.Count.ToString();
                HttpContext.Session.SetString("wishlist",wish);
            }
            else
            {
                return RedirectToAction("Login", "Account");

            }
            return RedirectToAction("Index", "Home");


        }
        // GET: Products/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .Include(p => p.Category)
                .SingleOrDefaultAsync(m => m.ProductId == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // GET: Products/Create
        [Authorize(Roles ="Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,CategoryId,Name,ImageFileName,UnitCost,Description,IsDownload,DownloadFileName")] Products products)
        {
            if (ModelState.IsValid)
            {
                _context.Add(products);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", products.CategoryId);
            return View(products);
        }
        [Authorize(Roles ="Admin")]
        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products.SingleOrDefaultAsync(m => m.ProductId == id);
            if (products == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", products.CategoryId);
            return View(products);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ProductId,CategoryId,Name,ImageFileName,UnitCost,Description,IsDownload,DownloadFileName")] Products products)
        {
            if (id != products.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(products);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsExists(products.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", new { pageno = 1 });
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", products.CategoryId);
            return View(products);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Products
                .Include(p => p.Category)
                .SingleOrDefaultAsync(m => m.ProductId == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var products = await _context.Products.SingleOrDefaultAsync(m => m.ProductId == id);
            _context.Products.Remove(products);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductsExists(string id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
