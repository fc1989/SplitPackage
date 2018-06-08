using SplitPackage.Business;
using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.EntityFrameworkCore.Seed.Business
{
    public class InitialBusinessDbBuilder
    {
        private readonly SplitPackageDbContext _context;

        public InitialBusinessDbBuilder(SplitPackageDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            new DefaultProductSortCreator(_context).Create();
            new DefaultLogisticCreator(_context).Create();
            new DefaultLogisticRelatedCreator(_context).Create();
            new DefaultOtherSystemCreator(_context).Create();
        }
    }
}
