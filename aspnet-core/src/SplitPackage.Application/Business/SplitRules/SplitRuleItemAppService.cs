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
    public class SplitRuleItemAppService : AsyncCrudAppService<SplitRuleItem, RuleItemDto, long, SplitRuleItemFilter, CreateRuleItemDto, UpdateRuleItemDto>
    {
        private readonly IRepository<ProductClass, long> _productClassRepository;
        private readonly IRepository<LogisticChannel, long> _lcRepository;
        private readonly IRepository<SplitRule, long> _srRepository;
        private readonly IRepository<TenantLogisticChannel, long> _tlcRepository;
        private readonly IRepository<Product, long> _productRepository;
        private readonly IEventBus _eventBus;

        public SplitRuleItemAppService(IRepository<SplitRuleItem, long> repository,
            IRepository<ProductClass, long> productClassRepository,
            IRepository<LogisticChannel, long> lcRepository,
            IRepository<SplitRule, long> srRepository,
            IRepository<TenantLogisticChannel, long> tlcRepository,
            IRepository<Product, long> pRepository,
            IEventBus eventBus) :base(repository)
        {
            this._productClassRepository = productClassRepository;
            this._lcRepository = lcRepository;
            this._srRepository = srRepository;
            this._tlcRepository = tlcRepository;
            this._productRepository = pRepository;
            this._eventBus = eventBus;
        }

        protected override IQueryable<SplitRuleItem> CreateFilteredQuery(SplitRuleItemFilter input)
        {
            var tenantId = AbpSession.TenantId;
            var query = from lc in this._lcRepository.GetAll().IgnoreQueryFilters().Where(o=>!o.IsDeleted)
                        join sr in this._srRepository.GetAll().IgnoreQueryFilters() on lc.Id equals sr.LogisticChannelId
                        join srp in this.Repository.GetAll().IgnoreQueryFilters() on sr.Id equals srp.SplitRuleId
                        join tlc in this._tlcRepository.GetAll().Where(o => o.TenantId == tenantId) on lc.Id equals tlc.LogisticChannelId into tlc1
                        from tlcleft in tlc1.DefaultIfEmpty()
                        where srp.SplitRuleId == input.SplitRuleId && (srp.TenantId == tenantId || tlcleft != null)
                        select srp;
            return query.Include(p => p.SplitRuleBy);
        }

        public override async Task<PagedResultDto<RuleItemDto>> GetAll(SplitRuleItemFilter input)
        {
            CheckGetAllPermission();

            var query = CreateFilteredQuery(input);

            var totalCount = await AsyncQueryableExecuter.CountAsync(query);

            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var entities = await AsyncQueryableExecuter.ToListAsync(query);
            List<ProductClass> pcSet = new List<ProductClass>();
            List<Product> pSet = new List<Product>();
            var ptidSet = entities.Where(oi => oi.Type == RuleItemStintType.PTId).Select(o => o.StintMark).Distinct().ToList();
            var skuSet = entities.Where(oi => oi.Type == RuleItemStintType.Sku).Select(o => o.StintMark).Distinct().ToList();
            if (ptidSet.Count > 0)
            {
                pcSet = this._productClassRepository.GetAll().Where(o => ptidSet.Contains(o.PTId)).ToList();
            }
            if (skuSet.Count > 0)
            {
                pSet = this._productRepository.GetAll().Where(o => skuSet.Contains(o.Sku)).ToList();
            }
            var result = new List<RuleItemDto>();
            foreach (var item in entities)
            {
                var entity = MapToEntityDto(item);
                if (item.Type == RuleItemStintType.PTId)
                {
                    var pc = pcSet.Where(oi => oi.PTId == entity.StintMark);
                    entity.ProductClass = string.Format("{0}({1})", string.Join(",", pc.Select(oi => oi.ClassName)), entity.StintMark);
                }
                else
                {
                    var p = pSet.Where(oi => oi.Sku.Equals(entity.StintMark));
                    entity.ProductClass = string.Format("{0}({1})", string.Join(",", p.Select(oi => oi.ProductName)), entity.StintMark);
                }
                result.Add(entity);
            }
            return new PagedResultDto<RuleItemDto>(totalCount, result);
        }

        protected override Task<SplitRuleItem> GetEntityByIdAsync(long id)
        {
            return Repository.GetAll().IgnoreQueryFilters().Include(p => p.SplitRuleBy).FirstAsync(o => o.Id == id);
        }

        public override async Task<RuleItemDto> Get(EntityDto<long> input)
        {
            CheckGetPermission();

            var entity = await GetEntityByIdAsync(input.Id);
            var result = MapToEntityDto(entity);
            if (entity.Type == RuleItemStintType.PTId)
            {
                var pc = this._productClassRepository.GetAll().Where(o => o.PTId.Equals(entity.StintMark));
                result.ProductClass = string.Format("{0}({1})", string.Join(",", pc.Select(oi => oi.ClassName)), entity.StintMark);
            }
            else if(entity.Type == RuleItemStintType.Sku)
            {
                var pc = this._productRepository.GetAll().Where(o => o.Sku.Equals(entity.StintMark));
                result.ProductClass = string.Format("{0}({1})", string.Join(",", pc.Select(oi => oi.ProductName)), entity.StintMark);
            }
            return result;
        }

        public async override Task Delete(EntityDto<long> input)
        {
            CheckDeletePermission();

            var entity = await this.Repository.GetAll().IgnoreQueryFilters().Include(p => p.SplitRuleBy).ThenInclude(p=>p.LogisticChannelBy)
                .SingleAsync(o => o.Id == input.Id && o.TenantId == AbpSession.TenantId);

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

            var splitRule = await this._srRepository.GetAll().IgnoreQueryFilters().Include(p => p.LogisticChannelBy)
                .FirstAsync(o => o.Id == input.SplitRuleId);
            var entity = MapToEntity(input);

            await Repository.InsertAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();
            var @event = this.ObjectMapper.Map<CreateSplitRuleItemEvent>(entity);
            @event.TenantId = AbpSession.TenantId;
            @event.LogisticId = splitRule.LogisticChannelBy.LogisticId;
            @event.LogisticChannelId = splitRule.LogisticChannelId;
            await this._eventBus.TriggerAsync(@event);
            entity.SplitRuleBy = splitRule;
            var result = MapToEntityDto(entity);
            if (entity.Type == RuleItemStintType.PTId)
            {
                var pc = this._productClassRepository.GetAll().Where(o => o.PTId.Equals(entity.StintMark));
                result.ProductClass = string.Format("{0}({1})", string.Join(",", pc.Select(oi => oi.ClassName)), entity.StintMark);
            }
            else if (entity.Type == RuleItemStintType.Sku)
            {
                var pc = this._productRepository.GetAll().Where(o => o.Sku.Equals(entity.StintMark));
                result.ProductClass = string.Format("{0}({1})", string.Join(",", pc.Select(oi => oi.ProductName)), entity.StintMark);
            }
            return result;
        }

        public async override Task<RuleItemDto> Update(UpdateRuleItemDto input)
        {
            CheckUpdatePermission();

            var entity = await this.Repository.GetAll().IgnoreQueryFilters().Include(p=>p.SplitRuleBy).ThenInclude(p=>p.LogisticChannelBy)
                .FirstAsync(o=>o.Id == input.Id && o.TenantId == AbpSession.TenantId);
            MapToEntity(input, entity);
            await CurrentUnitOfWork.SaveChangesAsync();
            var @event = this.ObjectMapper.Map<ModifySplitRuleItemEvent>(entity);
            @event.TenantId = AbpSession.TenantId;
            @event.LogisticId = entity.SplitRuleBy.LogisticChannelBy.LogisticId;
            @event.LogisticChannelId = entity.SplitRuleBy.LogisticChannelId;
            await this._eventBus.TriggerAsync(@event);
            var result = MapToEntityDto(entity);
            if (entity.Type == RuleItemStintType.PTId)
            {
                var pc = this._productClassRepository.GetAll().Where(o => o.PTId.Equals(entity.StintMark));
                result.ProductClass = string.Format("{0}({1})", string.Join(",", pc.Select(oi => oi.ClassName)), entity.StintMark);
            }
            else if (entity.Type == RuleItemStintType.Sku)
            {
                var pc = this._productRepository.GetAll().Where(o => o.Sku.Equals(entity.StintMark));
                result.ProductClass = string.Format("{0}({1})", string.Join(",", pc.Select(oi => oi.ProductName)), entity.StintMark);
            }
            return result;
        }
    }
}
