﻿using Abp.Domain.Repositories;
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
            Assert.Equal(141, productClassSet.Count);
            var ssCache = this._cacheManager.GetCache<string, SplitPackageSettingCache>("SplitSetting");
            Assert.Equal(18, ssCache.GetOrDefault("host").OwnLogistics.Count);
            Assert.Equal(1, ssCache.GetOrDefault("host").Relateds.Count);
            IRepository<Tenant> tenantRepository = Resolve<IRepository<Tenant>>();
            var tenants = tenantRepository.GetAllList();
            foreach (var item in tenants)
            {
                if (item.TenancyName == "AstraeaAssistant")
                {
                    Assert.Equal(18, ssCache.GetOrDefault(item.Id.ToString()).OwnLogistics.Count);
                    Assert.Equal(1, ssCache.GetOrDefault(item.Id.ToString()).Relateds.Count);
                }
                else
                {
                    Assert.Null(ssCache.GetOrDefault(item.Id.ToString()));
                }
            }
        }
    }
}
