using CmsShoppingCart.Infrastructure;
using CmsShoppingCart.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CmsShoppingCart.Areas.Admin.Controllers
{
    [Authorize(Roles ="Admin")]
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly CmsShoppingCartContextt _context;
        public CategoriesController(CmsShoppingCartContextt context)
        {
            _context = context; 
        }
        public async Task<IActionResult> Index()
        {

            return View(await _context.categories.OrderBy(c => c.Sorting).ToListAsync());
        }

        //GET/Admin/categories/Create
        public async Task<IActionResult> Create() => View();




        //POST//Admin/Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                category.Slug = category.Name.ToLower().Replace(" ", "-");
                category.Sorting = 100;

                var cat = await _context.categories.FirstOrDefaultAsync(x => x.Slug == category.Slug);
                if (cat != null)
                {
                    ModelState.AddModelError(" ", "The category already exists");
                    return View(category);
                }
                _context.Add(category);
                await _context.SaveChangesAsync();
                TempData["Success"] = "The category has been added ";
                return RedirectToAction(nameof(Index));
                
            }
            return View(category);
        }

        //GET/Admin/Categories/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            Category category = await _context.categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        //POST/Admin/Categories/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category, int? id)
        {
            if (ModelState.IsValid)
            {
                category.Slug =  category.Name.ToLower().Replace(" ", "-");

                var cat = await _context.categories.Where(x => x.Id != id).FirstOrDefaultAsync(x => x.Slug == category.Slug);
                if (cat != null)
                {
                    ModelState.AddModelError("", "The category already exist");
                    return View();
                }

                _context.Update(category);
                await _context.SaveChangesAsync();

                TempData["Success"] = "The category has been Edited";
                return RedirectToAction(nameof(Index),new { id});
            }

            return View(category);
        }

        //GET/Admin/Categories/Delete
        public async Task<IActionResult> Delete(int id)
        {
            Category category = await _context.categories.FindAsync(id);

            if (category == null)
            {
                TempData["Error"] = "The category does not exist";
            }
            else
            {
                _context.Remove(category);
                await _context.SaveChangesAsync();

                TempData["Success"] = "The page has been deleted successfully";
            }

            return RedirectToAction(nameof(Index));
        }

        //POST/Admin/Categories/Reoder
        [HttpPost]
        public async Task<IActionResult> Reoder(int[] id)
        {
            int count = 1;

            foreach (var catId in id)
            {
                Category category = await _context.categories.FindAsync(catId);
                category.Sorting = count;
                _context.Update(category);
                await _context.SaveChangesAsync();
                count++;
            }
            return Ok();
        }
    }
}
