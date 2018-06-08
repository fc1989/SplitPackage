using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Events.Bus;
using Abp.UI;
using Microsoft.EntityFrameworkCore;
using SplitPackage.Authorization;
using SplitPackage.Business.Dto;
using SplitPackage.Business.Logistics.Dto;
using SplitPackage.Business.Products.Dto;
using SplitPackage.Domain.Logistic;
using SplitPackage.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.Business.Logistics
{
    [AbpAuthorize(PermissionNames.Pages_Tenant_Logistics)]
    public class LogisticAppService : AsyncCrudAppService<Logistic, LogisticDto, long, LogisticSearchFilter, CreateLogisticDto, UpdateLogisticDto>
    {
        private readonly ILogisticLogic _logic;
        private readonly IEventBus _eventBus;

        public LogisticAppService(IRepository<Logistic, long> repository, 
            ILogisticLogic logic,
            IEventBus eventBus) : base(repository)
        {
            this._logic = logic;
            this._eventBus = eventBus;
        }

        protected override IQueryable<Logistic> CreateFilteredQuery(LogisticSearchFilter input)
        {
            var query = this._logic.GetQuery(AbpSession.TenantId);
            var filter = input.GenerateFilter();
            return filter == null ? query : query.Where(filter);
        }

        public override async Task<LogisticDto> Create(CreateLogisticDto input)
        {
            CheckCreatePermission();

            var entity = await this._logic.Create(input,AbpSession.TenantId);
            await CurrentUnitOfWork.SaveChangesAsync();
            await this._eventBus.TriggerAsync(new CreateLogisticEvent() {
                Id = entity.Id,
                CorporationName = entity.CorporationName,
                CorporationUrl = entity.CorporationUrl,
                LogoURL = entity.LogoURL,
                LogisticCode = entity.LogisticCode,
                TenantId = AbpSession.TenantId
            });
            return MapToEntityDto(entity);
        }

        public override async Task<LogisticDto> Update(UpdateLogisticDto input)
        {
            CheckUpdatePermission();

            var entity = await GetEntityByIdAsync(input.Id);
            MapToEntity(input, entity);
            await CurrentUnitOfWork.SaveChangesAsync();
            await this._eventBus.TriggerAsync(this.ObjectMapper.Map<ModifyLogisticEvent>(entity));
            return MapToEntityDto(entity);
        }

        public override Task Delete(EntityDto<long> input)
        {
            throw new UserFriendlyException("unavailable","logistic can't be delete");
        }

        public async Task Switch(long id, bool IsActive)
        {
            CheckUpdatePermission();

            var entity = await GetEntityByIdAsync(id);
            if (entity.IsActive == IsActive)
            {
                return;
            }
            entity.IsActive = IsActive;
            await CurrentUnitOfWork.SaveChangesAsync();
            if (IsActive)
            {
                await this._eventBus.TriggerAsync(new StartUseLogisticEvent() {
                    TenantId = entity.TenantId,
                    LogisticId = entity.Id
                });
            }
            else
            {
                await this._eventBus.TriggerAsync(new BanishLogisticEvent()
                {
                    TenantId = entity.TenantId,
                    LogisticId = entity.Id
                });
            }
        }

        public async Task<bool> Verify(string flag)
        {
            return await this._logic.Verify(AbpSession.TenantId, flag);
        }
    }
}
