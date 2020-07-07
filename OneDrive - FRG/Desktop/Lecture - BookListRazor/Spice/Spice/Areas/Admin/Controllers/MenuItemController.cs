﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    
    [Area("Admin")]
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly IWebHostEnvironment hostingEnvironment;

        [BindProperty]
        public MenuItemViewModel MenuItemVM { get; set; }
        public MenuItemController(ApplicationDbContext _db, IWebHostEnvironment _hostingEnvironment)
        {
            db = _db;
            hostingEnvironment = _hostingEnvironment;
            MenuItemVM = new MenuItemViewModel()
            {
                Category = db.Category,
                MenuItem = new MenuItem()

            };
        }
        public async Task<IActionResult> Index()
        {
            var menuItems = await db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).ToListAsync();
            return View(menuItems);
        }

        //GET -CREATE
        public IActionResult Create()
        {
            return View(MenuItemVM);
        }

        //POST - CREATE
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CreatePOST()
        {
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"]);
            if (!ModelState.IsValid)
            {
                return View(MenuItemVM);
            }

            db.MenuItem.Add(MenuItemVM.MenuItem);
            await db.SaveChangesAsync();

            //working on the image saving section

            string webRootPath = hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = await db.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);

            if (files.Count() > 0)
            {
                //files has been uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(files[0].FileName);

                using (var filesStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension;
            }
            else
            {
                //No file was uploaded, so use default
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultFoodImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + MenuItemVM.MenuItem.Id + ".png");
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + ".png";
            }

            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }


        //GET -EDIT
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);
            MenuItemVM.SubCategory = await db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }
            return View(MenuItemVM);
        }

        //POST - Edit
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> EditPOST(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            MenuItemVM.MenuItem.SubCategoryId = Convert.ToInt32(Request.Form["SubCategoryId"]);

            if (!ModelState.IsValid)
            {
                MenuItemVM.SubCategory = await db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();
                return View(MenuItemVM);
            }

            //working on the image saving section

            string webRootPath = hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;

            var menuItemFromDb = await db.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);

            if (files.Count() > 0)
            {
                //New Image has been uploaded
                var uploads = Path.Combine(webRootPath, "images");
                var extension_new = Path.GetExtension(files[0].FileName);

                //Delete the original File
                var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                //we will upload the new file and save it to the database
                using (var filesStream = new FileStream(Path.Combine(uploads, MenuItemVM.MenuItem.Id + extension_new), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }
                menuItemFromDb.Image = @"\images\" + MenuItemVM.MenuItem.Id + extension_new;
            }

            menuItemFromDb.Name = MenuItemVM.MenuItem.Name;
            menuItemFromDb.Description = MenuItemVM.MenuItem.Description;
            menuItemFromDb.Price = MenuItemVM.MenuItem.Price;
            menuItemFromDb.Spicyness = MenuItemVM.MenuItem.Spicyness;
            menuItemFromDb.CategoryId = MenuItemVM.MenuItem.CategoryId;
            menuItemFromDb.SubCategoryId = MenuItemVM.MenuItem.SubCategoryId;

            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        //GET -DETAILS
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);
            MenuItemVM.SubCategory = await db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }
            return View(MenuItemVM);
        }




        //GET -DELETE
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            MenuItemVM.MenuItem = await db.MenuItem.Include(m => m.Category).Include(m => m.SubCategory).SingleOrDefaultAsync(m => m.Id == id);
            MenuItemVM.SubCategory = await db.SubCategory.Where(s => s.CategoryId == MenuItemVM.MenuItem.CategoryId).ToListAsync();

            if (MenuItemVM.MenuItem == null)
            {
                return NotFound();
            }
            return View(MenuItemVM);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var menuItem = await db.MenuItem.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            string webRootPath = hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var menuItemFromDb = await db.MenuItem.FindAsync(MenuItemVM.MenuItem.Id);
            var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));

            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            db.MenuItem.Remove(menuItem);
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }



    }
}