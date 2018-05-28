using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SplitPackage.Business;

namespace SplitPackage.EntityFrameworkCore.Seed.Tenants
{
    public class DefaultTenantBusinessBuilder
    {
        private readonly SplitPackageDbContext _context;

        public DefaultTenantBusinessBuilder(SplitPackageDbContext context)
        {
            this._context = context;
        }

        public void Create()
        {
            var tenant = this._context.Tenants.Where(o => o.TenancyName == "AstraeaAssistant").FirstOrDefault();
            if (tenant == null)
            {
                return;
            }
            if (this._context.TenantLogisticChannel.AsQueryable().Where(o=>o.TenantId == tenant.Id).Any())
            {
                return;
            }
            this._context.LogisticChannels.AsQueryable().IgnoreQueryFilters().Where(o => o.TenantId == null).Select(o => o.Id).ToList().ForEach(o =>
            {
                this._context.TenantLogisticChannel.Add(new SplitPackage.Business.TenantLogisticChannel() {
                    TenantId = tenant.Id,
                    LogisticChannelId = o
                });
            });
            var relatedItems = this._context.Logistics.AsQueryable().Where(o => new string[] { "EWE Express 标准线", "EWE Express 经济线" }.Contains(o.LogisticCode));
            var related = new LogisticRelated()
            {
                RelatedName = "EWE Express",
                TenantId = tenant.Id
            };
            related.Items = relatedItems.Select(o => new LogisticRelatedItem()
            {
                LogisticId = o.Id,
                LogisticRelatedBy = related,
            }).ToList();
            this._context.LogisticRelated.Add(related);
            this._context.SaveChanges();
        }
    }
}
