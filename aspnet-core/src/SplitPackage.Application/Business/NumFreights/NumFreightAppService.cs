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

        protected override IQueryable<NumFreight> CreateFilteredQuery(PagedResultRequestDto input)
        {
            return base.CreateFilteredQuery(input).Include(p=>p.LogisticChannelBy);
        }

        protected override NumFreightDto MapToEntityDto(NumFreight entity)
        {
            var result = base.MapToEntityDto(entity);
            result.LogisticChannelName = entity.LogisticChannelBy.ChannelName;
            return result;
        }
    }
}
