using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SplitPackage.Authorization;
using SplitPackage.Business.SplitRules.Dto;
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

        public SplitRuleAppService(IRepository<SplitRule, long> repository, 
            IRepository<SplitRuleProductClass, long> srpRepository,
            IRepository<Logistic, long> lRepository,
            IRepository<LogisticChannel, long> lcRepository) : base(repository)
        {
            this._srpRepository = srpRepository;
            this._lRepository = lRepository;
            this._lcRepository = lcRepository;
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
                        where sr.TenantId == tenantId
                        select sr;
            if (!string.IsNullOrEmpty(input.PTId))
            {
                query = query.Where(o=>o.ProductClasses.Any(oi=>oi.PTId.StartsWith(input.PTId)));
            }
            return query.Include(p => p.LogisticChannelBy).ThenInclude((LogisticChannel p) => p.LogisticBy);
        }

        public override async Task<SplitRuleDto> Create(CreateSplitRuleDto input)
        {
            CheckCreatePermission();

            var entity = MapToEntity(input);
            entity.TenantId = AbpSession.TenantId;
            await this.Repository.InsertAsync(entity);
            return MapToEntityDto(entity);
        }
    }
}
