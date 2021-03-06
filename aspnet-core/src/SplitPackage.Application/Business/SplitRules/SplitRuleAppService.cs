﻿using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
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
        private readonly IRepository<SplitRuleItem, long> _srpRepository;
        private readonly IRepository<TenantLogisticChannel, long> _tlcRepository;
        private readonly IEventBus _eventBus;

        public SplitRuleAppService(IRepository<SplitRule, long> repository, 
            IRepository<SplitRuleItem, long> srpRepository,
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
                        join sr in this.Repository.GetAll() on lc.Id equals sr.LogisticChannelId
                        join tlc in this._tlcRepository.GetAll().Where(o => o.TenantId == tenantId) on lc.Id equals tlc.LogisticChannelId into tlc1
                        from tlcleft in tlc1.DefaultIfEmpty()
                        where sr.TenantId == tenantId || tlcleft != null
                        select sr;
            if (!string.IsNullOrEmpty(input.PTId))
            {
                query = query.Where(o=>o.ProductClasses.Any(oi=>oi.StintMark.StartsWith(input.PTId) && (oi.TenantId == tenantId || oi.TenantId == null)));
            }
            return query.Include(p => p.LogisticChannelBy).ThenInclude((LogisticChannel p) => p.LogisticBy);
        }

        public async override Task<SplitRuleDto> Create(CreateSplitRuleDto input)
        {
            CheckCreatePermission();

            var channel = await this._lcRepository.GetAll().Include(o=>o.LogisticBy).IgnoreQueryFilters().SingleAsync(o=> o.Id == input.LogisticChannelId);

            var entity = MapToEntity(input);

            await Repository.InsertAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();
            var @event = this.ObjectMapper.Map<CreateSplitRuleEvent>(entity);
            @event.LogisticId = channel.LogisticId;
            @event.TenantId = AbpSession.TenantId;
            await this._eventBus.TriggerAsync(@event);
            entity.LogisticChannelBy = channel;
            return MapToEntityDto(entity);
        }

        public async override Task<SplitRuleDto> Get(EntityDto<long> input)
        {
            CheckGetPermission();

            var entity = await GetEntityByIdAsync(input.Id);
            return MapToEntityDto(entity);
        }

        protected async override Task<SplitRule> GetEntityByIdAsync(long id)
        {
            return await Repository.GetAll().IgnoreQueryFilters().Include(p=>p.LogisticChannelBy).ThenInclude(p=>p.LogisticBy).SingleAsync(o=>o.Id == id);
        }

        public async override Task<SplitRuleDto> Update(UpdateSplitRuleDto input)
        {
            CheckUpdatePermission();

            var entity = await this.Repository.GetAll().IgnoreQueryFilters().Include(p => p.LogisticChannelBy).ThenInclude(p=>p.LogisticBy)
                .SingleAsync(o => o.Id == input.Id && o.TenantId == AbpSession.TenantId);

            MapToEntity(input, entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            await this._eventBus.TriggerAsync(new ModifyImportSplitRuleEvent()
            {
                TenantId = AbpSession.TenantId,
                LogisticId = entity.LogisticChannelBy.LogisticId,
                LogisticChannelId = entity.LogisticChannelId,
                SplitRuleId = entity.Id,
                RuleName = input.RuleName,
                MaxPackage = input.MaxPackage,
                MaxWeight = input.MaxWeight,
                MaxTax = input.MaxTax,
                MaxPrice = input.MaxPrice
            });
            return MapToEntityDto(entity);
        }

        public async override Task Delete(EntityDto<long> input)
        {
            CheckDeletePermission();

            var entity = await this.Repository.GetAll().IgnoreQueryFilters().Include(p => p.LogisticChannelBy)
                .SingleAsync(o=>o.Id == input.Id && o.TenantId == AbpSession.TenantId);

            await Repository.DeleteAsync(entity);
            await this._eventBus.TriggerAsync(new BanishSplitRuleEvent()
            {
                TenantId = AbpSession.TenantId,
                LogisticId = entity.LogisticChannelBy.LogisticId,
                LogisticChannelId = entity.LogisticChannelId,
                SplitRuleId = entity.Id
            });
        }

        [UnitOfWork]
        public virtual async Task Switch(long id, bool IsActive)
        {
            CheckPermission(UpdatePermissionName);

            var entity = await this.Repository.GetAll().IgnoreQueryFilters().Include(p=>p.LogisticChannelBy)
                .SingleAsync(o => o.Id == id && o.TenantId == AbpSession.TenantId);
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
                    LogisticChannelId = entity.LogisticChannelId,
                    SplitRuleId = entity.Id
                });
            }
            else
            {
                await this._eventBus.TriggerAsync(new BanishSplitRuleEvent()
                {
                    TenantId = AbpSession.TenantId,
                    LogisticId = entity.LogisticChannelBy.LogisticId,
                    LogisticChannelId = entity.LogisticChannelId,
                    SplitRuleId = entity.Id
                });
            }
        }
    }
}
