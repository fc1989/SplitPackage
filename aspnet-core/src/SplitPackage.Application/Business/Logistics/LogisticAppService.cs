using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using SplitPackage.Business.Logistics.Dto;
using SplitPackage.Business.Products.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.Business.Logistics
{
    public class LogisticAppService : AsyncCrudAppService<Logistic, LogisticDto, long, PagedResultRequestDto, CreateLogisticDto, UpdateLogisticDto>, ILogisticAppService
    {
        public LogisticAppService(IRepository<Logistic, long> repository) : base(repository)
        {

        }

        public override async Task<LogisticDto> Create(CreateLogisticDto input)
        {
            CheckCreatePermission();

            var entity = MapToEntity(input);
            entity.TenantId = AbpSession.TenantId;

            await Repository.InsertAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            return MapToEntityDto(entity);
        }
    }
}
