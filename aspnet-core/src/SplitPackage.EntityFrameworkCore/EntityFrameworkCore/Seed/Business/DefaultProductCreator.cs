using Microsoft.EntityFrameworkCore;
using SplitPackage.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SplitPackage.EntityFrameworkCore.Seed.Business
{
    public class DefaultProductCreator
    {
        private readonly SplitPackageDbContext _context;

        public DefaultProductCreator(SplitPackageDbContext context)
        {
            this._context = context;
        }

        public void Create()
        {
            AddIfNotExists(new Product() {
                ProductName = "NESTLE成人奶粉840G",
                Sku = "7613035243453",
                Brand = "Nestle",
                Weight = 1,
                DeclarePrice = 200,
                DeclareTaxrate = 0.5,
                PTId = "1010706"
            });
        }

        private void AddIfNotExists(Product p)
        {
            if (_context.Products.Any(s => p.Sku == p.Sku))
            {
                return;
            }

            _context.Products.Add(p);
            _context.SaveChanges();
        }
    }
}
