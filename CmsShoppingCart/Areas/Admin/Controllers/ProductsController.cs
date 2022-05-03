using CmsShoppingCart.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CmsShoppingCart.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace CmsShoppingCart.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly CmsShoppingCartContextt _context;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ProductsController(CmsShoppingCartContextt context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            this.webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            return  View( await _context.products.OrderByDescending(x => x.Id).Include(c => c.Category).ToListAsync());
        }

        //GET/Admin/Products/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.CategoryId = new SelectList(_context.categories.OrderBy(x => x.Sorting), "Id","Name");
            return View();
        }

        //POST/Admin/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product products)
        {
            ViewBag.CategoryId = new SelectList(_context.categories.OrderBy(x => x.Sorting), "Id", "Name");

            if (ModelState.IsValid)
            {
                products.Slug = products.Name.ToLower().Replace(" ", "-");

                var slug = await _context.products.FirstOrDefaultAsync(x => x.Slug == products.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The Product already exists");
                    return View(products);
                }
                ////-----Upload Image--------///
                string imageName = "noimage.png";
                if (products.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(webHostEnvironment.WebRootPath, "media/product");
                    imageName = Guid.NewGuid().ToString() + "_" + products.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await products.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                }
                products.Image = imageName;

                _context.Add(products);
                await _context.SaveChangesAsync();


                TempData["success"] = "The product has been added";

                return RedirectToAction(nameof(Index));
            }
            return View(products);
        }
    }
}
