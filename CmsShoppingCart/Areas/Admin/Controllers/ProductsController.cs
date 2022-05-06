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
using Microsoft.AspNetCore.Authorization;

namespace CmsShoppingCart.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> Index(int p = 1)
        {
            int pageSize = 6;
            var products = _context.products.OrderByDescending(x => x.Id)
                .Include(c => c.Category)
                .Skip((p - 1) * pageSize)
                .Take(pageSize);

            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)_context.products.Count()/pageSize);

            return View(await products.ToListAsync());
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

        //GET/Admin/Products/details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Product product = await _context.products.Include(x => x.Category).FirstOrDefaultAsync(c => c.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        //GET/Admin/Products/Edit
        public async Task<IActionResult> Edit(int id)
        {
            Product product = await _context.products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.CategoryId = new SelectList(_context.categories.OrderBy(x => x.Sorting), "Id", "Name");

            return View(product);
        }

        //POST/Admin/Products/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product,int id)
        {
            ViewBag.CategoryId = new SelectList(_context.categories.OrderBy(x => x.Sorting), "Id", "Name",product.CategoryId);

            if (ModelState.IsValid)
            {
                product.Slug = product.Name.ToLower().Replace(" ", "-");
                var slug = await _context.products.Where(x => x.Id != id).FirstOrDefaultAsync(c => c.Slug == product.Slug);
                if (slug != null)
                {
                    ModelState.AddModelError("", "The Product already exist");
                    return View(product);
                }
                //string imageName = "noimage.png";
                if (product.ImageUpload != null)
                {
                    string uploadsDir = Path.Combine(webHostEnvironment.WebRootPath, "media/product");

                    if (!string.Equals(product.Image, "noimage.png"))
                    {
                        string oldImagePath = Path.Combine(uploadsDir, product.Image);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    string imageName = Guid.NewGuid().ToString() + "_" + product.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadsDir, imageName);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await product.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    product.Image = imageName;

                }
                _context.Update(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "The product has been Updated";

                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        //GET/Admin/Product/Delete
        public async Task<IActionResult> Delete(int id)
        {
            Product product = await _context.products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            else
            {
                if (!string.Equals(product.Image, "noimage.png"))
                {
                    string uploadsDir = Path.Combine(webHostEnvironment.WebRootPath, "media/products");
                    string oldImagePath = Path.Combine(uploadsDir, product.Image);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                _context.products.Remove(product);
                await _context.SaveChangesAsync();

                TempData["Success"] = "The Page has been deleted";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
