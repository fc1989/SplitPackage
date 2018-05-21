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

        public DefaultTenantBuilder(SplitPackageDbContext context)
        {
            _context = context;
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
            // Default tenant
            string[] tenants = new string[] { "AstraeaAssistant", "Auz", "Iautao", "Phoenix" };
            foreach (var item in tenants)
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
