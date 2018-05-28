using System.Linq;
using Microsoft.EntityFrameworkCore;
using Abp.MultiTenancy;
using SplitPackage.Editions;
using SplitPackage.MultiTenancy;

namespace SplitPackage.EntityFrameworkCore.Seed.Tenants
{
    public class DefaultTenantBuilder
    {
        private readonly SplitPackageDbContext _context;
        private readonly string[] _tenants;

        public DefaultTenantBuilder(SplitPackageDbContext context,string[] tenants)
        {
            this._context = context;
            this._tenants = tenants;
        }

        public void Create()
        {
            if (_context.Tenants.Any())
            {
                return;
            }
            CreateDefaultTenant();
        }

        private void CreateDefaultTenant()
        {
            foreach (var item in this._tenants)
            {
                var tenant = new Tenant(item, item);
                var defaultEdition = _context.Editions.IgnoreQueryFilters().FirstOrDefault(e => e.Name == EditionManager.DefaultEditionName);
                if (defaultEdition != null)
                {
                    tenant.EditionId = defaultEdition.Id;
                }
                _context.Tenants.Add(tenant);
                _context.SaveChanges();
                new TenantRoleAndUserBuilder(_context,tenant.Id).Create();
            }
        }
    }
}
