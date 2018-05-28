using System;
using System.Collections.Generic;
using System.Text;

namespace SplitPackage.EntityFrameworkCore.Seed.Tenants
{
    public class InitialTenantBuilder
    {
        private readonly SplitPackageDbContext _context;

        public InitialTenantBuilder(SplitPackageDbContext context)
        {
            this._context = context;
        }

        public void Create()
        {
            new DefaultTenantBuilder(_context, new string[] { "AstraeaAssistant", "Auz", "Iautao", "Phoenix" }).Create();
            new DefaultTenantBusinessBuilder(_context).Create();
        }
    }
}
