using System.Threading.Tasks;
using Abp.Application.Services;
using SplitPackage.Authorization.Accounts.Dto;

namespace SplitPackage.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
