using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SplitPackage.MultiTenancy.Dto;

namespace SplitPackage.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, TenantSearchFilter, CreateTenantDto, TenantDto>
    {
    }
}
