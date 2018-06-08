using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Events.Bus;
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
    public class SplitRuleItemAppService : AsyncCrudAppService<SplitRuleProductClass, RuleItemDto, long, SplitRuleItemFilter, CreateRuleItemDto, UpdateRuleItemDto>
    {
        private readonly IRepository<ProductClass, long> _productClassRepository;
        private readonly IRepository<SplitRule, long> _srRepository;
        private readonly IEventBus _eventBus;

        public SplitRuleItemAppService(IRepository<SplitRuleProductClass, long> repository, 
            IRepository<ProductClass, long> productClassRepository,
            IRepository<SplitRule, long> srRepository,
            IEventBus eventBus) :base(repository)
        {
            this._productClassRepository = productClassRepository;
            this._srRepository = srRepository;
            this._eventBus = eventBus;
        }

        protected override IQueryable<SplitRuleProductClass> CreateFilteredQuery(SplitRuleItemFilter input)
        {
            return base.CreateFilteredQuery(input).Where(o=>o.SplitRuleId == input.SplitRuleId).Include(p => p.SplitRuleBy);
        }

        public override async Task<PagedResultDto<RuleItemDto>> GetAll(SplitRuleItemFilter input)
        {
            CheckGetAllPermission();

            var query = CreateFilteredQuery(input);

            var totalCount = await AsyncQueryableExecuter.CountAsync(query);

            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var entities = await AsyncQueryableExecuter.ToListAsync(query);

            var result = entities.Select(MapToEntityDto).ToList();

            var pcSet = _productClassRepository.GetAll().Where(o => result.Select(oi => oi.PTId).Contains(o.PTId));
            result.ForEach(o =>{
                var pc = pcSet.Where(oi => oi.PTId == o.PTId);
                o.ProductClass = string.Format("{0}({1})", string.Join(",", pc.Select(oi => oi.ClassName)), o.PTId);
            });
            return new PagedResultDto<RuleItemDto>(
                totalCount, result
            );
        }

        public async override Task Delete(EntityDto<long> input)
        {
            CheckDeletePermission();

            var entity = await this.Repository.GetAll().Include(p => p.SplitRuleBy).ThenInclude(p=>p.LogisticChannelBy)
                .SingleAsync(o => o.Id == input.Id);

            await Repository.DeleteAsync(entity);
            await this._eventBus.TriggerAsync(new BanishSplitRuleItemEvent()
            {
                TenantId = AbpSession.TenantId,
                LogisticId = entity.SplitRuleBy.LogisticChannelBy.LogisticId,
                LogisticChannelId = entity.SplitRuleBy.LogisticChannelId,
                SplitRuleId = entity.SplitRuleId,
                SplitRuleItemId = entity.Id
            });
        }

        public async override Task<RuleItemDto> Create(CreateRuleItemDto input)
        {
            CheckCreatePermission();

            var splitRule = await this._srRepository.GetAll().Include(p => p.LogisticChannelBy)
                .FirstAsync(o => o.Id == input.SplitRuleId);
            var entity = MapToEntity(input);

            await Repository.InsertAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();
            var @event = this.ObjectMapper.Map<CreateSplitRuleItemEvent>(input);
            @event.TenantId = AbpSession.TenantId;
            @event.LogisticId = splitRule.LogisticChannelBy.LogisticId;
            @event.LogisticChannelId = splitRule.LogisticChannelId;
            await this._eventBus.TriggerAsync(@event);
            return MapToEntityDto(entity);
        }

        public async override Task<RuleItemDto> Update(UpdateRuleItemDto input)
        {
            CheckUpdatePermission();

            var entity = await this.Repository.GetAll().Include(p=>p.SplitRuleBy).ThenInclude(p=>p.LogisticChannelBy)
                .FirstAsync(o=>o.Id == input.Id);
            MapToEntity(input, entity);
            await CurrentUnitOfWork.SaveChangesAsync();
            var @event = this.ObjectMapper.Map<ModifySplitRuleItemEvent>(entity);
            @event.TenantId = AbpSession.TenantId;
            @event.LogisticId = entity.SplitRuleBy.LogisticChannelBy.LogisticId;
            @event.LogisticChannelId = entity.SplitRuleBy.LogisticChannelId;
            return MapToEntityDto(entity);
        }
    }
}
