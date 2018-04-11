using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace SplitPackage.Controllers
{
    public abstract class SplitPackageControllerBase: AbpController
    {
        protected SplitPackageControllerBase()
        {
            LocalizationSourceName = SplitPackageConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
