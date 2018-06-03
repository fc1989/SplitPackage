using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Events.Bus;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SplitPackage.Authorization;
using SplitPackage.Business.SplitRules.Dto;
using SplitPackage.Domain.Logistic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.Business.SplitRules
{
    [AbpAuthorize(PermissionNames.Pages_Tenant_SplitRules)]
    public class SplitRuleAppService : AsyncCrudAppService<SplitRule, SplitRuleDto, long, SplitRuleSearchFilter, CreateSplitRuleDto, UpdateSplitRuleDto>
    {
        private readonly IRepository<Logistic, long> _lRepository;
        private readonly IRepository<LogisticChannel, long> _lcRepository;
        private readonly IRepository<SplitRuleProductClass, long> _srpRepository;
        private readonly IRepository<TenantLogisticChannel, long> _tlcRepository;
        private readonly IEventBus _eventBus;

        public SplitRuleAppService(IRepository<SplitRule, long> repository, 
            IRepository<SplitRuleProductClass, long> srpRepository,
            IRepository<Logistic, long> lRepository,
            IRepository<LogisticChannel, long> lcRepository,
            IRepository<TenantLogisticChannel, long> tlcRepository,
            IEventBus eventBus) : base(repository)
        {
            this._srpRepository = srpRepository;
            this._lRepository = lRepository;
            this._lcRepository = lcRepository;
            this._tlcRepository = tlcRepository;
            this._eventBus = eventBus;
        }

        protected override IQueryable<SplitRule> CreateFilteredQuery(SplitRuleSearchFilter input)
        {
            var tenantId = AbpSession.TenantId;
            IQueryable<Logistic> lQuery = null;
            IQueryable<LogisticChannel> lcQuery = null;
            if (!string.IsNullOrEmpty(input.LogisticName))
            {
                lQuery = this._lRepository.GetAll().IgnoreQueryFilters().Where(o => o.CorporationName.StartsWith(input.LogisticName) && !o.IsDeleted && o.IsActive);
            }
            else
            {
                lQuery = this._lRepository.GetAll().IgnoreQueryFilters();
            }
            if (!string.IsNullOrEmpty(input.ChannelName))
            {
                lcQuery = this._lcRepository.GetAll().IgnoreQueryFilters().Where(o => o.ChannelName.StartsWith(input.ChannelName) && !o.IsDeleted && o.IsActive);
            }
            else
            {
                lcQuery = this._lcRepository.GetAll().IgnoreQueryFilters();
            }
            var query = from l in lQuery
                        join lc in lcQuery on l.Id equals lc.LogisticId
                        join tlc in this._tlcRepository.GetAll() on lc.Id equals tlc.LogisticChannelId into tlc1
                        from tlcleft in tlc1.DefaultIfEmpty()
                        join sr in this.Repository.GetAll() on lc.Id equals sr.LogisticChannelId
                        where lc.TenantId == tenantId || tlcleft.TenantId == tenantId
                        select sr;
            if (!string.IsNullOrEmpty(input.PTId))
            {
                query = query.Where(o=>o.ProductClasses.Any(oi=>oi.PTId.StartsWith(input.PTId)));
            }
            return query.Include(p => p.LogisticChannelBy).ThenInclude((LogisticChannel p) => p.LogisticBy);
        }

        public async override Task Delete(EntityDto<long> input)
        {
            CheckDeletePermission();

            var entity = await this.Repository.GetAll().Include(p => p.LogisticChannelBy)
                .SingleAsync(o=>o.Id == input.Id);

            await Repository.DeleteAsync(entity);
            await this._eventBus.TriggerAsync(new BanishSplitRuleEvent()
            {
                TenantId = AbpSession.TenantId,
                LogisticId = entity.LogisticChannelBy.LogisticId,
                ChannelId = entity.LogisticChannelId,
                SplitRuleId = entity.Id
            });
        }

        public async Task Switch(long id, bool IsActive)
        {
            CheckPermission(UpdatePermissionName);

            var entity = await this.Repository.GetAll().Include(p=>p.LogisticChannelBy)
                .SingleAsync(o => o.Id == id);
            if (entity.IsActive == IsActive)
            {
                return;
            }
            entity.IsActive = IsActive;
            await CurrentUnitOfWork.SaveChangesAsync();
            if (IsActive)
            {
                await this._eventBus.TriggerAsync(new StartUseSplitRuleEvent()
                {
                    TenantId = AbpSession.TenantId,
                    LogisticId = entity.LogisticChannelBy.LogisticId,
                    ChannelId = entity.LogisticChannelId,
                    SplitRuleId = entity.Id
                });
            }
            else
            {
                await this._eventBus.TriggerAsync(new BanishSplitRuleEvent()
                {
                    TenantId = AbpSession.TenantId,
                    LogisticId = entity.LogisticChannelBy.LogisticId,
                    ChannelId = entity.LogisticChannelId,
                    SplitRuleId = entity.Id
                });
            }
        }
    }
}
