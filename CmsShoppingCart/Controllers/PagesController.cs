using CmsShoppingCart.Infrastructure;
using CmsShoppingCart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CmsShoppingCart.Controllers
{
    public class PagesController : Controller
    {
        private readonly CmsShoppingCartContextt _context;

        public PagesController(CmsShoppingCartContextt context)
        {
            this._context = context;
        }

        //GET/
        public async Task<IActionResult> Page(string slug)
        {
            if (slug == null)
            {
                return View(await _context.Pages.Where(x => x.Slug == "home").FirstOrDefaultAsync());
            }
            Page page = await _context.Pages.Where(x => x.Slug == slug).FirstOrDefaultAsync();

            if (page == null)
            {
                return NotFound();
            }
            return View(page);
        }
    }
}
