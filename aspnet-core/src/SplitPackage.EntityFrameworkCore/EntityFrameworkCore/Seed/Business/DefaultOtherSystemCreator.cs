using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using SplitPackage.MultiTenancy;

namespace SplitPackage.EntityFrameworkCore.Seed.Business
{
    public class DefaultOtherSystemCreator
    {
        private readonly SplitPackageDbContext _context;

        public DefaultOtherSystemCreator(SplitPackageDbContext context)
        {
            this._context = context;
        }

        public void Create()
        {
            var systemNames = new string[] { "UPost" };
            var importedOtherSystem = this._context.OtherSystems.Where(o => systemNames.Contains(o.SystemName)).ToList();
            foreach (var item in systemNames)
            {
                if (importedOtherSystem.Any(o => o.SystemName == item))
                {
                    return;
                }
                var system = new OtherSystem()
                {
                    SystemName = item,
                    Certificate = item
                };
                this._context.OtherSystems.Add(system);
            }
            this._context.SaveChanges();
        }
    }
}
