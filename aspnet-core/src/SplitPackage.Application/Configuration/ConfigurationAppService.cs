using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using SplitPackage.Configuration.Dto;

namespace SplitPackage.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : SplitPackageAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
