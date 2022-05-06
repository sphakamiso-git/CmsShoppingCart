﻿using CmsShoppingCart.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CmsShoppingCart.Infrastructure
{
    public class CategoriesViewComponent : ViewComponent
    {
        private readonly CmsShoppingCartContextt _context;

        public CategoriesViewComponent(CmsShoppingCartContextt context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await GetCategoriesAsync();
            return View(categories);
        }

        private Task<List<Category>> GetCategoriesAsync()
        {
            return _context.categories.OrderBy(x => x.Sorting).ToListAsync();
        }
    }
}
