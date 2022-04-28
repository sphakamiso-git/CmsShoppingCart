using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CmsShoppingCart.Infrastructure
{
    public class CmsShoppingCartContext:DbContext
    {
        public CmsShoppingCartContext(DbContextOptions<CmsShoppingCartContext> options):base(options)
        {

        }
    }
}
