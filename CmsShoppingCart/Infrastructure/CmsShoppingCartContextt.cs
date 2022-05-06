using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CmsShoppingCart.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CmsShoppingCart.Infrastructure
{
    public class CmsShoppingCartContextt:IdentityDbContext<AppUser>
    {
        public CmsShoppingCartContextt(DbContextOptions<CmsShoppingCartContextt> options):base(options)
        {

        }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<Product> products { get; set; }
       // public DbSet<AppUser> AppUsers { get; set; }
    
    }
}
