using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using SplitPackage.Cache;
using SplitPackage.Cache.Dto;
using SplitPackage.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SplitPackage.Tests.Contexts
{
    public abstract class BaseCaseContext : SplitPackageTestBase, IDisposable
    {
        public BaseCaseContext():base()
        {

        }

        #region cache
        public async Task UsingCurrentSettingCacheAsync(Func<SplitPackageSettingCache, Task> action)
        {
            var mc = IocManager.Resolve<ManageCache>();
            var settingCache = await mc.GetSplitPackageSettingAsync(AbpSession.TenantId);
            await action(settingCache);
        }

        public async Task UsingTenantSettingCacheAsync(Func<SplitPackageSettingCache, Task> action)
        {
            var mc = IocManager.Resolve<ManageCache>();
            using (var unitOfWork = IocManager.Resolve<IUnitOfWorkManager>().Begin())
            {
                var tenants = await IocManager.Resolve<IRepository<Tenant>>().GetAll().ToListAsync();
                foreach (var item in tenants)
                {
                    var settingCache = await mc.GetSplitPackageSettingAsync(item.Id);
                    await action(settingCache);
                }
            }
        }
        #endregion

        public int? GetTenantId()
        {
            return this.AbpSession.TenantId;
        }

        public T ResolveService<T>()
        {
            return this.Resolve<T>();
        }

        public virtual void Dispose()
        {

        }
    }
}
