using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using SplitPackage.Roles.Dto;
using SplitPackage.Users.Dto;

namespace SplitPackage.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedResultRequestDto, CreateUserDto, UserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();

        Task ChangeLanguage(ChangeUserLanguageDto input);
    }
}
