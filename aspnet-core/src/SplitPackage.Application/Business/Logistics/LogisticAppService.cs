using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SplitPackage.Authorization;
using SplitPackage.Business.Dto;
using SplitPackage.Business.Logistics.Dto;
using SplitPackage.Business.Products.Dto;
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

        public LogisticAppService(IRepository<Logistic, long> repository, ILogisticLogic logic) : base(repository)
        {
            this._logic = logic;
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
            return MapToEntityDto(entity);
        }

        public async Task<bool> Verify(string flag)
        {
            return await this._logic.Verify(AbpSession.TenantId, flag);
        }
    }
}
