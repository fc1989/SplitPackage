using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using SplitPackage.Business.NumFreights.Dto;

namespace SplitPackage.Business.NumFreights
{
    public class NumFreightAppService : AsyncCrudAppService<NumFreight, NumFreightDto, long, PagedResultRequestDto, CreateNumFreightDto, UpdateNumFreightDto>, INumFreightAppService
    {
        public NumFreightAppService(IRepository<NumFreight, long> repository) : base(repository)
        {

        }
    }
}
