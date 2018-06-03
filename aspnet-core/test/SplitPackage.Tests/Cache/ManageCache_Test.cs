using Abp.Domain.Repositories;
using Abp.Runtime.Caching;
using SplitPackage.Cache.Dto;
using SplitPackage.MultiTenancy;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SplitPackage.Tests.Cache
{
    public class ManageCache_Test : SplitPackageTestBase
    {
        private readonly ICacheManager _cacheManager;

        public ManageCache_Test(Xunit.Abstractions.ITestOutputHelper output)
        {
            this._cacheManager = Resolve<ICacheManager>();
        }

        [Fact]
        public void Test()
        {
            var pcCache = this._cacheManager.GetCache("ProductClass");
            var productClassSet = pcCache.GetOrDefault<string, List<ProductClassCacheDto>>("host");
            Assert.True(productClassSet.Count == 141);
            var ssCache = this._cacheManager.GetCache<string, SplitPackageSettingCache>("SplitSetting");
            Assert.True(ssCache.GetOrDefault("host").OwnLogistics.Count == 17);
            Assert.True(ssCache.GetOrDefault("host").Relateds.Count == 1);
            IRepository<Tenant> tenantRepository = Resolve<IRepository<Tenant>>();
            var tenants = tenantRepository.GetAllList();
            foreach (var item in tenants)
            {
                if (item.TenancyName == "AstraeaAssistant")
                {
                    Assert.True(ssCache.GetOrDefault(item.Id.ToString()).OwnLogistics.Count == 17);
                    Assert.True(ssCache.GetOrDefault(item.Id.ToString()).Relateds.Count == 1);
                }
                else
                {
                    Assert.Null(ssCache.GetOrDefault(item.Id.ToString()));
                }
            }
        }
    }
}
