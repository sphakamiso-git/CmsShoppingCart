using CmsShoppingCart.Infrastructure;
using CmsShoppingCart.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CmsShoppingCart.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly CmsShoppingCartContextt _context;
         
        public ProductsController(CmsShoppingCartContextt context)
        {
            _context = context;
        }
        //GET/Products
        public async Task<IActionResult> Index(int p = 1)
        {
            int pageSize = 6;
            var products = _context.products.OrderByDescending(x => x.Id)              
                                            .Skip((p - 1) * pageSize)
                                            .Take(pageSize);

            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)_context.products.Count() / pageSize);
            return View(await products.ToListAsync());
        }

       //GET/products/category
       public async Task<IActionResult>ProductsByCategory(string categorySlug,int p = 1)
        {
            Category category = await _context.categories.Where(c => c.Slug == categorySlug).FirstOrDefaultAsync();
            if (category == null)
            {
                RedirectToAction(nameof(Index));
            }

            int pageSize =6;
            var products = _context.products.OrderByDescending(x => x.Id)
                                            .Where(c => c.CategoryId == category.Id)
                                            .Skip((p - 1) * pageSize)
                                            .Take(pageSize);

            ViewBag.pageNumber = p;
            ViewBag.pageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)_context.products.Where(x => x.CategoryId == category.Id).Count() / pageSize);
            ViewBag.CategoryName = category.Name;
            ViewBag.CategorySlug = categorySlug;
            return View(await products.ToListAsync());
        }
    }
}
