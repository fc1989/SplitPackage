using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SplitPackage.Authorization;
using SplitPackage.Business.WeightFreights.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitPackage.Business.WeightFreights
{
    [AbpAuthorize(PermissionNames.Pages_Tenant_WeightFreights)]
    public class WeightFreightAppService : AsyncCrudAppService<WeightFreight, WeightFreightDto, long, PagedResultRequestDto, CreateWeightFreightDto, UpdateWeightFreightDto>, IWeightFreightAppService
    {
        public WeightFreightAppService(IRepository<WeightFreight, long> repository) : base(repository)
        {

        }

        public override async Task<PagedResultDto<WeightFreightDto>> GetAll(PagedResultRequestDto input)
        {
            CheckGetAllPermission();

            var query = CreateFilteredQuery(input);

            var totalCount = await AsyncQueryableExecuter.CountAsync(query);

            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var entities = await AsyncQueryableExecuter.ToListAsync(query.Include(p => p.LogisticLineBy));

            return new PagedResultDto<WeightFreightDto>(
                totalCount,
                entities.Select(o => new WeightFreightDto()
                {
                    Id = o.Id,
                    LogisticLineId = o.LogisticLineId,
                    LogisticLineName = o.LogisticLineBy.LineName,
                    StartingWeight = o.StartingWeight,
                    StartingPrice = o.StartingPrice,
                    StepWeight = o.StepWeight,
                    Price = o.Price,
                    IsActive = o.IsActive
                }).ToList()
            );
        }
    }
}
