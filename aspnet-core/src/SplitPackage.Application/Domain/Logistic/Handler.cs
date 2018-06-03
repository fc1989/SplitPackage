using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Events.Bus.Entities;
using Abp.Events.Bus.Handlers;
using SplitPackage.Business;
using SplitPackage.Cache;
using SplitPackage.Cache.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Abp.Domain.Uow;
using Microsoft.EntityFrameworkCore;
using Abp.ObjectMapping;
using Abp.Events.Bus;

namespace SplitPackage.Domain.Logistic
{
    public class Handler : 
        IAsyncEventHandler<EntityCreatedEventData<Business.Logistic>>,
        IAsyncEventHandler<ModifyLogisticEvent>,
        IAsyncEventHandler<TenantModifyImportLogisticEvent>,
        IAsyncEventHandler<StartUseLogisticEvent>,
        IAsyncEventHandler<BanishLogisticEvent>,
        IAsyncEventHandler<TenantStartUseImportLogisticEvent>,
        IAsyncEventHandler<TenantBanishImportLogisticEvent>,

        IAsyncEventHandler<EntityCreatedEventData<LogisticChannel>>,
        IAsyncEventHandler<ModifyChannelEvent>,
        IAsyncEventHandler<TenantModifyImportChannelEvent>,
        IAsyncEventHandler<StartUseChannelEvent>,
        IAsyncEventHandler<TenantStartUseImportChannelEvent>,
        IAsyncEventHandler<BanishChannelEvent>,
        IAsyncEventHandler<TenantBanishImportChannelEvent>,
        IAsyncEventHandler<TenantImportChannelEvent>,

        IAsyncEventHandler<EntityCreatedEventData<SplitRule>>,
        IAsyncEventHandler<TenantCreateImportSplitRuleEvent>,
        IAsyncEventHandler<EntityUpdatedEventData<SplitRule>>,
        IAsyncEventHandler<TenanModifyImportSplitRuleEvent>,
        IAsyncEventHandler<StartUseSplitRuleEvent>,
        IAsyncEventHandler<TenantStartUseImportSplitRuleEvent>,
        IAsyncEventHandler<BanishSplitRuleEvent>,
        IAsyncEventHandler<TenantBanishImportSplitRuleEvent>,

        IAsyncEventHandler<EntityCreatedEventData<SplitRuleProductClass>>,
        IAsyncEventHandler<TenantCreateImportSplitRuleItemEvent>,
        IAsyncEventHandler<EntityUpdatedEventData<SplitRuleProductClass>>,
        IAsyncEventHandler<TenanModifyImportSplitRuleItemEvent>,
        IAsyncEventHandler<BanishSplitRuleItemEvent>,
        IAsyncEventHandler<BanishTenantSplitRuleItemEvent>,

        ITransientDependency
    {
        private readonly ManageCache _manageCache;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public IObjectMapper _objectMapper;
        public IEventBus EventBus;
        private readonly IRepository<Business.Logistic, long> _logisticRepository;
        private readonly IRepository<LogisticChannel, long> _logisticChannelRepository;
        private readonly IRepository<TenantLogisticChannel, long> _tenantLogisticChannelRepository;
        private readonly IRepository<SplitRule, long> _splitRuleRepository;
        private readonly IRepository<SplitRuleProductClass, long> _splitRuleProductClassRepository;

        public Handler(ManageCache manageCache,
            IUnitOfWorkManager unitOfWorkManager,
            IObjectMapper objectMapper,
            IRepository<Business.Logistic, long> logisticRepository,
            IRepository<LogisticChannel, long> logisticChannelRepository,
            IRepository<TenantLogisticChannel, long> tenantLogisticChannelRepository,
            IRepository<SplitRule, long> splitRuleRepository,
            IRepository<SplitRuleProductClass, long> splitRuleProductClassRepository)
        {
            this._manageCache = manageCache;
            this._unitOfWorkManager = unitOfWorkManager;
            this._objectMapper = objectMapper;
            this.EventBus = NullEventBus.Instance;
            this._logisticRepository = logisticRepository;
            this._logisticChannelRepository = logisticChannelRepository;
            this._tenantLogisticChannelRepository = tenantLogisticChannelRepository;
            this._splitRuleRepository = splitRuleRepository;
            this._splitRuleProductClassRepository = splitRuleProductClassRepository;
        }

        #region logistic
        public async Task HandleEventAsync(EntityCreatedEventData<Business.Logistic> eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.Entity.TenantId);
            settingCache.OwnLogistics.Add(new LogisticCacheDto() {
                Id = eventData.Entity.Id,
                CorporationName = eventData.Entity.CorporationName,
                CorporationUrl = eventData.Entity.CorporationUrl,
                LogoURL = eventData.Entity.LogoURL,
                LogisticCode = eventData.Entity.LogisticCode,
                LogisticChannels = new List<ChannelCacheDto>()
            });
            await this._manageCache.SetSplitPackageSettingAsync(eventData.Entity.TenantId, settingCache);
        }

        public async Task HandleEventAsync(ModifyLogisticEvent eventData)
        {
            if (!eventData.IsActive)
            {
                return;
            }
            //update cache for myself
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticCache = settingCache.OwnLogistics.First(o => o.Id == eventData.Id);
            logisticCache.CorporationName = eventData.CorporationName;
            logisticCache.CorporationUrl = eventData.CorporationUrl;
            logisticCache.LogoURL = eventData.LogoURL;
            logisticCache.LogisticCode = eventData.LogisticCode;
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
            //associated impact other
            if (!eventData.TenantId.HasValue)
            {
                var importTenantId = await this._tenantLogisticChannelRepository.GetAll()
                    .Where(o => o.LogisticChannelBy.LogisticBy.Id == eventData.Id)
                    .Select(o => o.TenantId).Distinct().ToListAsync();
                foreach (var item in importTenantId)
                {
                    await this.EventBus.TriggerAsync(new TenantModifyImportLogisticEvent()
                    {
                        TenantId = item,
                        LogisticId = eventData.Id
                    });
                }
            }
        }

        public async Task HandleEventAsync(TenantModifyImportLogisticEvent eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(null);
            var tenantSettingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticCache = settingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId);
            var tenantLogisticCache = tenantSettingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId);
            tenantLogisticCache.CorporationName = logisticCache.CorporationName;
            tenantLogisticCache.CorporationUrl = logisticCache.CorporationUrl;
            tenantLogisticCache.LogoURL = logisticCache.LogoURL;
            tenantLogisticCache.LogisticCode = logisticCache.LogisticCode;
            await _manageCache.SetSplitPackageSettingAsync(eventData.TenantId, tenantSettingCache);
        }

        public async Task HandleEventAsync(StartUseLogisticEvent eventData)
        {
            //update cache for myself
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticData = await this._logisticRepository.GetAll()
                .Include(p => p.LogisticChannels).ThenInclude((LogisticChannel p) => p.WeightFreights)
                .Include(p => p.LogisticChannels).ThenInclude((LogisticChannel p) => p.NumFreights)
                .Include(p => p.LogisticChannels).ThenInclude((LogisticChannel p) => p.SplitRules).ThenInclude((SplitRule p) => p.ProductClasses)
                .IgnoreQueryFilters()
                .FirstAsync(o=>o.TenantId == eventData.TenantId && o.Id == eventData.LogisticId);
            var logisticCache = this._objectMapper.Map<LogisticCacheDto>(logisticData);
            settingCache.OwnLogistics.Add(logisticCache);
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
            //Associated impact other
            if (!eventData.TenantId.HasValue)
            {
                var importTenantId = await this._tenantLogisticChannelRepository.GetAll()
                    .Where(o => o.LogisticChannelBy.LogisticBy.Id == eventData.LogisticId)
                    .Select(o => o.TenantId).Distinct().ToListAsync();
                foreach (var item in importTenantId)
                {
                    await this.EventBus.TriggerAsync(new TenantStartUseImportLogisticEvent() {
                        TenantId = item,
                        LogisticId = eventData.LogisticId
                    });
                }
            }
        }

        public async Task HandleEventAsync(TenantStartUseImportLogisticEvent eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(null);
            var tenantSettingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var importChannelSet = await this._tenantLogisticChannelRepository.GetAll().IgnoreQueryFilters()
                .Where(o => o.LogisticChannelBy.LogisticId == eventData.LogisticId && o.TenantId == eventData.TenantId).ToListAsync();
            var tenantChannelSet = importChannelSet.Select(o =>
            {
                var channel = settingCache.OwnLogistics.SelectMany(oi => oi.LogisticChannels).First(oi => oi.Id == o.LogisticChannelId);
                if (!string.IsNullOrEmpty(o.AliasName))
                {
                    channel.AliasName = o.AliasName;
                }
                if (o.Way.HasValue)
                {
                    channel.Way = o.Way.Value;
                }
                var information = o.GetInformation();
                if (information != null)
                {
                    channel.WeightFreights = information.WeightChargeRules.Select(this._objectMapper.Map<WeightFreightCacheDto>).ToList();
                    channel.NumFreights = information.NumChargeRules.Select(this._objectMapper.Map<NumFreightCacheDto>).ToList();
                }
                return channel;
            });
            var logisticCache = settingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId);
            logisticCache.LogisticChannels = tenantChannelSet.ToList();
            tenantSettingCache.OwnLogistics.Add(logisticCache);
            await _manageCache.SetSplitPackageSettingAsync(eventData.TenantId, tenantSettingCache);
        }

        public async Task HandleEventAsync(BanishLogisticEvent eventData)
        {
            //update cache for myself
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            settingCache.OwnLogistics.Remove(settingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId));
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
            //associated impact other
            if (!eventData.TenantId.HasValue)
            {
                var importTenantId = await this._tenantLogisticChannelRepository.GetAll()
                    .Where(o => o.LogisticChannelBy.LogisticBy.Id == eventData.LogisticId)
                    .Select(o => o.TenantId).Distinct().ToListAsync();
                foreach (var item in importTenantId)
                {
                    await this.EventBus.TriggerAsync(new TenantBanishImportLogisticEvent()
                    {
                        TenantId = item,
                        LogisticId = eventData.LogisticId
                    });
                }
            }
        }

        public async Task HandleEventAsync(TenantBanishImportLogisticEvent eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            settingCache.OwnLogistics.Remove(settingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId));
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
        }
        #endregion

        #region channel
        public async Task HandleEventAsync(EntityCreatedEventData<LogisticChannel> eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.Entity.TenantId);
            settingCache.OwnLogistics.First(o => o.Id == eventData.Entity.LogisticId).LogisticChannels.Add(this._objectMapper.Map<ChannelCacheDto>(eventData.Entity));
            await this._manageCache.SetSplitPackageSettingAsync(eventData.Entity.TenantId, settingCache);
        }

        public async Task HandleEventAsync(ModifyChannelEvent eventData)
        {
            if (!eventData.IsActive)
            {
                return;
            }
            //update cache for myself
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var channelCache = settingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId).LogisticChannels.First(o => o.Id == eventData.Id);
            this._objectMapper.Map<ModifyChannelEvent, ChannelCacheDto>(eventData, channelCache);
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
            //associated import other
            if (!eventData.TenantId.HasValue)
            {
                var importTenantId = await this._tenantLogisticChannelRepository.GetAll()
                    .Where(o => o.LogisticChannelId == eventData.Id)
                    .Select(o => o.TenantId).Distinct().ToListAsync();
                foreach (var item in importTenantId)
                {
                    await this.EventBus.TriggerAsync(new TenantModifyImportChannelEvent()
                    {
                        TenantId = item,
                        LogisticId = eventData.LogisticId,
                        ChannelId = eventData.Id
                    });
                }
            }
        }

        public async Task HandleEventAsync(TenantModifyImportChannelEvent eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(null);
            var tenantSettingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var channelCache = settingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId).LogisticChannels.First(o => o.Id == eventData.ChannelId);
            var tenantChannelCache = tenantSettingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId).LogisticChannels.First(o => o.Id == eventData.ChannelId);
            var importRelated = await this._tenantLogisticChannelRepository.SingleAsync(o => o.TenantId == eventData.TenantId && o.LogisticChannelId == eventData.ChannelId);
            if (string.IsNullOrEmpty(importRelated.AliasName))
            {
                tenantChannelCache.AliasName = channelCache.AliasName;
            }
            else
            {
                tenantChannelCache.AliasName = importRelated.AliasName;
            }
            if (!importRelated.Way.HasValue)
            {
                tenantChannelCache.Way = channelCache.Way;
            }
            else
            {
                tenantChannelCache.Way = importRelated.Way.Value;
            }
            var information = importRelated.GetInformation();
            if (information == null)
            {
                tenantChannelCache.WeightFreights = channelCache.WeightFreights;
                tenantChannelCache.NumFreights = channelCache.NumFreights;
            }
            else
            {
                tenantChannelCache.WeightFreights = information.WeightChargeRules.Select(this._objectMapper.Map<WeightFreightCacheDto>).ToList();
                tenantChannelCache.NumFreights = information.NumChargeRules.Select(this._objectMapper.Map<NumFreightCacheDto>).ToList();
            }
            tenantChannelCache.Type = channelCache.Type;
            tenantChannelCache.ChannelName = channelCache.ChannelName;
            tenantChannelCache.SplitRules = channelCache.SplitRules;
            await _manageCache.SetSplitPackageSettingAsync(eventData.TenantId, tenantSettingCache);
        }

        public async Task HandleEventAsync(StartUseChannelEvent eventData)
        {
            //update cache for myself
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var channelData = await this._logisticChannelRepository.GetAll()
                .Include(p => p.WeightFreights)
                .Include(p => p.NumFreights)
                .Include(p => p.SplitRules).ThenInclude((SplitRule p) => p.ProductClasses)
                .IgnoreQueryFilters()
                .FirstAsync(o => o.TenantId == eventData.TenantId && o.Id == eventData.ChannelId);
            var channelCache = this._objectMapper.Map<ChannelCacheDto>(channelData);
            settingCache.OwnLogistics.First(o=>o.Id == eventData.LogisticId).LogisticChannels.Add(channelCache);
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
            //Associated impact other
            if (!eventData.TenantId.HasValue)
            {
                var importTenantId = await this._tenantLogisticChannelRepository.GetAll()
                    .Where(o => o.LogisticChannelId == eventData.ChannelId)
                    .Select(o => o.TenantId).Distinct().ToListAsync();
                foreach (var item in importTenantId)
                {
                    await this.EventBus.TriggerAsync(new TenantStartUseImportChannelEvent()
                    {
                        TenantId = item,
                        LogisticId = eventData.LogisticId,
                        ChannelId = eventData.ChannelId
                    });
                }
            }
        }

        public async Task HandleEventAsync(TenantStartUseImportChannelEvent eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(null);
            var tenantSettingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var importRelated = await this._tenantLogisticChannelRepository.GetAll().IgnoreQueryFilters()
                .SingleAsync(o => o.LogisticChannelId == eventData.ChannelId && o.TenantId == eventData.TenantId);
            var channelCache = settingCache.OwnLogistics.SelectMany(oi => oi.LogisticChannels).First(oi => oi.Id == eventData.ChannelId);
            if (!string.IsNullOrEmpty(importRelated.AliasName))
            {
                channelCache.AliasName = importRelated.AliasName;
            }
            if (importRelated.Way.HasValue)
            {
                channelCache.Way = importRelated.Way.Value;
            }
            var information = importRelated.GetInformation();
            if (information != null)
            {
                channelCache.WeightFreights = information.WeightChargeRules.Select(this._objectMapper.Map<WeightFreightCacheDto>).ToList();
                channelCache.NumFreights = information.NumChargeRules.Select(this._objectMapper.Map<NumFreightCacheDto>).ToList();
            }
            tenantSettingCache.OwnLogistics.First(o=>o.Id == eventData.LogisticId).LogisticChannels.Add(channelCache);
            await _manageCache.SetSplitPackageSettingAsync(eventData.TenantId, tenantSettingCache);
        }

        public async Task HandleEventAsync(BanishChannelEvent eventData)
        {
            //update cache for myself
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var channelCache = settingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId).LogisticChannels.First(o=>o.Id == eventData.ChannelId);
            settingCache.OwnLogistics.First(o=>o.Id == eventData.LogisticId).LogisticChannels.Remove(channelCache);
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
            //associated impact other
            if (!eventData.TenantId.HasValue)
            {
                var importTenantId = await this._tenantLogisticChannelRepository.GetAll()
                    .Where(o => o.LogisticChannelId == eventData.ChannelId)
                    .Select(o => o.TenantId).Distinct().ToListAsync();
                foreach (var item in importTenantId)
                {
                    await this.EventBus.TriggerAsync(new TenantBanishImportChannelEvent()
                    {
                        TenantId = item,
                        LogisticId = eventData.LogisticId,
                        ChannelId = eventData.ChannelId
                    });
                }
            }
        }

        public async Task HandleEventAsync(TenantBanishImportChannelEvent eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var channelCache = settingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId).LogisticChannels.First(o => o.Id == eventData.ChannelId);
            settingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId).LogisticChannels.Remove(channelCache);
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
        }

        public async Task HandleEventAsync(TenantImportChannelEvent eventData)
        {
            var channelsSet = await this._logisticChannelRepository.GetAll().Where(o => eventData.RemoveChannelIds.Union(eventData.AddChannelIds).Contains(o.Id))
                .Select(o=>new {
                    o.LogisticId,
                    ChannelId = o.Id
                }).ToListAsync();
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(null);
            var tenantSettingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            foreach (var item in eventData.AddChannelIds)
            {
                var channel = channelsSet.First(o => o.ChannelId == item);
                var channelCache = settingCache.OwnLogistics.First(o => o.Id == channel.LogisticId).LogisticChannels.First(o => o.Id == channel.ChannelId);
                tenantSettingCache.OwnLogistics.First(o => o.Id == channel.LogisticId).LogisticChannels.Add(channelCache);
            }
            foreach (var item in eventData.RemoveChannelIds)
            {
                var channel = channelsSet.First(o => o.ChannelId == item);
                var tenantChannelCache = tenantSettingCache.OwnLogistics.First(o => o.Id == channel.LogisticId).LogisticChannels.First(o => o.Id == channel.ChannelId);
                tenantSettingCache.OwnLogistics.First(o => o.Id == channel.LogisticId).LogisticChannels.Remove(tenantChannelCache);
            }
            await _manageCache.SetSplitPackageSettingAsync(eventData.TenantId, tenantSettingCache);
        }
        #endregion

        #region splitrule
        public async Task HandleEventAsync(EntityCreatedEventData<SplitRule> eventData)
        {
            //update cache for myself
            var splitRule = await this._splitRuleRepository.GetAll()
                .Where(o => o.Id == eventData.Entity.Id)
                .Select(o=> new {
                    TenantId = o.LogisticChannelBy.TenantId,
                    LogisticId = o.LogisticChannelBy.LogisticId,
                    LogisticChannelId = o.LogisticChannelId
                })
                .FirstAsync();
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(splitRule.TenantId);
            settingCache.OwnLogistics.First(o => o.Id == splitRule.LogisticId).LogisticChannels
                .First(o=> o.Id == splitRule.LogisticChannelId).SplitRules
                .Add(this._objectMapper.Map<SplitRuleCacheDto>(eventData.Entity));
            await this._manageCache.SetSplitPackageSettingAsync(splitRule.TenantId, settingCache);
            //associated  import other
            if (!splitRule.TenantId.HasValue)
            {
                var importTenantId = await this._tenantLogisticChannelRepository.GetAll()
                    .Where(o => o.LogisticChannelId == eventData.Entity.LogisticChannelId)
                    .Select(o => o.TenantId).Distinct().ToListAsync();
                foreach (var item in importTenantId)
                {
                    await this.EventBus.TriggerAsync(new TenantCreateImportSplitRuleEvent()
                    {
                        TenantId = item,
                        LogisticId = splitRule.LogisticId,
                        ChannelId = splitRule.LogisticChannelId,
                        SplitRuleId = eventData.Entity.Id
                    });
                }
            }
        }

        public async Task HandleEventAsync(TenantCreateImportSplitRuleEvent eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(null);
            var tenantSettingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var splitRuleCache = settingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId)
                .LogisticChannels.First(o => o.Id == eventData.ChannelId)
                .SplitRules.First(o => o.Id == eventData.SplitRuleId);
            tenantSettingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId)
                .LogisticChannels.First(o => o.Id == eventData.ChannelId)
                .SplitRules.Add(splitRuleCache);
            await _manageCache.SetSplitPackageSettingAsync(eventData.TenantId, tenantSettingCache);
        }

        public async Task HandleEventAsync(EntityUpdatedEventData<SplitRule> eventData)
        {
            //update cache for myself
            var logistic = await this._logisticChannelRepository.GetAll().Include(p => p.LogisticBy).Select(o => o.LogisticBy).FirstAsync(o => o.Id == eventData.Entity.LogisticChannelId);
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(logistic.TenantId);
            var splitRuleCache = settingCache.OwnLogistics.First(o => o.Id == logistic.Id).LogisticChannels
                .First(o => o.Id == eventData.Entity.LogisticChannelId).SplitRules.First(o=>o.Id == eventData.Entity.Id);
            splitRuleCache.RuleName = eventData.Entity.RuleName;
            splitRuleCache.MaxPackage = eventData.Entity.MaxPackage;
            splitRuleCache.MaxWeight = eventData.Entity.MaxWeight;
            splitRuleCache.MaxTax = eventData.Entity.MaxTax;
            splitRuleCache.MaxPrice = eventData.Entity.MaxPrice;
            await this._manageCache.SetSplitPackageSettingAsync(logistic.TenantId, settingCache);
            //associated  import other
            if (!logistic.TenantId.HasValue)
            {
                var importTenantId = await this._tenantLogisticChannelRepository.GetAll()
                    .Where(o => o.LogisticChannelId == eventData.Entity.LogisticChannelId)
                    .Select(o => o.TenantId).Distinct().ToListAsync();
                foreach (var item in importTenantId)
                {
                    await this.EventBus.TriggerAsync(new TenanModifyImportSplitRuleEvent()
                    {
                        TenantId = item,
                        LogisticId = logistic.Id,
                        ChannelId = eventData.Entity.LogisticChannelId,
                        SplitRuleId = eventData.Entity.Id
                    });
                }
            }
        }

        public async Task HandleEventAsync(TenanModifyImportSplitRuleEvent eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(null);
            var tenantSettingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var splitRuleCache = settingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId)
                .LogisticChannels.First(o => o.Id == eventData.ChannelId)
                .SplitRules.First(o => o.Id == eventData.SplitRuleId);
            var tenantSplitRuleCache = tenantSettingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId)
                .LogisticChannels.First(o => o.Id == eventData.ChannelId)
                .SplitRules.First(o => o.Id == eventData.SplitRuleId);
            tenantSplitRuleCache = splitRuleCache;
            await _manageCache.SetSplitPackageSettingAsync(eventData.TenantId, tenantSettingCache);
        }

        public async Task HandleEventAsync(StartUseSplitRuleEvent eventData)
        {
            //update cache for myself
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var splitRuleData = await this._splitRuleRepository.GetAll()
                .Include((SplitRule p) => p.ProductClasses)
                .IgnoreQueryFilters()
                .FirstAsync(o => o.Id == eventData.SplitRuleId);
            var splitRuleCache = this._objectMapper.Map<SplitRuleCacheDto>(splitRuleData);
            settingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId).LogisticChannels.First(o=>o.Id == eventData.ChannelId)
                .SplitRules.Add(splitRuleCache);
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
            //Associated impact other
            if (!eventData.TenantId.HasValue)
            {
                var importTenantId = await this._tenantLogisticChannelRepository.GetAll()
                    .Where(o => o.LogisticChannelId == eventData.ChannelId)
                    .Select(o => o.TenantId).Distinct().ToListAsync();
                foreach (var item in importTenantId)
                {
                    await this.EventBus.TriggerAsync(new TenantStartUseImportSplitRuleEvent()
                    {
                        TenantId = item,
                        LogisticId = eventData.LogisticId,
                        ChannelId = eventData.ChannelId,
                        SplitRuleId = eventData.SplitRuleId
                    });
                }
            }
        }

        public async Task HandleEventAsync(TenantStartUseImportSplitRuleEvent eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(null);
            var tenantSettingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var splitRuleCache = settingCache.OwnLogistics.SelectMany(oi => oi.LogisticChannels)
                .First(oi => oi.Id == eventData.ChannelId).SplitRules.First(o=>o.Id == eventData.SplitRuleId);
            tenantSettingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId).LogisticChannels.First(o=>o.Id == eventData.ChannelId)
                .SplitRules.Add(splitRuleCache);
            await _manageCache.SetSplitPackageSettingAsync(eventData.TenantId, tenantSettingCache);
        }

        public async Task HandleEventAsync(BanishSplitRuleEvent eventData)
        {
            //update cache for myself
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var splitRuleCache = settingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId).LogisticChannels
                .First(o => o.Id == eventData.ChannelId).SplitRules.First(o => o.Id == eventData.SplitRuleId);
            settingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId).LogisticChannels
                .First(o => o.Id == eventData.ChannelId).SplitRules
                .Remove(splitRuleCache);
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
            //associated  import other
            if (!eventData.TenantId.HasValue)
            {
                var importTenantId = await this._tenantLogisticChannelRepository.GetAll()
                    .Where(o => o.LogisticChannelId == eventData.ChannelId)
                    .Select(o => o.TenantId).Distinct().ToListAsync();
                foreach (var item in importTenantId)
                {
                    await this.EventBus.TriggerAsync(new TenantBanishImportSplitRuleEvent()
                    {
                        TenantId = item,
                        LogisticId = eventData.LogisticId,
                        ChannelId = eventData.ChannelId,
                        SplitRuleId = eventData.SplitRuleId
                    });
                }
            }
        }

        public async Task HandleEventAsync(TenantBanishImportSplitRuleEvent eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var splitRuleCache = settingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId).LogisticChannels
                .First(o => o.Id == eventData.ChannelId).SplitRules.First(o => o.Id == eventData.SplitRuleId);
            settingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId).LogisticChannels
                .First(o => o.Id == eventData.ChannelId).SplitRules
                .Remove(splitRuleCache);
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
        }
        #endregion

        #region splitruleitem
        public async Task HandleEventAsync(EntityCreatedEventData<SplitRuleProductClass> eventData)
        {
            //update cache for myself
            var splitRuleItem = await this._splitRuleProductClassRepository.GetAll()
                .Where(o => o.Id == eventData.Entity.Id)
                .Select(o => new
                {
                    TenantId = o.SplitRuleBy.LogisticChannelBy.TenantId,
                    LogisticId = o.SplitRuleBy.LogisticChannelBy.LogisticId,
                    LogisticChannelId = o.SplitRuleBy.LogisticChannelId,
                    SplitRuleId = o.SplitRuleId
                }).FirstAsync();
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(splitRuleItem.TenantId);
            settingCache.OwnLogistics.First(o => o.Id == splitRuleItem.LogisticId).LogisticChannels
                .First(o => o.Id == splitRuleItem.LogisticChannelId).SplitRules
                .First(o=>o.Id == splitRuleItem.SplitRuleId).ProductClasses
                .Add(this._objectMapper.Map<SplitRuleProductClassCacheDto>(eventData.Entity));
            await this._manageCache.SetSplitPackageSettingAsync(splitRuleItem.TenantId, settingCache);
            //associated  import other
            if (!splitRuleItem.TenantId.HasValue)
            {
                var importTenantId = await this._tenantLogisticChannelRepository.GetAll()
                    .Where(o => o.LogisticChannelId == splitRuleItem.LogisticChannelId)
                    .Select(o => o.TenantId).Distinct().ToListAsync();
                foreach (var item in importTenantId)
                {
                    await this.EventBus.TriggerAsync(new TenantCreateImportSplitRuleItemEvent()
                    {
                        TenantId = item,
                        LogisticId = splitRuleItem.LogisticId,
                        ChannelId = splitRuleItem.LogisticChannelId,
                        SplitRuleId = splitRuleItem.SplitRuleId,
                        SplitRuleItemId = eventData.Entity.Id
                    });
                }
            }
        }

        public async Task HandleEventAsync(TenantCreateImportSplitRuleItemEvent eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(null);
            var tenantSettingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var splitRuleItemCache = settingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId)
                .LogisticChannels.First(o => o.Id == eventData.ChannelId)
                .SplitRules.First(o => o.Id == eventData.SplitRuleId)
                .ProductClasses.First(o=>o.Id == eventData.SplitRuleItemId);
            tenantSettingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId)
                .LogisticChannels.First(o => o.Id == eventData.ChannelId)
                .SplitRules.First(o => o.Id == eventData.SplitRuleId)
                .ProductClasses.Add(splitRuleItemCache);
            await _manageCache.SetSplitPackageSettingAsync(eventData.TenantId, tenantSettingCache);
        }

        public async Task HandleEventAsync(EntityUpdatedEventData<SplitRuleProductClass> eventData)
        {
            //update cache for myself
            var splitRuleItem = await this._splitRuleProductClassRepository.GetAll()
                .Where(o => o.Id == eventData.Entity.Id)
                .Select(o => new
                {
                    TenantId = o.SplitRuleBy.LogisticChannelBy.TenantId,
                    LogisticId = o.SplitRuleBy.LogisticChannelBy.LogisticId,
                    LogisticChannelId = o.SplitRuleBy.LogisticChannelId,
                    SplitRuleId = o.SplitRuleId
                }).FirstAsync();
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(splitRuleItem.TenantId);
            var splitRuleItemCache = settingCache.OwnLogistics.First(o => o.Id == splitRuleItem.LogisticId)
                .LogisticChannels.First(o => o.Id == splitRuleItem.LogisticChannelId)
                .SplitRules.First(o => o.Id == splitRuleItem.SplitRuleId)
                .ProductClasses.First(o=>o.Id == eventData.Entity.Id);
            splitRuleItemCache.PTId = eventData.Entity.PTId;
            splitRuleItemCache.MinNum = eventData.Entity.MinNum;
            splitRuleItemCache.MaxNum = eventData.Entity.MaxNum;
            await this._manageCache.SetSplitPackageSettingAsync(splitRuleItem.TenantId, settingCache);
            //associated  import other
            if (!splitRuleItem.TenantId.HasValue)
            {
                var importTenantId = await this._tenantLogisticChannelRepository.GetAll()
                    .Where(o => o.LogisticChannelId == splitRuleItem.LogisticChannelId)
                    .Select(o => o.TenantId).Distinct().ToListAsync();
                foreach (var item in importTenantId)
                {
                    await this.EventBus.TriggerAsync(new TenanModifyImportSplitRuleItemEvent()
                    {
                        TenantId = item,
                        LogisticId = splitRuleItem.LogisticId,
                        ChannelId = splitRuleItem.LogisticChannelId,
                        SplitRuleId = splitRuleItem.SplitRuleId,
                        SplitRuleItemId = eventData.Entity.Id
                    });
                }
            }
        }

        public async Task HandleEventAsync(TenanModifyImportSplitRuleItemEvent eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(null);
            var tenantSettingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var splitRuleItemCache = settingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId)
                .LogisticChannels.First(o => o.Id == eventData.ChannelId)
                .SplitRules.First(o => o.Id == eventData.SplitRuleId)
                .ProductClasses.First(o => o.Id == eventData.SplitRuleItemId);
            var tenantSplitRuleItemCache = tenantSettingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId)
                .LogisticChannels.First(o => o.Id == eventData.ChannelId)
                .SplitRules.First(o => o.Id == eventData.SplitRuleId)
                .ProductClasses.First(o => o.Id == eventData.SplitRuleItemId);
            tenantSplitRuleItemCache = splitRuleItemCache;
            await _manageCache.SetSplitPackageSettingAsync(eventData.TenantId, tenantSettingCache);
        }

        public async Task HandleEventAsync(BanishSplitRuleItemEvent eventData)
        {
            //update cache for myself
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var splitRuleItemCache = settingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId)
                .LogisticChannels.First(o => o.Id == eventData.ChannelId)
                .SplitRules.First(o => o.Id == eventData.SplitRuleId)
                .ProductClasses.First(o => o.Id == eventData.SplitRuleItemId);
            settingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId)
                .LogisticChannels.First(o => o.Id == eventData.ChannelId)
                .SplitRules.First(o => o.Id == eventData.SplitRuleId)
                .ProductClasses.Remove(splitRuleItemCache);
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
            //associated  import other
            if (!eventData.TenantId.HasValue)
            {
                var importTenantId = await this._tenantLogisticChannelRepository.GetAll()
                    .Where(o => o.LogisticChannelId == eventData.ChannelId)
                    .Select(o => o.TenantId).Distinct().ToListAsync();
                foreach (var item in importTenantId)
                {
                    await this.EventBus.TriggerAsync(new BanishTenantSplitRuleItemEvent()
                    {
                        TenantId = item,
                        LogisticId = eventData.LogisticId,
                        ChannelId = eventData.ChannelId,
                        SplitRuleId = eventData.SplitRuleId,
                        SplitRuleItemId = eventData.SplitRuleItemId
                    });
                }
            }
        }

        public async Task HandleEventAsync(BanishTenantSplitRuleItemEvent eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var splitRuleItemCache = settingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId)
                .LogisticChannels.First(o => o.Id == eventData.ChannelId)
                .SplitRules.First(o => o.Id == eventData.SplitRuleId)
                .ProductClasses.First(o => o.Id == eventData.SplitRuleItemId);
            settingCache.OwnLogistics.First(o => o.Id == eventData.LogisticId)
                .LogisticChannels.First(o => o.Id == eventData.ChannelId)
                .SplitRules.First(o => o.Id == eventData.SplitRuleId)
                .ProductClasses.Remove(splitRuleItemCache);
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
        }
        #endregion
    }
}
