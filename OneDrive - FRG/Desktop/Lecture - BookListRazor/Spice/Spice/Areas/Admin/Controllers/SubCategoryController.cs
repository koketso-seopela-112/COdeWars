using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.ViewModels;
using Spice.Utility;

namespace Spice.Areas.Admin.Controllers
{
    
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext db;
        [TempData]
        public string SattusMessage { get; set; }

        public SubCategoryController(ApplicationDbContext _db)
        {
            db = _db;
        }

        //GET - INDEX
        public async Task<IActionResult> Index()
        {
            var subCategotries = await db.SubCategory.Include(s => s.Category).ToListAsync();
            return View(subCategotries);
        }

        //GET- CREATE
        public async Task<IActionResult> Create()
        {
            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await db.Category.ToListAsync(),
                SubCategory = new SubCategory(),
                SubCategoryList = await db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };

            return View(model);
        }

        //POST - CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCategoryAndCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var doesSubCategoryExist = db.SubCategory.Include(s => s.Category)
                    .Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);
                if (doesSubCategoryExist.Count() > 0)
                {
                    SattusMessage = "Error: SubCategory Exists uder " + doesSubCategoryExist.First().Category.Name + " category. please use another name.";

                }
                else
                {
                    db.SubCategory.Add(model.SubCategory);
                    await db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMessage = SattusMessage
            };
            return View(modelVM);
        }

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategories = new List<SubCategory>();

            subCategories = await (from subCategory in db.SubCategory
                                   where subCategory.CategoryId == id
                                   select subCategory).ToListAsync();
            return Json(new SelectList(subCategories, "Id", "Name"));
        }


        //GET- EDIT
        public async Task<IActionResult> Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var subCategory = await db.SubCategory.SingleOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };

            return View(model);
        }

        //POST - EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SubCategoryAndCategoryViewModel model)
        {

            if (ModelState.IsValid)
            {
                var doesSubCategoryExist = db.SubCategory.Include(s => s.Category)
                    .Where(s => s.Name == model.SubCategory.Name && s.Category.Id == model.SubCategory.CategoryId);
                if (doesSubCategoryExist.Count() > 0)
                {
                    //Error
                    SattusMessage = "Error: SubCategory Exists uder " + doesSubCategoryExist.First().Category.Name + " category. please use another name.";

                }
                else
                {
                    var subCatFromDb = await db.SubCategory.FindAsync(model.SubCategory.Id);
                    subCatFromDb.Name = model.SubCategory.Name;
                    await db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            SubCategoryAndCategoryViewModel modelVM = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await db.Category.ToListAsync(),
                SubCategory = model.SubCategory,
                SubCategoryList = await db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMessage = SattusMessage
            };  
            return View(modelVM);
        }


        //GET- DETAILS
        public async Task<IActionResult> Details(int? id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var subCategory = await db.SubCategory.SingleOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };

            return View(model);
        }
       

        //GET- DELETE
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var subCategory = await db.SubCategory.SingleOrDefaultAsync(m => m.Id == id);

            if (subCategory == null)
            {
                return NotFound();
            }

            SubCategoryAndCategoryViewModel model = new SubCategoryAndCategoryViewModel()
            {
                CategoryList = await db.Category.ToListAsync(),
                SubCategory = subCategory,
                SubCategoryList = await db.SubCategory.OrderBy(p => p.Name).Select(p => p.Name).Distinct().ToListAsync()
            };

            return View(model);
        }


        // POST - DELETE
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var subCategory = await db.SubCategory.FindAsync(id);
            if (subCategory == null)
            {
                return NotFound();
            }
            db.SubCategory.Remove(subCategory);
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}