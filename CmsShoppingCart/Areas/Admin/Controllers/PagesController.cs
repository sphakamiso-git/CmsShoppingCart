using CmsShoppingCart.Infrastructure;
using CmsShoppingCart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CmsShoppingCart.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PagesController : Controller
    {
        private readonly CmsShoppingCartContext _context;

        public PagesController(CmsShoppingCartContext context)
        {
            this._context = context;
        }
        public async Task<IActionResult> Index()
        {
            IQueryable<Page> pages = from p in _context.Pages
                                     orderby p.Sorting
                                     select p;

            List<Page> pageList = await pages.ToListAsync();
            return View(pageList);
        }

        //GET/Admin/Details
        public async Task<IActionResult> Details(int? id)
        {
            Page page = await _context.Pages.FirstOrDefaultAsync(c => c.Id == id);
            if (id == null)
            {
                 return NotFound();
            }

            return View(page);
        }

        //GET/Admin/Create
        public async Task<IActionResult> Create() => View();

        //POST/Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Page page)
        {
            if (ModelState.IsValid)
            {
                page.Slug = page.Title.ToLower().Replace(" ", "-");
                page.Sorting = 100;

                var slug = await _context.Pages.FirstOrDefaultAsync(x => x.Slug == page.Slug);
                if(slug != null)
                {
                    ModelState.AddModelError("", "The title already exists");
                    return View(page);
                }


                _context.Add(page);
                await _context.SaveChangesAsync();

                TempData["success"] = "The page has been added!";


                return RedirectToAction(nameof(Index));
            }
            return View(page);
        }

        //GET/Admin/Edit
        public async Task<IActionResult> Edit(int? id)
        {
            Page page = await _context.Pages.FindAsync(id);
            if (page == null)
            {
                return NotFound();
            }
            return View(page);
        }

        //POST/Admin/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, Page page)
        {
            if (ModelState.IsValid)
            {
                page.Slug = page.Id == 1 ? "home" : page.Title.ToLower().Replace(" ", "-");
                page.Sorting = 100;

                var slug = await _context.Pages.Where(c => c.Id != page.Id).FirstOrDefaultAsync(x => x.Slug == page.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The page already exists");
                    return View(page);
                }
                _context.Update(page);
                await _context.SaveChangesAsync();

                TempData["success"] = "The page has been edited";
                return RedirectToAction(nameof(Index), new { id = page.Id });
            }

            return View();
        }
    }
}
