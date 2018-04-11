using System.Threading.Tasks;
using SplitPackage.Configuration.Dto;

namespace SplitPackage.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
