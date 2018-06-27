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
        IAsyncEventHandler<CreateLogisticEvent>,
        IAsyncEventHandler<ModifyLogisticEvent>,
        IAsyncEventHandler<TenantModifyImportLogisticEvent>,
        IAsyncEventHandler<StartUseLogisticEvent>,
        IAsyncEventHandler<BanishLogisticEvent>,
        IAsyncEventHandler<TenantStartUseImportLogisticEvent>,
        IAsyncEventHandler<TenantBanishImportLogisticEvent>,

        IAsyncEventHandler<CreateChannelEvent>,
        IAsyncEventHandler<ModifyChannelEvent>,
        IAsyncEventHandler<TenantModifyImportChannelEvent>,
        IAsyncEventHandler<StartUseChannelEvent>,
        IAsyncEventHandler<TenantStartUseImportChannelEvent>,
        IAsyncEventHandler<BanishChannelEvent>,
        IAsyncEventHandler<TenantBanishImportChannelEvent>,
        IAsyncEventHandler<TenantImportChannelEvent>,

        IAsyncEventHandler<CreateSplitRuleEvent>,
        IAsyncEventHandler<TenantCreateImportSplitRuleEvent>,
        IAsyncEventHandler<ModifyImportSplitRuleEvent>,
        IAsyncEventHandler<TenanModifyImportSplitRuleEvent>,
        IAsyncEventHandler<StartUseSplitRuleEvent>,
        IAsyncEventHandler<TenantStartUseImportSplitRuleEvent>,
        IAsyncEventHandler<BanishSplitRuleEvent>,
        IAsyncEventHandler<TenantBanishImportSplitRuleEvent>,

        IAsyncEventHandler<CreateSplitRuleItemEvent>,
        IAsyncEventHandler<TenantCreateImportSplitRuleItemEvent>,
        IAsyncEventHandler<ModifySplitRuleItemEvent>,
        IAsyncEventHandler<TenanModifyImportSplitRuleItemEvent>,
        IAsyncEventHandler<BanishSplitRuleItemEvent>,
        IAsyncEventHandler<BanishTenantSplitRuleItemEvent>,

        IAsyncEventHandler<CreateLogisticRelation>,
        IAsyncEventHandler<ModifyLogisticRelation>,
        IAsyncEventHandler<BanishLogisticRelation>,

        ITransientDependency
    {
        private readonly ManageCache _manageCache;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        public IObjectMapper _objectMapper;
        private readonly IEventBus _eventBus;
        private readonly IRepository<Business.Logistic, long> _logisticRepository;
        private readonly IRepository<LogisticChannel, long> _logisticChannelRepository;
        private readonly IRepository<TenantLogisticChannel, long> _tenantLogisticChannelRepository;
        private readonly IRepository<SplitRule, long> _splitRuleRepository;
        private readonly IRepository<SplitRuleItem, long> _splitRuleProductClassRepository;

        public Handler(ManageCache manageCache,
            IUnitOfWorkManager unitOfWorkManager,
            IObjectMapper objectMapper,
            IEventBus eventBus,
            IRepository<Business.Logistic, long> logisticRepository,
            IRepository<LogisticChannel, long> logisticChannelRepository,
            IRepository<TenantLogisticChannel, long> tenantLogisticChannelRepository,
            IRepository<SplitRule, long> splitRuleRepository,
            IRepository<SplitRuleItem, long> splitRuleProductClassRepository)
        {
            this._manageCache = manageCache;
            this._unitOfWorkManager = unitOfWorkManager;
            this._objectMapper = objectMapper;
            this._eventBus = eventBus;
            this._logisticRepository = logisticRepository;
            this._logisticChannelRepository = logisticChannelRepository;
            this._tenantLogisticChannelRepository = tenantLogisticChannelRepository;
            this._splitRuleRepository = splitRuleRepository;
            this._splitRuleProductClassRepository = splitRuleProductClassRepository;
        }

        #region logistic
        public virtual async Task HandleEventAsync(CreateLogisticEvent eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticCache = this._objectMapper.Map<LogisticCacheDto>(eventData);
            logisticCache.LogisticChannels = new List<ChannelCacheDto>();
            settingCache.OwnLogistics.Add(logisticCache);
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
        }

        public virtual async Task HandleEventAsync(ModifyLogisticEvent eventData)
        {
            if (!eventData.IsActive)
            {
                return;
            }
            //update cache for myself
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticCache = settingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.Id);
            if (logisticCache == null)
            {
                return;
            }
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
                    await this._eventBus.TriggerAsync(new TenantModifyImportLogisticEvent()
                    {
                        TenantId = item,
                        LogisticId = eventData.Id
                    });
                }
            }
        }

        public virtual async Task HandleEventAsync(TenantModifyImportLogisticEvent eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(null);
            var tenantSettingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticCache = settingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (logisticCache == null)
            {
                return;
            }
            var tenantLogisticCache = tenantSettingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (tenantLogisticCache == null)
            {
                return;
            }
            tenantLogisticCache.CorporationName = logisticCache.CorporationName;
            tenantLogisticCache.CorporationUrl = logisticCache.CorporationUrl;
            tenantLogisticCache.LogoURL = logisticCache.LogoURL;
            tenantLogisticCache.LogisticCode = logisticCache.LogisticCode;
            await _manageCache.SetSplitPackageSettingAsync(eventData.TenantId, tenantSettingCache);
        }

        public virtual async Task HandleEventAsync(StartUseLogisticEvent eventData)
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
                    await this._eventBus.TriggerAsync(new TenantStartUseImportLogisticEvent() {
                        TenantId = item,
                        LogisticId = eventData.LogisticId
                    });
                }
            }
        }

        public virtual async Task HandleEventAsync(TenantStartUseImportLogisticEvent eventData)
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
            var logisticCache = settingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (logisticCache == null)
            {
                return;
            }
            logisticCache.LogisticChannels = tenantChannelSet.ToList();
            tenantSettingCache.OwnLogistics.Add(logisticCache);
            await _manageCache.SetSplitPackageSettingAsync(eventData.TenantId, tenantSettingCache);
        }

        public virtual async Task HandleEventAsync(BanishLogisticEvent eventData)
        {
            //update cache for myself
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticCache = settingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (logisticCache == null)
            {
                return;
            }
            settingCache.OwnLogistics.Remove(logisticCache);
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
            //associated impact other
            if (!eventData.TenantId.HasValue)
            {
                var importTenantId = await this._tenantLogisticChannelRepository.GetAll()
                    .Where(o => o.LogisticChannelBy.LogisticBy.Id == eventData.LogisticId)
                    .Select(o => o.TenantId).Distinct().ToListAsync();
                foreach (var item in importTenantId)
                {
                    await this._eventBus.TriggerAsync(new TenantBanishImportLogisticEvent()
                    {
                        TenantId = item,
                        LogisticId = eventData.LogisticId
                    });
                }
            }
        }

        public virtual async Task HandleEventAsync(TenantBanishImportLogisticEvent eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticCache = settingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (logisticCache == null)
            {
                return;
            }
            settingCache.OwnLogistics.Remove(logisticCache);
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
        }
        #endregion

        #region channel
        public virtual async Task HandleEventAsync(CreateChannelEvent eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticCache = settingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (logisticCache == null)
            {
                return;
            }
            else
            {
                logisticCache.LogisticChannels.Add(this._objectMapper.Map<ChannelCacheDto>(eventData));
                await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
            }
        }

        public virtual async Task HandleEventAsync(ModifyChannelEvent eventData)
        {
            if (!eventData.IsActive)
            {
                return;
            }
            //update cache for myself
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticCache = settingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (logisticCache == null)
            {
                return;
            }
            var channelCache = logisticCache.LogisticChannels.FirstOrDefault(o => o.Id == eventData.Id);
            if (channelCache == null)
            {
                return;
            }
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
                    await this._eventBus.TriggerAsync(new TenantModifyImportChannelEvent()
                    {
                        TenantId = item,
                        LogisticId = eventData.LogisticId,
                        LogisticChannelId = eventData.Id
                    });
                }
            }
        }

        public virtual async Task HandleEventAsync(TenantModifyImportChannelEvent eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(null);
            var tenantSettingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticCache = settingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (logisticCache == null)
            {
                return;
            }
            var channelCache = logisticCache.LogisticChannels.FirstOrDefault(o => o.Id == eventData.LogisticChannelId);
            if (channelCache == null)
            {
                return;
            }
            var tenantLogisticCache = tenantSettingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (tenantLogisticCache == null)
            {
                return;
            }
            var tenantChannelCache = tenantLogisticCache.LogisticChannels.FirstOrDefault(o => o.Id == eventData.LogisticChannelId);
            if (tenantChannelCache == null)
            {
                return;
            }
            var importRelated = await this._tenantLogisticChannelRepository.SingleAsync(o => o.TenantId == eventData.TenantId && o.LogisticChannelId == eventData.LogisticChannelId);
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

        public virtual async Task HandleEventAsync(StartUseChannelEvent eventData)
        {
            //update cache for myself
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var channelData = await this._logisticChannelRepository.GetAll()
                .Include(p => p.WeightFreights)
                .Include(p => p.NumFreights)
                .Include(p => p.SplitRules).ThenInclude((SplitRule p) => p.ProductClasses)
                .IgnoreQueryFilters()
                .FirstAsync(o => o.TenantId == eventData.TenantId && o.Id == eventData.LogisticChannelId);
            var channelCache = this._objectMapper.Map<ChannelCacheDto>(channelData);
            var logisticCache = settingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (logisticCache == null)
            {
                return;
            }
            logisticCache.LogisticChannels.Add(channelCache);
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
            //Associated impact other
            if (!eventData.TenantId.HasValue)
            {
                var importTenantId = await this._tenantLogisticChannelRepository.GetAll()
                    .Where(o => o.LogisticChannelId == eventData.LogisticChannelId)
                    .Select(o => o.TenantId).Distinct().ToListAsync();
                foreach (var item in importTenantId)
                {
                    await this._eventBus.TriggerAsync(new TenantStartUseImportChannelEvent()
                    {
                        TenantId = item,
                        LogisticId = eventData.LogisticId,
                        LogisticChannelId = eventData.LogisticChannelId
                    });
                }
            }
        }

        public virtual async Task HandleEventAsync(TenantStartUseImportChannelEvent eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(null);
            var tenantSettingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var importRelated = await this._tenantLogisticChannelRepository.GetAll().IgnoreQueryFilters()
                .SingleAsync(o => o.LogisticChannelId == eventData.LogisticChannelId && o.TenantId == eventData.TenantId);
            var channelCache = settingCache.OwnLogistics.SelectMany(oi => oi.LogisticChannels).First(oi => oi.Id == eventData.LogisticChannelId);
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
            var tenantLogisticCache = tenantSettingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (tenantLogisticCache == null)
            {
                return;
            }
            tenantLogisticCache.LogisticChannels.Add(channelCache);
            await _manageCache.SetSplitPackageSettingAsync(eventData.TenantId, tenantSettingCache);
        }

        public virtual async Task HandleEventAsync(BanishChannelEvent eventData)
        {
            //update cache for myself
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticCache = settingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (logisticCache == null)
            {
                return;
            }
            var channelCache = logisticCache.LogisticChannels.FirstOrDefault(o=>o.Id == eventData.LogisticChannelId);
            if (channelCache == null)
            {
                return;
            }
            logisticCache.LogisticChannels.Remove(channelCache);
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
            //associated impact other
            if (!eventData.TenantId.HasValue)
            {
                var importTenantId = await this._tenantLogisticChannelRepository.GetAll()
                    .Where(o => o.LogisticChannelId == eventData.LogisticChannelId)
                    .Select(o => o.TenantId).Distinct().ToListAsync();
                foreach (var item in importTenantId)
                {
                    await this._eventBus.TriggerAsync(new TenantBanishImportChannelEvent()
                    {
                        TenantId = item,
                        LogisticId = eventData.LogisticId,
                        LogisticChannelId = eventData.LogisticChannelId
                    });
                }
            }
        }

        public virtual async Task HandleEventAsync(TenantBanishImportChannelEvent eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticCache = settingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (logisticCache == null)
            {
                return;
            }
            var channelCache = logisticCache.LogisticChannels.FirstOrDefault(o => o.Id == eventData.LogisticChannelId);
            if (channelCache == null)
            {
                return;
            }
            logisticCache.LogisticChannels.Remove(channelCache);
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
        }

        public virtual async Task HandleEventAsync(TenantImportChannelEvent eventData)
        {
            var channelsSet = await this._logisticChannelRepository.GetAll().IgnoreQueryFilters()
                .Where(o => eventData.RemoveChannelIds.Union(eventData.AddChannelIds).Contains(o.Id) && !o.IsDeleted)
                .Select(o=>new {
                    o.LogisticId,
                    ChannelId = o.Id
                }).ToListAsync();
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(null);
            var tenantSettingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            foreach (var item in eventData.AddChannelIds)
            {
                var channel = channelsSet.First(o => o.ChannelId == item);
                var logisticCache = settingCache.OwnLogistics.FirstOrDefault(o => o.Id == channel.LogisticId);
                if (logisticCache == null)
                {
                    return;
                }
                var channelCache = logisticCache.LogisticChannels.FirstOrDefault(o => o.Id == channel.ChannelId);
                if (channelCache == null)
                {
                    return;
                }
                if (tenantSettingCache == null)
                {
                    tenantSettingCache = new SplitPackageSettingCache() {
                        OwnLogistics = new List<LogisticCacheDto>(),
                        Relateds = new List<LogisticRelatedCacheDto>()
                    };
                }
                var tenantLogisticCache = tenantSettingCache.OwnLogistics.FirstOrDefault(o => o.Id == channel.LogisticId);
                if (tenantLogisticCache == null)
                {
                    tenantSettingCache.OwnLogistics.Add(new LogisticCacheDto()
                    {
                        Id = logisticCache.Id,
                        CorporationName = logisticCache.CorporationName,
                        CorporationUrl = logisticCache.CorporationUrl,
                        LogoURL = logisticCache.LogoURL,
                        LogisticCode = logisticCache.LogisticCode,
                        LogisticChannels = new List<ChannelCacheDto>() { channelCache }
                    });
                }
                else
                {
                    tenantLogisticCache.LogisticChannels.Add(channelCache);
                }
            }
            foreach (var item in eventData.RemoveChannelIds)
            {
                var channel = channelsSet.First(o => o.ChannelId == item);
                var tenantChannelCache = tenantSettingCache.OwnLogistics.First(o => o.Id == channel.LogisticId).LogisticChannels.First(o => o.Id == channel.ChannelId);
                tenantSettingCache.OwnLogistics.First(o => o.Id == channel.LogisticId).LogisticChannels.Remove(tenantChannelCache);
            }
            await _manageCache.SetSplitPackageSettingAsync(eventData.TenantId, tenantSettingCache);
            Abp.Logging.LogHelper.Logger.Debug(string.Format("{1}导入物流渠道:{0}",tenantSettingCache.OwnLogistics.Count,eventData.TenantId));
        }
        #endregion

        #region splitrule
        public virtual async Task HandleEventAsync(CreateSplitRuleEvent eventData)
        {
            //update cache for myself
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticCache = settingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (logisticCache == null)
            {
                return;
            }
            var ChannelCache = logisticCache.LogisticChannels.FirstOrDefault(o => o.Id == eventData.LogisticChannelId);
            if (ChannelCache == null)
            {
                return;
            }
            ChannelCache.SplitRules.Add(this._objectMapper.Map<SplitRuleCacheDto>(eventData));
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
            //associated  import other
            if (!eventData.TenantId.HasValue)
            {
                var importTenantId = await this._tenantLogisticChannelRepository.GetAll()
                    .Where(o => o.LogisticChannelId == eventData.LogisticChannelId)
                    .Select(o => o.TenantId).Distinct().ToListAsync();
                foreach (var item in importTenantId)
                {
                    await this._eventBus.TriggerAsync(new TenantCreateImportSplitRuleEvent()
                    {
                        TenantId = item,
                        LogisticId = eventData.LogisticId,
                        LogisticChannelId = eventData.LogisticChannelId,
                        SplitRuleId = eventData.Id
                    });
                }
            }
        }

        public virtual async Task HandleEventAsync(TenantCreateImportSplitRuleEvent eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(null);
            var tenantSettingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticCache = settingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (logisticCache == null)
            {
                return;
            }
            var channelCache = logisticCache.LogisticChannels.FirstOrDefault(o => o.Id == eventData.LogisticChannelId);
            if (channelCache == null)
            {
                return;
            }
            var splitRuleCache = channelCache.SplitRules.FirstOrDefault(o => o.Id == eventData.SplitRuleId);
            if (splitRuleCache == null)
            {
                return;
            }
            var tenantLogisticCache = tenantSettingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (tenantLogisticCache == null)
            {
                return;
            }
            var tenantChannelCache = tenantLogisticCache.LogisticChannels.FirstOrDefault(o => o.Id == eventData.LogisticChannelId);
            if (tenantChannelCache == null)
            {
                return;
            }
            tenantChannelCache.SplitRules.Add(splitRuleCache);
            await _manageCache.SetSplitPackageSettingAsync(eventData.TenantId, tenantSettingCache);
        }

        public virtual async Task HandleEventAsync(ModifyImportSplitRuleEvent eventData)
        {
            //update cache for myself
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticCache = settingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (logisticCache == null)
            {
                return;
            }
            var channelCache = logisticCache.LogisticChannels.FirstOrDefault(o => o.Id == eventData.LogisticChannelId);
            if (channelCache == null)
            {
                return;
            }
            var splitRuleCache = channelCache.SplitRules.FirstOrDefault(o => o.Id == eventData.SplitRuleId);
            if (splitRuleCache == null)
            {
                return;
            }
            splitRuleCache.RuleName = eventData.RuleName;
            splitRuleCache.MaxPackage = eventData.MaxPackage;
            splitRuleCache.MaxWeight = eventData.MaxWeight;
            splitRuleCache.MaxTax = eventData.MaxTax;
            splitRuleCache.MaxPrice = eventData.MaxPrice;
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
            //associated  import other
            if (!eventData.TenantId.HasValue)
            {
                var importTenantId = await this._tenantLogisticChannelRepository.GetAll()
                    .Where(o => o.LogisticChannelId == eventData.LogisticChannelId)
                    .Select(o => o.TenantId).Distinct().ToListAsync();
                foreach (var item in importTenantId)
                {
                    await this._eventBus.TriggerAsync(new TenanModifyImportSplitRuleEvent()
                    {
                        TenantId = item,
                        LogisticId = eventData.LogisticId,
                        LogisticChannelId = eventData.LogisticChannelId,
                        SplitRuleId = eventData.SplitRuleId
                    });
                }
            }
        }

        public virtual async Task HandleEventAsync(TenanModifyImportSplitRuleEvent eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(null);
            var tenantSettingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticCache = settingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (logisticCache == null)
            {
                return;
            }
            var channelCache = logisticCache.LogisticChannels.FirstOrDefault(o => o.Id == eventData.LogisticChannelId);
            if (channelCache == null)
            {
                return;
            }
            var splitRuleCache = channelCache.SplitRules.FirstOrDefault(o => o.Id == eventData.SplitRuleId);
            if (splitRuleCache == null)
            {
                return;
            }
            var tenantLogisticCache = tenantSettingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (tenantLogisticCache == null)
            {
                return;
            }
            var tenantChannelCache = tenantLogisticCache.LogisticChannels.FirstOrDefault(o => o.Id == eventData.LogisticChannelId);
            if (tenantChannelCache == null)
            {
                return;
            }
            var tenantSplitRuleCache = tenantChannelCache.SplitRules.FirstOrDefault(o => o.Id == eventData.SplitRuleId);
            if (tenantSplitRuleCache == null)
            {
                return;
            }
            tenantSplitRuleCache = splitRuleCache;
            await _manageCache.SetSplitPackageSettingAsync(eventData.TenantId, tenantSettingCache);
        }

        public virtual async Task HandleEventAsync(StartUseSplitRuleEvent eventData)
        {
            //update cache for myself
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var splitRuleData = await this._splitRuleRepository.GetAll()
                .Include((SplitRule p) => p.ProductClasses)
                .IgnoreQueryFilters()
                .FirstAsync(o => o.Id == eventData.SplitRuleId);
            var splitRuleCache = this._objectMapper.Map<SplitRuleCacheDto>(splitRuleData);
            var logisticCache = settingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (logisticCache == null)
            {
                return;
            }
            var channelCache = logisticCache.LogisticChannels.FirstOrDefault(o => o.Id == eventData.LogisticChannelId);
            if (channelCache == null)
            {
                return;
            }
            channelCache.SplitRules.Add(splitRuleCache);
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
            //Associated impact other
            if (!eventData.TenantId.HasValue)
            {
                var importTenantId = await this._tenantLogisticChannelRepository.GetAll()
                    .Where(o => o.LogisticChannelId == eventData.LogisticChannelId)
                    .Select(o => o.TenantId).Distinct().ToListAsync();
                foreach (var item in importTenantId)
                {
                    await this._eventBus.TriggerAsync(new TenantStartUseImportSplitRuleEvent()
                    {
                        TenantId = item,
                        LogisticId = eventData.LogisticId,
                        LogisticChannelId = eventData.LogisticChannelId,
                        SplitRuleId = eventData.SplitRuleId
                    });
                }
            }
        }

        public virtual async Task HandleEventAsync(TenantStartUseImportSplitRuleEvent eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(null);
            var tenantSettingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticCache = settingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (logisticCache == null)
            {
                return;
            }
            var channelCache = logisticCache.LogisticChannels.FirstOrDefault(o => o.Id == eventData.LogisticChannelId);
            if (channelCache == null)
            {
                return;
            }
            var splitRuleCache = channelCache.SplitRules.FirstOrDefault(o=>o.Id == eventData.SplitRuleId);
            if (splitRuleCache == null)
            {
                return;
            }
            var tenantLogisticCache = tenantSettingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (tenantLogisticCache == null)
            {
                return;
            }
            var tenantChannelCache = tenantLogisticCache.LogisticChannels.FirstOrDefault(o => o.Id == eventData.LogisticChannelId);
            if (tenantChannelCache == null)
            {
                return;
            }
            tenantChannelCache.SplitRules.Add(splitRuleCache);
            await _manageCache.SetSplitPackageSettingAsync(eventData.TenantId, tenantSettingCache);
        }

        public virtual async Task HandleEventAsync(BanishSplitRuleEvent eventData)
        {
            //update cache for myself
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticCache = settingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (logisticCache == null)
            {
                return;
            }
            var channelCache = logisticCache.LogisticChannels.FirstOrDefault(o => o.Id == eventData.LogisticChannelId);
            if (channelCache == null)
            {
                return;
            }
            var splitRuleCache = channelCache.SplitRules.FirstOrDefault(o => o.Id == eventData.SplitRuleId);
            if (splitRuleCache == null)
            {
                return;
            }
            channelCache.SplitRules.Remove(splitRuleCache);
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
            //associated  import other
            if (!eventData.TenantId.HasValue)
            {
                var importTenantId = await this._tenantLogisticChannelRepository.GetAll()
                    .Where(o => o.LogisticChannelId == eventData.LogisticChannelId)
                    .Select(o => o.TenantId).Distinct().ToListAsync();
                foreach (var item in importTenantId)
                {
                    await this._eventBus.TriggerAsync(new TenantBanishImportSplitRuleEvent()
                    {
                        TenantId = item,
                        LogisticId = eventData.LogisticId,
                        LogisticChannelId = eventData.LogisticChannelId,
                        SplitRuleId = eventData.SplitRuleId
                    });
                }
            }
        }

        public virtual async Task HandleEventAsync(TenantBanishImportSplitRuleEvent eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticCache = settingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (logisticCache == null)
            {
                return;
            }
            var channelCache = logisticCache.LogisticChannels.FirstOrDefault(o => o.Id == eventData.LogisticChannelId);
            if (channelCache == null)
            {
                return;
            }
            var splitRuleCache = channelCache.SplitRules.FirstOrDefault(o => o.Id == eventData.SplitRuleId);
            if (splitRuleCache == null)
            {
                return;
            }
            channelCache.SplitRules.Remove(splitRuleCache);
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
        }
        #endregion

        #region splitruleitem
        public virtual async Task HandleEventAsync(CreateSplitRuleItemEvent eventData)
        {
            //update cache for myself
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticCache = settingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (logisticCache == null)
            {
                return;
            }
            var channelCache = logisticCache.LogisticChannels.FirstOrDefault(o => o.Id == eventData.LogisticChannelId);
            if (channelCache == null)
            {
                return;
            }
            var splitRuleCache = channelCache.SplitRules.FirstOrDefault(o => o.Id == eventData.SplitRuleId);
            if (splitRuleCache == null)
            {
                return;
            }
            splitRuleCache.ProductClasses.Add(this._objectMapper.Map<SplitRuleProductClassCacheDto>(eventData));
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
            //associated  import other
            if (!eventData.TenantId.HasValue)
            {
                var importTenantId = await this._tenantLogisticChannelRepository.GetAll()
                    .Where(o => o.LogisticChannelId == eventData.LogisticChannelId)
                    .Select(o => o.TenantId).Distinct().ToListAsync();
                foreach (var item in importTenantId)
                {
                    await this._eventBus.TriggerAsync(new TenantCreateImportSplitRuleItemEvent()
                    {
                        TenantId = item,
                        LogisticId = eventData.LogisticId,
                        LogisticChannelId = eventData.LogisticChannelId,
                        SplitRuleId = eventData.SplitRuleId,
                        SplitRuleItemId = eventData.Id,
                        Type = eventData.Type
                    });
                }
            }
        }

        public virtual async Task HandleEventAsync(TenantCreateImportSplitRuleItemEvent eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(null);
            var tenantSettingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticCache = settingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (logisticCache == null)
            {
                return;
            }
            var channelCache = logisticCache.LogisticChannels.FirstOrDefault(o => o.Id == eventData.LogisticChannelId);
            if (channelCache == null)
            {
                return;
            }
            var splitRuleCache = channelCache.SplitRules.FirstOrDefault(o => o.Id == eventData.SplitRuleId);
            if (splitRuleCache == null)
            {
                return;
            }
            var splitRuleItemCache = splitRuleCache.ProductClasses.FirstOrDefault(o=>o.Id == eventData.SplitRuleItemId);
            if (splitRuleCache == null)
            {
                return;
            }
            var tenantLogisticCache = tenantSettingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (tenantLogisticCache == null)
            {
                return;
            }
            var tenantChannelCache = tenantLogisticCache.LogisticChannels.FirstOrDefault(o => o.Id == eventData.LogisticChannelId);
            if (channelCache == null)
            {
                return;
            }
            var tenantSplitRuleCache = tenantChannelCache.SplitRules.FirstOrDefault(o => o.Id == eventData.SplitRuleId);
            if (tenantSplitRuleCache == null)
            {
                return;
            }
            tenantSplitRuleCache.ProductClasses.Add(splitRuleItemCache);
            await _manageCache.SetSplitPackageSettingAsync(eventData.TenantId, tenantSettingCache);
        }

        public virtual async Task HandleEventAsync(ModifySplitRuleItemEvent eventData)
        {
            //update cache for myself
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticCache = settingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (logisticCache == null)
            {
                return;
            }
            var channelCache = logisticCache.LogisticChannels.FirstOrDefault(o => o.Id == eventData.LogisticChannelId);
            if (channelCache == null)
            {
                return;
            }
            var splitRuleCache = channelCache.SplitRules.FirstOrDefault(o => o.Id == eventData.SplitRuleId);
            if (splitRuleCache == null)
            {
                return;
            }
            var splitRuleItemCache = splitRuleCache.ProductClasses.FirstOrDefault(o => o.Id == eventData.Id);
            if (splitRuleCache == null)
            {
                return;
            }
            splitRuleItemCache.PTId = eventData.PTId;
            splitRuleItemCache.MinNum = eventData.MinNum;
            splitRuleItemCache.MaxNum = eventData.MaxNum;
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
            //associated  import other
            if (!eventData.TenantId.HasValue)
            {
                var importTenantId = await this._tenantLogisticChannelRepository.GetAll()
                    .Where(o => o.LogisticChannelId == eventData.LogisticChannelId)
                    .Select(o => o.TenantId).Distinct().ToListAsync();
                foreach (var item in importTenantId)
                {
                    await this._eventBus.TriggerAsync(new TenanModifyImportSplitRuleItemEvent()
                    {
                        TenantId = item,
                        LogisticId = eventData.LogisticId,
                        LogisticChannelId = eventData.LogisticChannelId,
                        SplitRuleId = eventData.SplitRuleId,
                        SplitRuleItemId = eventData.Id
                    });
                }
            }
        }

        public virtual async Task HandleEventAsync(TenanModifyImportSplitRuleItemEvent eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(null);
            var tenantSettingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticCache = settingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (logisticCache == null)
            {
                return;
            }
            var channelCache = logisticCache.LogisticChannels.FirstOrDefault(o => o.Id == eventData.LogisticChannelId);
            if (channelCache == null)
            {
                return;
            }
            var splitRuleCache = channelCache.SplitRules.FirstOrDefault(o => o.Id == eventData.SplitRuleId);
            if (splitRuleCache == null)
            {
                return;
            }
            var splitRuleItemCache = splitRuleCache.ProductClasses.FirstOrDefault(o => o.Id == eventData.SplitRuleItemId);
            if (splitRuleCache == null)
            {
                return;
            }
            var tenantLogisticCache = tenantSettingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (tenantLogisticCache == null)
            {
                return;
            }
            var tenantChannelCache = tenantLogisticCache.LogisticChannels.FirstOrDefault(o => o.Id == eventData.LogisticChannelId);
            if (tenantChannelCache == null)
            {
                return;
            }
            var tenantSplitRuleCache = tenantChannelCache.SplitRules.FirstOrDefault(o => o.Id == eventData.SplitRuleId);
            if (tenantSplitRuleCache == null)
            {
                return;
            }
            var tenantSplitRuleItemCache = tenantSplitRuleCache.ProductClasses.FirstOrDefault(o => o.Id == eventData.SplitRuleItemId);
            if (tenantSplitRuleItemCache == null)
            {
                return;
            }
            tenantSplitRuleItemCache = splitRuleItemCache;
            await _manageCache.SetSplitPackageSettingAsync(eventData.TenantId, tenantSettingCache);
        }

        public virtual async Task HandleEventAsync(BanishSplitRuleItemEvent eventData)
        {
            //update cache for myself
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticCache = settingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (logisticCache == null)
            {
                return;
            }
            var channelCache = logisticCache.LogisticChannels.FirstOrDefault(o => o.Id == eventData.LogisticChannelId);
            if (channelCache == null)
            {
                return;
            }
            var splitRuleCache = channelCache.SplitRules.FirstOrDefault(o => o.Id == eventData.SplitRuleId);
            if (splitRuleCache == null)
            {
                return;
            }
            var splitRuleItemCache = splitRuleCache.ProductClasses.FirstOrDefault(o => o.Id == eventData.SplitRuleItemId);
            if (splitRuleItemCache == null)
            {
                return;
            }
            splitRuleCache.ProductClasses.Remove(splitRuleItemCache);
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
            //associated  import other
            if (!eventData.TenantId.HasValue)
            {
                var importTenantId = await this._tenantLogisticChannelRepository.GetAll()
                    .Where(o => o.LogisticChannelId == eventData.LogisticChannelId)
                    .Select(o => o.TenantId).Distinct().ToListAsync();
                foreach (var item in importTenantId)
                {
                    await this._eventBus.TriggerAsync(new BanishTenantSplitRuleItemEvent()
                    {
                        TenantId = item,
                        LogisticId = eventData.LogisticId,
                        LogisticChannelId = eventData.LogisticChannelId,
                        SplitRuleId = eventData.SplitRuleId,
                        SplitRuleItemId = eventData.SplitRuleItemId
                    });
                }
            }
        }

        public virtual async Task HandleEventAsync(BanishTenantSplitRuleItemEvent eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticCache = settingCache.OwnLogistics.FirstOrDefault(o => o.Id == eventData.LogisticId);
            if (logisticCache == null)
            {
                return;
            }
            var channelCache = logisticCache.LogisticChannels.FirstOrDefault(o => o.Id == eventData.LogisticChannelId);
            if (channelCache == null)
            {
                return;
            }
            var splitRuleCache = channelCache.SplitRules.FirstOrDefault(o => o.Id == eventData.SplitRuleId);
            if (splitRuleCache == null)
            {
                return;
            }
            var splitRuleItemCache = splitRuleCache.ProductClasses.FirstOrDefault(o => o.Id == eventData.SplitRuleItemId);
            if (splitRuleItemCache == null)
            {
                return;
            }
            splitRuleCache.ProductClasses.Remove(splitRuleItemCache);
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
        }
        #endregion

        #region logisticrelation
        public async Task HandleEventAsync(CreateLogisticRelation eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var relationLogistics = this._logisticRepository.GetAll().IgnoreQueryFilters().Where(o => eventData.LogisticIds.Contains(o.Id)).Select(o => new LogisticRelatedOptionCacheDto()
            {
                LogisticId = o.Id,
                LogisticCode = o.LogisticCode
            }).ToList();
            settingCache.Relateds.Add(new LogisticRelatedCacheDto() {
                RelatedId = eventData.RelationId,
                Logistics = relationLogistics
            });
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
        }

        public async Task HandleEventAsync(ModifyLogisticRelation eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticRelationCache = settingCache.Relateds.First(o => o.RelatedId == eventData.RelationId);
            if (eventData.AddLogisticIds != null && eventData.AddLogisticIds.Count > 0)
            {
                var relationLogistics = this._logisticRepository.GetAll().IgnoreQueryFilters().Where(o => eventData.AddLogisticIds.Contains(o.Id)).Select(o => new LogisticRelatedOptionCacheDto()
                {
                    LogisticId = o.Id,
                    LogisticCode = o.LogisticCode
                }).ToList();
                logisticRelationCache.Logistics = logisticRelationCache.Logistics.Union(relationLogistics).ToList();
            }
            if (eventData.RemoveLogisticIds != null && eventData.RemoveLogisticIds.Count > 0)
            {
                logisticRelationCache.Logistics = logisticRelationCache.Logistics.Where(o => !eventData.RemoveLogisticIds.Contains(o.LogisticId)).ToList();
            }
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
        }

        public async Task HandleEventAsync(BanishLogisticRelation eventData)
        {
            var settingCache = await this._manageCache.GetSplitPackageSettingAsync(eventData.TenantId);
            var logisticRelationCache = settingCache.Relateds.First(o => o.RelatedId == eventData.RelationId);
            settingCache.Relateds.Remove(logisticRelationCache);
            await this._manageCache.SetSplitPackageSettingAsync(eventData.TenantId, settingCache);
        }
        #endregion
    }
}
