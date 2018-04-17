using Microsoft.EntityFrameworkCore;
using SplitPackage.EntityFrameworkCore;
using System.Linq;
using SplitPackage.Business;

namespace SplitPackage.EntityFrameworkCore.Seed.Business
{
    public class DefaultProductProductClassCreator
    {
        private readonly SplitPackageDbContext _context;

        public DefaultProductProductClassCreator(SplitPackageDbContext context)
        {
            this._context = context;
        }

        public void Create()
        {
            AddIfNotExists(new ProductProductClass()
            {
                ProductId = 1,
                ProductClassId = 1
            });
        }

        private void AddIfNotExists(ProductProductClass pc)
        {
            if (_context.ProductProductClass.IgnoreQueryFilters().Any(s => s.ProductId == pc.ProductId && s.ProductClassId == pc.ProductClassId))
            {
                return;
            }

            _context.ProductProductClass.Add(pc);
            _context.SaveChanges();
        }
    }
}
