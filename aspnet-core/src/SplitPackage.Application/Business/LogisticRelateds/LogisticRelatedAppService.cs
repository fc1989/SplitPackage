using Abp.Application.Services;
using Abp.Domain.Repositories;
using SplitPackage.Business.LogisticRelateds.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Events.Bus;
using SplitPackage.Domain.Logistic;

namespace SplitPackage.Business.LogisticRelateds
{
    public class LogisticRelatedAppService : AsyncCrudAppService<LogisticRelated, LogisticRelatedDto, long, LogisticRelatedSearchFilter, CreateLogisticRelatedDto, UpdateLogisticRelatedDto>
    {
        private readonly IRepository<LogisticRelatedItem, long> _lriRepository;

        private readonly IEventBus _eventBus;

        public LogisticRelatedAppService(IRepository<LogisticRelated, long> repository, IRepository<LogisticRelatedItem, long> lriRepository,
            IEventBus eventBus) : base(repository)
        {
            this._lriRepository = lriRepository;
            this._eventBus = eventBus;
        }

        protected override IQueryable<LogisticRelated> CreateFilteredQuery(LogisticRelatedSearchFilter input)
        {
            return Repository.GetAll().Where(o=>o.TenantId == AbpSession.TenantId).IgnoreQueryFilters().Include(p=>p.Items).ThenInclude((LogisticRelatedItem p)=>p.LogisticBy);
        }

        public async override Task<LogisticRelatedDto> Create(CreateLogisticRelatedDto input)
        {
            CheckCreatePermission();

            var entity = MapToEntity(input);
            var id = await Repository.InsertAndGetIdAsync(entity);
            foreach (var item in input.LogisticIds)
            {
                await this._lriRepository.InsertAsync(new LogisticRelatedItem() {
                    LogisticRelatedId = id,
                    LogisticId = item
                });
            }
            await CurrentUnitOfWork.SaveChangesAsync();
            await this._eventBus.TriggerAsync(new CreateLogisticRelation()
            {
                TenantId = AbpSession.TenantId,
                RelationId = id,
                LogisticIds = input.LogisticIds
            });
            return MapToEntityDto(entity);
        }

        public async override Task<LogisticRelatedDto> Update(UpdateLogisticRelatedDto input)
        {
            CheckUpdatePermission();

            var entity = await GetEntityByIdAsync(input.Id);
            MapToEntity(input, entity);
            var items = await this._lriRepository.GetAllListAsync(o => o.LogisticRelatedId == input.Id);
            var addSet = input.LogisticIds.Where(o => !items.Any(oi => oi.LogisticId == o));
            foreach (var item in addSet)
            {
                await this._lriRepository.InsertAsync(new LogisticRelatedItem()
                {
                    LogisticId = item,
                    LogisticRelatedId = input.Id
                });
            }
            var deleteSet = items.Where(o => !input.LogisticIds.Contains(o.LogisticId));
            foreach (var item in deleteSet)
            {
                await this._lriRepository.DeleteAsync(item);
            }
            await CurrentUnitOfWork.SaveChangesAsync();
            await this._eventBus.TriggerAsync(new ModifyLogisticRelation()
            {
                TenantId = AbpSession.TenantId,
                RelationId = input.Id,
                AddLogisticIds = addSet.ToList(),
                RemoveLogisticIds = deleteSet.Select(o=>o.LogisticId).ToList()
            });
            return MapToEntityDto(entity);
        }

        public async override Task Delete(EntityDto<long> input)
        {
            CheckDeletePermission();
            var entity = await this.Repository.SingleAsync(o=>o.Id == input.Id);
            await Repository.DeleteAsync(entity);
            await this._eventBus.TriggerAsync(new BanishLogisticRelation() {
                TenantId = AbpSession.TenantId,
                RelationId = input.Id
            });
        }
    }
}
