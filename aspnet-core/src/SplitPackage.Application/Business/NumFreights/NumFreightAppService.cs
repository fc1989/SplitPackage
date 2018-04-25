using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using SplitPackage.Authorization;
using SplitPackage.Business.NumFreights.Dto;
using System.Linq;
using System.Threading.Tasks;

namespace SplitPackage.Business.NumFreights
{
    [AbpAuthorize(PermissionNames.Pages_Tenant_NumFreights)]
    public class NumFreightAppService : AsyncCrudAppService<NumFreight, NumFreightDto, long, PagedResultRequestDto, CreateNumFreightDto, UpdateNumFreightDto>, INumFreightAppService
    {
        public NumFreightAppService(IRepository<NumFreight, long> repository) : base(repository)
        {

        }

        public override async Task<PagedResultDto<NumFreightDto>> GetAll(PagedResultRequestDto input)
        {
            CheckGetAllPermission();

            var query = CreateFilteredQuery(input);

            var totalCount = await AsyncQueryableExecuter.CountAsync(query);

            query = ApplySorting(query, input);
            query = ApplyPaging(query, input);

            var entities = await AsyncQueryableExecuter.ToListAsync(query.Include(p => p.LogisticLineBy));

            return new PagedResultDto<NumFreightDto>(
                totalCount,
                entities.Select(o => new NumFreightDto()
                {
                    Id = o.Id,
                    LogisticLineId = o.LogisticLineId,
                    LogisticLineName = o.LogisticLineBy.LineName,
                    ProductNum = o.ProductNum,
                    PackagePrice = o.PackagePrice,
                    IsActive = o.IsActive
                }).ToList()
            );
        }
    }
}
