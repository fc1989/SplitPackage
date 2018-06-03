using Abp.Domain.Repositories;
using Abp.Runtime.Caching;
using SplitPackage.Business;
using SplitPackage.Cache.Dto;
using SplitPackage.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Abp.Domain.Uow;
using Abp.ObjectMapping;

namespace SplitPackage.Cache
{
    public class ManageCache
    {
        private readonly ICacheManager _cacheManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<Tenant> _tenantRepository;
        private readonly IRepository<ProductClass, long> _productClassRepository;
        private readonly IRepository<Logistic, long> _logisticRepository;
        private readonly IRepository<LogisticRelated, long> _logisticRelatedRepository;
        private readonly IRepository<TenantLogisticChannel, long> _importLogisticRepository;
        private readonly IObjectMapper _objectMapper;

        protected ITypedCache<string, List<ProductClassCacheDto>> productClassCache;
        protected ITypedCache<string, SplitPackageSettingCache> splitPackageSettingCache;

        public ManageCache(ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<Tenant> tenantRepository, 
            IRepository<ProductClass, long> productClassRepository,
            IRepository<Logistic, long> logisticRepository,
            IRepository<LogisticRelated, long> logisticRelatedRepository,
            IRepository<TenantLogisticChannel, long> importLogisticRepository,
            IObjectMapper objectMapper)
        {
            this._cacheManager = cacheManager;
            this._unitOfWorkManager = unitOfWorkManager;
            this._tenantRepository = tenantRepository;
            this._productClassRepository = productClassRepository;
            this._logisticRepository = logisticRepository;
            this._logisticRelatedRepository = logisticRelatedRepository;
            this._importLogisticRepository = importLogisticRepository;
            this._objectMapper = objectMapper;

            this.productClassCache = this._cacheManager.GetCache<string, List<ProductClassCacheDto>>("ProductClass");
            this.splitPackageSettingCache = this._cacheManager.GetCache<string, SplitPackageSettingCache>("SplitSetting");
        }

        public async Task<IList<ProductClassCacheDto>> GetProductClassAsync()
        {
            return await this.productClassCache.GetOrDefaultAsync("host");
        }

        public async Task<SplitPackageSettingCache> GetSplitPackageSettingAsync(int? tenantId)
        {
            return await this.splitPackageSettingCache.GetOrDefaultAsync(tenantId.HasValue ?tenantId.ToString() : "host");
        }

        public async Task SetProductClassAsync(IList<ProductClassCacheDto> pcSet)
        {
            await this.productClassCache.SetAsync("host", pcSet.ToList());
        }

        public async Task SetSplitPackageSettingAsync(int? tenantId, SplitPackageSettingCache settingCache)
        {
            await this.splitPackageSettingCache.SetAsync(tenantId.HasValue ? tenantId.ToString() : "host", settingCache);
        }

        public void InitCache()
        {
            using (var unitOfWork = _unitOfWorkManager.Begin())
            {
                List<ProductClassCacheDto> pcSet = this._productClassRepository.GetAll().Include(p=>p.ProductSortBy).ToList().Select(o => new ProductClassCacheDto()
                {
                    ProductClassId = o.Id,
                    ProductSortId = o.ProductSortId,
                    SortName = o.ProductSortBy.SortName,
                    ClassName = o.ClassName,
                    PTId = o.PTId,
                    PostTaxRate = o.PostTaxRate,
                    BCTaxRate = o.BCTaxRate,
                    IsActive = o.IsActive
                }).ToList();
                productClassCache.Set("host", pcSet);
                var logisticQuery = this._logisticRepository.GetAll().Include(p => p.LogisticChannels)
                    .ThenInclude((LogisticChannel p) => p.NumFreights)
                    .Include(p => p.LogisticChannels).ThenInclude((LogisticChannel p) => p.WeightFreights)
                    .Include(p => p.LogisticChannels).ThenInclude((LogisticChannel p) => p.SplitRules).ThenInclude((SplitRule p) => p.ProductClasses);
                var logisticRelatedQuery = this._logisticRelatedRepository.GetAll().Include(p => p.Items).ThenInclude((LogisticRelatedItem p) => p.LogisticBy);
                var hostLogistics = logisticQuery.IgnoreQueryFilters().Where(o => o.TenantId == null && o.IsActive).Select(this._objectMapper.Map<LogisticCacheDto>).ToList();
                splitPackageSettingCache.Set("host", new SplitPackageSettingCache()
                {
                    OwnLogistics = hostLogistics,
                    Relateds = logisticRelatedQuery.IgnoreQueryFilters().Where(o => o.TenantId == null).Select(this._objectMapper.Map<IList<LogisticRelatedOptionCacheDto>>).ToList()
                });
                foreach (var item in _tenantRepository.GetAll().Where(o => o.IsActive).ToList())
                {
                    var ownLogistics = logisticQuery.IgnoreQueryFilters().Where(o => o.TenantId == item.Id && o.IsActive).Select(this._objectMapper.Map<LogisticCacheDto>).ToList();
                    var import = this._importLogisticRepository.GetAll().Include(o => o.LogisticChannelBy)
                        .ThenInclude((LogisticChannel p) => p.LogisticBy)
                        .IgnoreQueryFilters().Where(o => o.TenantId == item.Id).ToList();
                    var set = hostLogistics.Where(o => import.Any(oi => oi.LogisticChannelBy.LogisticId == o.Id)).SelectMany(o => o.LogisticChannels)
                        .Where(o => import.Any(oi => oi.LogisticChannelId == o.Id)).ToList();
                    var importChannls = new ChannelCacheDto[set.Count];
                    set.CopyTo(importChannls);
                    var importSet = import.GroupBy(o => o.LogisticChannelBy.LogisticBy).Select(o => {
                        var result = new LogisticCacheDto()
                        {
                            Id = o.Key.Id,
                            CorporationName = o.Key.CorporationName,
                            CorporationUrl = o.Key.CorporationUrl,
                            LogoURL = o.Key.LogoURL,
                            LogisticCode = o.Key.LogisticCode,
                        };
                        result.LogisticChannels = o.Select(oi => {
                            var c = importChannls.Where(oii => oi.LogisticChannelId == oii.Id).First();
                            if (!string.IsNullOrEmpty(oi.AliasName))
                            {
                                c.AliasName = oi.AliasName;
                            }
                            if (oi.Way.HasValue)
                            {
                                c.Way = oi.Way.Value;
                            }
                            var information = oi.GetInformation();
                            if (information != null)
                            {
                                c.WeightFreights = information.WeightChargeRules.Select(this._objectMapper.Map<WeightFreightCacheDto>).ToList();
                                c.NumFreights = information.NumChargeRules.Select(this._objectMapper.Map<NumFreightCacheDto>).ToList();
                            }
                            return c;
                        }).ToList();
                        return result;
                    });
                    ownLogistics.AddRange(importSet);
                    var relateds = logisticRelatedQuery.IgnoreQueryFilters().Where(o => o.TenantId == item.Id).Select(this._objectMapper.Map<IList<LogisticRelatedOptionCacheDto>>).ToList();
                    if (ownLogistics.Count > 0)
                    {
                        splitPackageSettingCache.Set(item.Id.ToString(), new SplitPackageSettingCache()
                        {
                            OwnLogistics = ownLogistics,
                            Relateds = relateds
                        });
                    }
                }
                unitOfWork.Complete();
            }
        }
    }
}
