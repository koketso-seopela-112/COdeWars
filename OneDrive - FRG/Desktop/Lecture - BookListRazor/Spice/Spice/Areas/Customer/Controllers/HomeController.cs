using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;

namespace Spice.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext _db)
        {
            _logger = logger;
            db = _db;
        }

        public async Task<IActionResult> Index()
        {
            IndexViewModel IndexVM = new IndexViewModel()
            {
                MenuItem = await db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync(),
                Category = await db.Category.ToListAsync(),
                Coupon = await db.Coupon.Where(c => c.IsActive == true).ToListAsync()
            };
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);


            if(claim != null)
            {
                var cnt = db.ShoppingCart.Where(u => u.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.ShoppingCartCount, cnt);
            }
            return View(IndexVM);
        }


        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var menuItemFromDb = await db.MenuItem.Include(c => c.Category).Include(s => s.SubCategory).Where(m => m.Id == id).FirstOrDefaultAsync();
            ShoppingCart cardobj = new ShoppingCart()
            {
                MenuItem = menuItemFromDb,
                MenuItemId = menuItemFromDb.Id
            };

            return View(cardobj);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(ShoppingCart cardObject)
        {
            cardObject.Id = 0;

            if (ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)this.User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

                cardObject.ApplicationUserId = claim.Value;

                ShoppingCart cardFromDb = await db.ShoppingCart.Where(u => u.ApplicationUserId == cardObject.ApplicationUserId
                && u.MenuItemId == cardObject.MenuItemId).FirstOrDefaultAsync();

                if (cardFromDb == null)
                {
                    await db.ShoppingCart.AddAsync(cardObject);
                }
                else
                {
                    cardFromDb.Count = cardFromDb.Count + cardObject.Count;
                }

                await db.SaveChangesAsync();

                var count = db.ShoppingCart.Where(c => c.ApplicationUserId == cardObject.ApplicationUserId).ToList().Count();
                HttpContext.Session.SetInt32(SD.ShoppingCartCount, count);   
            }
            return RedirectToAction(nameof(Index));
        }

    }

}

