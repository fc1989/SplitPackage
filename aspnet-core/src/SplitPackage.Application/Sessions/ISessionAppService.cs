using System.Threading.Tasks;
using Abp.Application.Services;
using SplitPackage.Sessions.Dto;

namespace SplitPackage.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
